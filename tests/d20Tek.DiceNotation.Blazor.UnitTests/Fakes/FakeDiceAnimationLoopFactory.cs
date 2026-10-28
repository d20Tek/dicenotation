namespace d20Tek.DiceNotation.Blazor.UnitTests.Fakes;

using d20Tek.DiceNotation.Blazor.Interop;

/// <summary>
/// Creates animation loops backed by a controllable simulation and a renderer wired to a
/// fake JavaScript runtime, so component tests can drive animations without a browser or
/// real physics.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class FakeDiceAnimationLoopFactory : IDiceAnimationLoopFactory
{
    public FakeJSRuntime JSRuntime { get; } = new();

    public FakeDiceSimulation Simulation { get; } = new();

    public int CreateCount { get; private set; }

    public DiceSimulationSettings? LastSettings { get; private set; }

    public CanvasRenderer? Renderer { get; private set; }

    public DiceAnimationLoop? Loop { get; private set; }

    /// <summary>Gets or sets an exception thrown when a loop is requested.</summary>
    public Exception? CreateException { get; set; }

    /// <summary>Gets the module recording the interop calls made by the renderer.</summary>
    public FakeJSObjectReference Module => JSRuntime.Module;

    public (CanvasRenderer Renderer, DiceAnimationLoop Loop) Create(DiceSimulationSettings settings)
    {
        if (CreateException is not null)
        {
            throw CreateException;
        }

        CreateCount++;
        LastSettings = settings;

        Renderer = new CanvasRenderer(JSRuntime);
        Loop = new DiceAnimationLoop(Simulation, Renderer, settings);

        return (Renderer, Loop);
    }

    /// <summary>
    /// Gets a value indicating whether the browser could still invoke a frame, which is
    /// false once the loop has released the object reference it registered.
    /// </summary>
    public bool IsFrameCallbackReachable
    {
        get
        {
            var callback = Module.FrameCallbackOf<DiceAnimationLoop>();
            if (callback is null)
            {
                return false;
            }

            try
            {
                _ = callback.Value;
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Advances the animation by one frame, which stands in for a single
    /// <c>requestAnimationFrame</c> tick from the browser scheduler.
    /// </summary>
    /// <param name="elapsedSeconds">The elapsed time to report for the frame.</param>
    /// <returns>A task that completes when the frame has been processed.</returns>
    public async Task InvokeFrameAsync(double elapsedSeconds = 1d / 60d)
    {
        if (Loop is null)
        {
            return;
        }

        await Loop.OnFrameAsync(elapsedSeconds);
    }

    /// <summary>
    /// Settles the simulation and advances one frame, which drives an animation to its
    /// completion notification.
    /// </summary>
    /// <returns>A task that completes once the animation has finished.</returns>
    public async Task SettleAsync()
    {
        Simulation.SettleAfterSteps = 1;
        await InvokeFrameAsync();
    }
}
