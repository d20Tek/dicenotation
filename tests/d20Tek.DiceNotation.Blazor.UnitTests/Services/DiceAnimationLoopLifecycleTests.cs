using d20Tek.DiceNotation.Blazor.Interop;
using Microsoft.JSInterop;

namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public class DiceAnimationLoopLifecycleTests
{
    private static async Task<(FakeJSRuntime Runtime, FakeDiceSimulation Simulation, DiceAnimationLoop Loop)>
        CreateAsync(DiceSimulationSettings? settings = null)
    {
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);
        await renderer.InitializeAsync(default, CancellationToken.None);

        var simulation = new FakeDiceSimulation();
        return (runtime, simulation, new DiceAnimationLoop(simulation, renderer, settings));
    }

    [TestMethod]
    public async Task StartAsync_CalledRepeatedly_RestartsWithoutDuplicateCompletion()
    {
        // Arrange
        var (runtime, simulation, loop) = await CreateAsync();
        await using var _ = loop;
        var completions = 0;
        loop.AnimationCompleted += [ExcludeFromCodeCoverage](_) => { completions++; return Task.CompletedTask; };

        // Act
        await loop.StartAsync(VisualDiceFactory.Create(2), DiceTraySize.Default, null, CancellationToken.None);
        await loop.StartAsync(VisualDiceFactory.Create(3), DiceTraySize.Default, null, CancellationToken.None);
        await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, CancellationToken.None);

        // Assert
        Assert.AreEqual(3, simulation.StartRollCount);
        Assert.AreEqual(3, runtime.Module.CountOf("startLoop"));
        Assert.AreEqual(2, runtime.Module.CountOf("stopLoop"));
        Assert.AreEqual(0, completions);
        Assert.IsTrue(loop.IsRunning);
    }

    [TestMethod]
    public async Task StartAsync_WhenSimulationRejectsDice_ThrowsAndLeavesLoopIdle()
    {
        // Arrange
        var (runtime, simulation, loop) = await CreateAsync();
        await using var _ = loop;
        simulation.StartRollException = new ArgumentException("too many dice");

        // Act and Assert
        await Assert.ThrowsExactlyAsync<ArgumentException>(Act);
        Assert.IsFalse(loop.IsRunning);
        Assert.AreEqual(0, runtime.Module.CountOf("startLoop"));

        [ExcludeFromCodeCoverage]
        async Task Act() => await loop.StartAsync(
            VisualDiceFactory.Create(1), DiceTraySize.Default, null, CancellationToken.None);
    }

    [TestMethod]
    public async Task StartAsync_WhenSchedulerFails_RaisesFailureAndStopsRunning()
    {
        // Arrange
        var (runtime, _, loop) = await CreateAsync();
        await using var _ = loop;
        Exception? failure = null;
        loop.AnimationFailed += ex => { failure = ex; return Task.CompletedTask; };
        runtime.Module.InvokeExceptions["startLoop"] = new JSException("no scheduler");

        // Act
        await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, CancellationToken.None);

        // Assert
        Assert.IsNotNull(failure);
        Assert.IsInstanceOfType<JSException>(failure);
        Assert.IsFalse(loop.IsRunning);
    }

    [TestMethod]
    public async Task ResizeAsync_WhenRunning_ResizesAndRedraws()
    {
        // Arrange
        var (runtime, _, loop) = await CreateAsync();
        await using var _ = loop;
        await loop.StartAsync(VisualDiceFactory.Create(2), DiceTraySize.Default, null, CancellationToken.None);
        var renders = runtime.Module.CountOf("render");

        // Act
        await loop.ResizeAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(1, runtime.Module.CountOf("resize"));
        Assert.AreEqual(renders + 1, runtime.Module.CountOf("render"));
        Assert.IsTrue(loop.IsRunning);
    }

    [TestMethod]
    public async Task ResizeAsync_BeforeStart_DrawsEmptyScene()
    {
        // Arrange
        var (runtime, _, loop) = await CreateAsync();
        await using var _ = loop;

        // Act
        await loop.ResizeAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(1, runtime.Module.CountOf("resize"));
        Assert.AreEqual(1, runtime.Module.CountOf("render"));
    }

    [TestMethod]
    public async Task ResizeAsync_AfterDispose_DoesNothing()
    {
        // Arrange
        var (runtime, _, loop) = await CreateAsync();
        await loop.DisposeAsync();

        // Act
        await loop.ResizeAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(0, runtime.Module.CountOf("resize"));
    }

    [TestMethod]
    public async Task ResizeAsync_WhenInteropFails_RaisesFailure()
    {
        // Arrange
        var (runtime, _, loop) = await CreateAsync();
        await using var _ = loop;
        await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, CancellationToken.None);
        Exception? failure = null;
        loop.AnimationFailed += ex => { failure = ex; return Task.CompletedTask; };
        runtime.Module.InvokeExceptions["resize"] = new JSDisconnectedException("gone");

        // Act
        await loop.ResizeAsync(CancellationToken.None);

        // Assert
        Assert.IsNotNull(failure);
        Assert.IsFalse(loop.IsRunning);
    }

    [TestMethod]
    public async Task OnFrameAsync_WithBackgroundTabGap_CompletesOnTimeout()
    {
        // Arrange
        var settings = new DiceSimulationSettings { MaxDurationSeconds = 1f };
        var (_, simulation, loop) = await CreateAsync(settings);
        await using var _ = loop;
        DiceSceneState? completed = null;
        loop.AnimationCompleted += scene => { completed = scene; return Task.CompletedTask; };
        await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, CancellationToken.None);

        // Act
        simulation.ElapsedSeconds = 1f;
        simulation.IsSettled = true;
        await loop.OnFrameAsync(settings.MaxFrameSeconds);

        // Assert
        Assert.IsNotNull(completed);
        Assert.IsTrue(loop.IsTimedOut);
        Assert.IsFalse(loop.IsRunning);
    }

    [TestMethod]
    public async Task IsTimedOut_BeforeMaxDuration_ReturnsFalse()
    {
        // Arrange
        var (_, simulation, loop) = await CreateAsync();
        await using var _ = loop;

        // Act
        simulation.ElapsedSeconds = 0.5f;

        // Assert
        Assert.IsFalse(loop.IsTimedOut);
    }

    [TestMethod]
    [DataRow("js")]
    [DataRow("disconnected")]
    [DataRow("disposed")]
    [DataRow("canceled")]
    public async Task DisposeAsync_WhenInteropIsUnavailable_DoesNotThrow(string exceptionKind)
    {
        // Arrange
        // Disposal races with circuit teardown and navigation, so every interop exception
        // that signals an unreachable browser must be absorbed during disposal.
        var (runtime, simulation, loop) = await CreateAsync();
        await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, CancellationToken.None);
        runtime.Module.InvokeException = CreateInteropException(exceptionKind);

        // Act
        await loop.DisposeAsync();

        // Assert
        Assert.IsFalse(loop.IsRunning);
        Assert.IsTrue(simulation.IsDisposed);
    }

    [TestMethod]
    [DataRow("js")]
    [DataRow("disconnected")]
    [DataRow("disposed")]
    [DataRow("canceled")]
    public async Task DisposeAsync_WhenModuleReleaseFails_DoesNotThrow(string exceptionKind)
    {
        // Arrange
        // The module release happens after the canvas teardown calls, so it is a separate
        // failure point from the module function calls themselves.
        var (runtime, simulation, loop) = await CreateAsync();
        await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, CancellationToken.None);
        runtime.Module.DisposeException = CreateInteropException(exceptionKind);

        // Act
        await loop.DisposeAsync();

        // Assert
        Assert.IsFalse(loop.IsRunning);
        Assert.IsTrue(simulation.IsDisposed);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenJavaScriptThrows_SwallowsTheExceptionInTheLoop()
    {
        // Arrange
        // The renderer absorbs the disconnection exceptions itself, so a plain JSException
        // is the one case that reaches the loop's own disposal guard.
        var (runtime, simulation, loop) = await CreateAsync();
        await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, CancellationToken.None);
        runtime.Module.InvokeExceptions["stopLoop"] = new JSException("module call failed");

        // Act
        await loop.DisposeAsync();

        // Assert
        Assert.IsTrue(simulation.IsDisposed);
        Assert.AreEqual(1, runtime.Module.CountOf("stopLoop"));
    }

    [ExcludeFromCodeCoverage]
    private static Exception CreateInteropException(string kind) => kind switch
    {
        "js" => new JSException("module call failed"),
        "disconnected" => new JSDisconnectedException("circuit gone"),
        "disposed" => new ObjectDisposedException("module"),
        "canceled" => new OperationCanceledException("interop canceled"),
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown interop exception kind."),
    };

    [TestMethod]
    public async Task DisposeAsync_CalledTwice_DisposesOnce()
    {
        // Arrange
        var (_, simulation, loop) = await CreateAsync();
        await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, CancellationToken.None);

        // Act
        await loop.DisposeAsync();
        await loop.DisposeAsync();

        // Assert
        Assert.IsTrue(simulation.IsDisposed);
        Assert.IsFalse(loop.IsRunning);
    }

    [TestMethod]
    public async Task MultipleLoops_OnOnePage_UseIndependentCanvasHandles()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        var firstRenderer = new CanvasRenderer(runtime);
        var secondRenderer = new CanvasRenderer(runtime);
        await firstRenderer.InitializeAsync(default, CancellationToken.None);
        await secondRenderer.InitializeAsync(default, CancellationToken.None);

        await using var first = new DiceAnimationLoop(new FakeDiceSimulation(), firstRenderer);
        await using var second = new DiceAnimationLoop(new FakeDiceSimulation(), secondRenderer);

        // Act
        await first.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, CancellationToken.None);
        await second.StartAsync(VisualDiceFactory.Create(2), DiceTraySize.Default, null, CancellationToken.None);

        // Assert
        var handles = runtime.Module.Calls
            .Where(c => c.Identifier == "startLoop")
            .Select(c => c.Args![0])
            .ToArray();
        Assert.HasCount(2, handles);
        Assert.AreNotEqual(handles[0], handles[1]);
    }

    public TestContext TestContext { get; set; }
}
