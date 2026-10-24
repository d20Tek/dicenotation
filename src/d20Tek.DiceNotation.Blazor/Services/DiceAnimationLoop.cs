using d20Tek.DiceNotation.Blazor.Interop;
using d20Tek.DiceNotation.Blazor.Models;
using Microsoft.JSInterop;

namespace d20Tek.DiceNotation.Blazor.Services;

/// <summary>
/// Drives one dice animation by connecting a physics simulation to a Canvas renderer
/// through a single browser frame scheduler.
/// </summary>
/// <remarks>
/// <para>
/// The browser owns frame timing, this type owns the physics, and the renderer owns
/// drawing. Each displayed frame advances the simulation by the elapsed time and sends
/// exactly one batched snapshot to the renderer, so interop traffic stays proportional
/// to displayed frames rather than to dice or physics substeps.
/// </para>
/// <para>
/// This type holds mutable animation state, so each component instance should own its
/// own loop. Results are fixed before an animation starts, so a rendering or interop
/// failure stops the animation without changing any die value.
/// </para>
/// </remarks>
public sealed class DiceAnimationLoop : IAsyncDisposable
{
    private readonly IDiceSimulation _simulation;
    private readonly CanvasRenderer _renderer;
    private readonly DiceSimulationSettings _settings;
    private readonly System.Diagnostics.Stopwatch _frameTimer = new();
    private DotNetObjectReference<DiceAnimationLoop>? _selfReference;
    private DiceTraySize _traySize = DiceTraySize.Default;
    private DiceTheme? _theme;
    private bool _isRunning;
    private bool _completionRaised;
    private double _frameTotalMilliseconds;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiceAnimationLoop"/> class.
    /// </summary>
    /// <param name="simulation">The simulation that advances the scene.</param>
    /// <param name="renderer">The renderer that draws each snapshot.</param>
    /// <param name="settings">
    /// The settings used for frame clamping, or <see langword="null"/> to use the defaults.
    /// These must match the settings used by the supplied simulation.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="simulation"/> or <paramref name="renderer"/> is
    /// <see langword="null"/>.
    /// </exception>
    public DiceAnimationLoop(
        IDiceSimulation simulation,
        CanvasRenderer renderer,
        DiceSimulationSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(simulation);
        ArgumentNullException.ThrowIfNull(renderer);

        _simulation = simulation;
        _renderer = renderer;
        _settings = settings ?? new DiceSimulationSettings();
        _settings.Validate();
    }

    /// <summary>
    /// Occurs when an animation finishes, either because every die settled or because the
    /// roll reached its maximum duration. It is raised at most once per animation.
    /// </summary>
    public event Func<DiceSceneState, Task>? AnimationCompleted;

    /// <summary>
    /// Occurs when a frame fails. The animation stops, but the already-determined die
    /// results remain valid and should still be displayed.
    /// </summary>
    public event Func<Exception, Task>? AnimationFailed;

    /// <summary>Gets a value indicating whether an animation is currently running.</summary>
    public bool IsRunning => _isRunning;

    /// <summary>Gets the number of frames processed during the current animation.</summary>
    public int FrameCount { get; private set; }

    /// <summary>
    /// Gets the time taken by the most recent frame, in milliseconds. This covers the
    /// physics stepping and the batched interop render, so it can be used to measure
    /// interop cost rather than estimating it.
    /// </summary>
    public double LastFrameMilliseconds { get; private set; }

    /// <summary>Gets the slowest frame of the current animation, in milliseconds.</summary>
    public double WorstFrameMilliseconds { get; private set; }

    /// <summary>Gets the mean frame time of the current animation, in milliseconds.</summary>
    public double AverageFrameMilliseconds =>
        FrameCount == 0 ? 0 : _frameTotalMilliseconds / FrameCount;

