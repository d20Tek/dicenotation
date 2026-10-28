using d20Tek.DiceNotation.Blazor.Interop;
using d20Tek.DiceNotation.Blazor.Models;
using d20Tek.DiceNotation.Blazor.Services;
using d20Tek.DiceNotation.Results;
using Microsoft.AspNetCore.Components;

namespace d20Tek.DiceNotation.Blazor.Components;

/// <summary>
/// Animates a completed dice roll in a two-dimensional tray drawn on an HTML canvas.
/// </summary>
/// <remarks>
/// <para>
/// This component never rolls dice. It receives a <see cref="DiceResult"/> that the
/// notation library has already produced and animates the values it contains, so the
/// faces that come to rest always match the result the application calculated. A
/// rendering or interop failure is reported through <see cref="OnRollFailed"/> and never
/// changes a die value.
/// </para>
/// <para>
/// Each instance owns its own simulation, renderer, and canvas, so several rollers can
/// appear on one page. The component draws through the browser's own frame scheduler and
/// does not re-render itself per frame.
/// </para>
/// </remarks>
public sealed partial class VisualDiceRoller : ComponentBase, IAsyncDisposable
{
    private ElementReference _canvas;
    private CanvasRenderer? _renderer;
    private DiceAnimationLoop? _loop;
    private DiceResult? _animatedResult;
    private DiceRollSummary _summary = DiceRollSummary.Empty;
    private bool _initialized;
    private bool _disposed;

    [Inject]
    private IDiceAnimationLoopFactory LoopFactory { get; set; } = default!;

    [Inject]
    private DiceRollerOptions DefaultOptions { get; set; } = DiceRollerOptions.Default;

    /// <summary>
    /// Gets or sets the completed roll to animate. Assigning a different result starts a
    /// new animation, replacing any animation already in progress. A result representing
    /// a failed roll is reported through <see cref="OnRollFailed"/> and is not animated.
    /// </summary>
    [Parameter]
    public DiceResult? Result { get; set; }

    /// <summary>
    /// Gets or sets the tray, theme, presentation, and physics configuration. When this is
    /// <see langword="null"/> the defaults are used.
    /// </summary>
    [Parameter]
    public DiceRollerOptions? Options { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether new animations are suppressed. Setting this
    /// does not stop an animation that is already running.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes applied to the component's container element.
    /// </summary>
    [Parameter]
    public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets the accessible label describing the canvas. When this is
    /// <see langword="null"/> a label is generated from the current result, so the outcome
    /// is available to assistive technology without reading the canvas.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the dice come to rest. The summary carries
    /// every die in the roll along with the authoritative total from the notation library.
    /// </summary>
    [Parameter]
    public EventCallback<DiceRollSummary> OnRollCompleted { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when an animation cannot start or cannot continue.
    /// The already-determined results remain valid, so callers should keep displaying them.
    /// </summary>
    [Parameter]
    public EventCallback<Exception> OnRollFailed { get; set; }

    /// <summary>Gets a value indicating whether dice are currently animating.</summary>
    public bool IsRolling => _loop?.IsRunning ?? false;

    // Options supplied on the component win, then the application-wide defaults supplied
    // during registration, which fall back to the library defaults when nothing is configured.
    private DiceRollerOptions ResolvedOptions => Options ?? DefaultOptions;

    private string AspectRatio
    {
        get
        {
            var tray = ResolvedOptions.TraySize;
            return $"{tray.Width.ToString(System.Globalization.CultureInfo.InvariantCulture)} / " +
                   $"{tray.Height.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
        }
    }

    private string AccessibleLabel
    {
        get
        {
            if (AriaLabel is not null) return AriaLabel;
            if (_summary.Dice.Count == 0) return "Dice tray";

            var values = string.Join(", ", _summary.Dice.Select(d => d.Value));
            return $"Dice tray showing {_summary.Dice.Count} dice with values {values}, totaling {_summary.Total}.";
        }
    }

    /// <summary>
    /// Replays the current result. This is useful when a new roll produces the same values
    /// as the previous one, because assigning an equal result does not restart the
    /// animation on its own.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that completes once the animation has started.</returns>
    public async Task ReplayAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed || Disabled || Result is null) return;

        await StartAnimationAsync(Result, cancellationToken);
    }

    /// <summary>
    /// Stops the current animation without raising a completion notification.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that completes once the animation has stopped.</returns>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_loop is null) return;

        await _loop.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Recomputes the canvas backing buffer and redraws the current scene. Call this when
    /// the surrounding layout changes in a way the browser cannot detect on its own.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that completes once the canvas has been redrawn.</returns>
    public async Task ResizeAsync(CancellationToken cancellationToken = default)
    {
        if (_loop is null) return;

        await _loop.ResizeAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || _disposed) return;

        try
        {
            var options = ResolvedOptions;
            options.Validate();

            var (renderer, loop) = LoopFactory.Create(options.SimulationSettings);
            _renderer = renderer;
            _loop = loop;
            _loop.AnimationCompleted += OnAnimationCompletedAsync;
            _loop.AnimationFailed += OnAnimationFailedAsync;

            await _renderer.InitializeAsync(_canvas);
            _initialized = true;

            // A result supplied before the canvas existed still has to animate once.
            if (Result is not null && !Disabled)
            {
                await StartAnimationAsync(Result, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            await RaiseFailureAsync(ex);
        }
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        if (_disposed || !_initialized || Disabled || Result is null) return;

        // Reference equality is deliberate. Each roll produces a new result instance, and
        // re-animating an equal result is an explicit ReplayAsync call.
        if (ReferenceEquals(Result, _animatedResult)) return;

        await StartAnimationAsync(Result, CancellationToken.None);
    }

    private async Task StartAnimationAsync(DiceResult result, CancellationToken cancellationToken)
    {
        if (_loop is null || !_initialized) return;

        _animatedResult = result;

        try
        {
            var options = ResolvedOptions;
            options.Validate();

            // Mapping happens before any animation work, so an unusable result fails fast
            // without leaving the tray in a rolling state.
            _summary = VisualDieFactory.CreateSummary(result);

            if (_summary.Dice.Count == 0)
            {
                // A modifier-only expression has nothing to animate, but it still has a
                // total, so it completes immediately rather than appearing to hang.
                await RaiseCompletedAsync();
                return;
            }

            await _loop.StartAsync(
                _summary.Dice,
                options.TraySize,
                options.ResolveTheme(),
                options.VisualSettings,
                cancellationToken);
        }
        catch (Exception ex)
        {
            await RaiseFailureAsync(ex);
        }
    }

    private async Task OnAnimationCompletedAsync(DiceSceneState scene)
    {
        _ = scene;
        await RaiseCompletedAsync();
    }

    private async Task OnAnimationFailedAsync(Exception exception) => await RaiseFailureAsync(exception);

    private async Task RaiseCompletedAsync()
    {
        // Re-render once when the animation ends so the accessible label reflects the
        // settled results. This never happens per frame.
        StateHasChanged();

        if (OnRollCompleted.HasDelegate)
        {
            await OnRollCompleted.InvokeAsync(_summary);
        }
    }

    private async Task RaiseFailureAsync(Exception exception)
    {
        if (OnRollFailed.HasDelegate)
        {
            await OnRollFailed.InvokeAsync(exception);
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        _disposed = true;

        if (_loop is not null)
        {
            _loop.AnimationCompleted -= OnAnimationCompletedAsync;
            _loop.AnimationFailed -= OnAnimationFailedAsync;

            // The loop disposes the renderer and simulation it was given.
            await _loop.DisposeAsync();
        }

        _loop = null;
        _renderer = null;
    }
}
