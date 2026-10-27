using d20Tek.DiceNotation.Blazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace d20Tek.DiceNotation.Blazor.Interop;

/// <summary>
/// Wraps the Canvas JavaScript module. Each renderer instance owns one canvas element
/// and its associated JavaScript resources.
/// </summary>
/// <remarks>
/// Rendering failures must never change a die result. Callers should treat a failed
/// render as a visual problem only and continue to present the already-determined results.
/// </remarks>
internal sealed class CanvasRenderer : IAsyncDisposable
{
    private const string ModulePath = "./_content/d20Tek.DiceNotation.Blazor/js/dice-roller.js";

    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;
    private int _handle;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="CanvasRenderer"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime used to import the Canvas module.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="jsRuntime"/> is <see langword="null"/>.
    /// </exception>
    public CanvasRenderer(IJSRuntime jsRuntime)
    {
        ArgumentNullException.ThrowIfNull(jsRuntime);
        _jsRuntime = jsRuntime;
    }

    /// <summary>Gets a value indicating whether the renderer is ready to draw.</summary>
    public bool IsInitialized => _module is not null && _handle != 0;

    /// <summary>
    /// Imports the Canvas module and binds this renderer to a canvas element.
    /// </summary>
    /// <param name="canvas">A reference to the canvas element to draw on.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that completes when the canvas is ready to draw.</returns>
    public async ValueTask InitializeAsync(ElementReference canvas, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (IsInitialized) return;

        _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", cancellationToken, ModulePath);
        _handle = await _module.InvokeAsync<int>("initialize", cancellationToken, canvas);
    }

    /// <summary>
    /// Draws a scene snapshot using a single batched interop call.
    /// </summary>
    /// <param name="scene">The scene snapshot to draw.</param>
    /// <param name="traySize">The logical dimensions of the tray.</param>
    /// <param name="theme">The theme to draw with, or <see langword="null"/> for the default.</param>
    /// <param name="visualSettings">
    /// The presentation values used to resolve each die's drawing instructions, or
    /// <see langword="null"/> for the defaults.
    /// </param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that completes when the scene has been sent to the renderer.</returns>
    public async ValueTask RenderAsync(
        DiceSceneState scene,
        DiceTraySize traySize,
        DiceTheme? theme = null,
        DiceVisualSettings? visualSettings = null,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(scene);
        ArgumentNullException.ThrowIfNull(traySize);

        if (!IsInitialized) return;

        var payload = RenderScene.FromSnapshot(scene, traySize, theme, visualSettings);
        await _module!.InvokeVoidAsync("render", cancellationToken, _handle, payload);
    }

    /// <summary>
    /// Starts the browser animation scheduler for this canvas. The browser is the only
    /// frame source, and the supplied reference is invoked once per displayed frame.
    /// Starting a loop that is already running replaces the previous one.
    /// </summary>
    /// <param name="callback">
    /// A reference to the .NET object exposing the <c>OnFrameAsync</c> frame callback.
    /// </param>
    /// <param name="maxFrameSeconds">
    /// The largest elapsed time reported to .NET for a single frame. This bounds the
    /// catch-up work produced by a long pause such as a backgrounded tab.
    /// </param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that completes when the scheduler has started.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="callback"/> is <see langword="null"/>.
    /// </exception>
    public async ValueTask StartLoopAsync(
        object callback,
        float maxFrameSeconds,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(callback);

        if (!IsInitialized) return;

        await _module!.InvokeVoidAsync("startLoop", cancellationToken, _handle, callback, maxFrameSeconds);
    }

    /// <summary>
    /// Stops the browser animation scheduler for this canvas. Calling this when no loop
    /// is running has no effect.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that completes when the scheduler has stopped.</returns>
    public async ValueTask StopLoopAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!IsInitialized) return;

        await _module!.InvokeVoidAsync("stopLoop", cancellationToken, _handle);
    }

    /// <summary>
    /// Recomputes the canvas backing buffer and redraws the current scene.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that completes when the canvas has been resized.</returns>
    public async ValueTask ResizeAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!IsInitialized) return;

        await _module!.InvokeVoidAsync("resize", cancellationToken, _handle);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        _disposed = true;

        // Disposal commonly races with circuit teardown or page navigation, so the
        // interop exceptions that signal an unreachable browser are expected here.
        try
        {
            if (_module is not null)
            {
                if (_handle != 0)
                {
                    // Stop scheduling frames before releasing the module so a queued
                    // frame callback cannot run against disposed .NET state.
                    await _module.InvokeVoidAsync("stopLoop", _handle);
                    await _module.InvokeVoidAsync("dispose", _handle);
                }

                await _module.DisposeAsync();
            }
        }
        catch (Exception ex) when (ex is JSDisconnectedException or ObjectDisposedException or OperationCanceledException)
        {
        }
        finally
        {
            _module = null;
            _handle = 0;
        }
    }
}