    /// <summary>
    /// Starts a new animation for the supplied dice. Any animation already in progress is
    /// replaced, which never produces a second completion notification for the old one.
    /// </summary>
    /// <param name="dice">The dice to animate, each carrying its precomputed result.</param>
    /// <param name="traySize">The logical dimensions of the tray.</param>
    /// <param name="theme">The theme to draw with, or <see langword="null"/> for the default.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that completes once the scheduler is running.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="dice"/> or <paramref name="traySize"/> is <see langword="null"/>.
    /// </exception>
    public async ValueTask StartAsync(
        IReadOnlyList<VisualDie> dice,
        DiceTraySize traySize,
        DiceTheme? theme = null,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(dice);
        ArgumentNullException.ThrowIfNull(traySize);

        // Replacing an active animation must not leave the old scheduler running.
        await StopAsync(cancellationToken);

        _traySize = traySize;
        _theme = theme;
        _completionRaised = false;
        FrameCount = 0;
        LastFrameMilliseconds = 0;
        WorstFrameMilliseconds = 0;
        _frameTotalMilliseconds = 0;
        _simulation.StartRoll(dice, traySize);

        // Draw the launch frame immediately so the tray is never briefly empty.
        await RenderCurrentAsync(cancellationToken);

        _selfReference ??= DotNetObjectReference.Create(this);
        _isRunning = true;
        await _renderer.StartLoopAsync(_selfReference, _settings.MaxFrameSeconds, cancellationToken);
    }

    /// <summary>
    /// Stops the current animation without raising a completion notification. Calling this
    /// when no animation is running has no effect.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that completes once the scheduler has stopped.</returns>
    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!_isRunning) return;

        _isRunning = false;
        await _renderer.StopLoopAsync(cancellationToken);
    }

    /// <summary>
    /// Advances the simulation and draws one frame. This is invoked by the browser frame
    /// scheduler and should not be called directly by application code.
    /// </summary>
    /// <param name="elapsedSeconds">The elapsed time since the previous frame, in seconds.</param>
    /// <returns>A task that completes when the frame has been drawn.</returns>
    [JSInvokable]
    public async Task OnFrameAsync(double elapsedSeconds)
    {
        if (_disposed || !_isRunning) return;
        try
        {
            _frameTimer.Restart();

            // The simulation bounds its own catch-up work, so a large delta cannot
            // produce unbounded stepping here.
            _simulation.Step((float)elapsedSeconds);
            await RenderCurrentAsync(CancellationToken.None);

            _frameTimer.Stop();
            RecordFrame(_frameTimer.Elapsed.TotalMilliseconds);

            if (_simulation.IsSettled) await CompleteAsync();
        }
        catch (Exception ex)
        {
            await FailAsync(ex);
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        _disposed = true;
        _isRunning = false;

        // The renderer stops the scheduler as part of its own disposal, so a queued
        // frame cannot run against the released object reference.
        await _renderer.DisposeAsync();

        _selfReference?.Dispose();
        _selfReference = null;
        _simulation.Dispose();
    }

    private void RecordFrame(double milliseconds)
    {
        FrameCount++;
        LastFrameMilliseconds = milliseconds;
        _frameTotalMilliseconds += milliseconds;

        if (milliseconds > WorstFrameMilliseconds)
        {
            WorstFrameMilliseconds = milliseconds;
        }
    }

    private async ValueTask RenderCurrentAsync(CancellationToken cancellationToken)
    {
        var snapshot = _simulation.GetSnapshot();
        await _renderer.RenderAsync(snapshot, _traySize, _theme, cancellationToken);
    }

    private async Task CompleteAsync()
    {
        if (_completionRaised) return;

        _completionRaised = true;
        await StopAsync();

        var handler = AnimationCompleted;
        if (handler is not null)
        {
            await handler(_simulation.GetSnapshot());
        }
    }

    private async Task FailAsync(Exception exception)
    {
        _isRunning = false;

        // A failed frame must never leave the scheduler running, but stopping it may
        // fail for the same reason the frame did.
        try
        {
            await _renderer.StopLoopAsync();
        }
        catch (Exception ex) when (ex is JSException or
                                         JSDisconnectedException or
                                         ObjectDisposedException or
                                         OperationCanceledException)
        {
        }

        var handler = AnimationFailed;
        if (handler is not null) await handler(exception);
    }
}
