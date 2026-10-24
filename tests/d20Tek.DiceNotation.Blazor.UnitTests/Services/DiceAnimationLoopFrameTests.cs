using d20Tek.DiceNotation.Blazor.Interop;
using Microsoft.JSInterop;

namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public class DiceAnimationLoopFrameTests
{
    private static async Task<(FakeJSRuntime Runtime, FakeDiceSimulation Simulation, DiceAnimationLoop Loop)>
        CreateStartedAsync(FakeDiceSimulation? simulation = null)
    {
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);
        await renderer.InitializeAsync(default, CancellationToken.None);

        simulation ??= new FakeDiceSimulation();
        var loop = new DiceAnimationLoop(simulation, renderer);
        await loop.StartAsync(VisualDiceFactory.Create(2), DiceTraySize.Default, null, CancellationToken.None);

        return (runtime, simulation, loop);
    }

    [TestMethod]
    public async Task OnFrameAsync_WhenRunning_StepsSimulationAndRendersOnce()
    {
        // Arrange
        var (runtime, simulation, loop) = await CreateStartedAsync();
        await using var _ = loop;
        var rendersBefore = runtime.Module.CountOf("render");

        // Act
        await loop.OnFrameAsync(0.016);

        // Assert
        Assert.HasCount(1, simulation.Steps);
        Assert.AreEqual(0.016f, simulation.Steps[0], 0.0001f);
        Assert.AreEqual(rendersBefore + 1, runtime.Module.CountOf("render"));
    }

    [TestMethod]
    public async Task OnFrameAsync_ForManyFrames_SendsOneInteropRenderPerFrame()
    {
        // Arrange
        var (runtime, _, loop) = await CreateStartedAsync();
        await using var _ = loop;
        var rendersBefore = runtime.Module.CountOf("render");

        // Act
        for (var i = 0; i < 10; i++)
        {
            await loop.OnFrameAsync(0.016);
        }

        // Assert
        Assert.AreEqual(rendersBefore + 10, runtime.Module.CountOf("render"));
    }

    [TestMethod]
    public async Task OnFrameAsync_WhenNotRunning_DoesNothing()
    {
        // Arrange
        var (runtime, simulation, loop) = await CreateStartedAsync();
        await using var _ = loop;
        await loop.StopAsync(CancellationToken.None);
        var rendersBefore = runtime.Module.CountOf("render");

        // Act
        await loop.OnFrameAsync(0.016);

        // Assert
        Assert.IsEmpty(simulation.Steps);
        Assert.AreEqual(rendersBefore, runtime.Module.CountOf("render"));
    }

    [TestMethod]
    public async Task OnFrameAsync_AfterDispose_DoesNotThrow()
    {
        // Arrange
        var (_, simulation, loop) = await CreateStartedAsync();
        await loop.DisposeAsync();

        // Act
        await loop.OnFrameAsync(0.016);

        // Assert
        Assert.IsEmpty(simulation.Steps);
    }

    [TestMethod]
    public async Task OnFrameAsync_WhenSceneSettles_RaisesCompletedAndStopsScheduler()
    {
        // Arrange
        var simulation = new FakeDiceSimulation { SettleAfterSteps = 2 };
        var (runtime, _, loop) = await CreateStartedAsync(simulation);
        await using var _ = loop;
        DiceSceneState? completed = null;
        loop.AnimationCompleted += scene =>
        {
            completed = scene;
            return Task.CompletedTask;
        };

        // Act
        await loop.OnFrameAsync(0.016);
        await loop.OnFrameAsync(0.016);

        // Assert
        Assert.IsNotNull(completed);
        Assert.IsTrue(completed.IsSettled);
        Assert.IsFalse(loop.IsRunning);
        Assert.AreEqual(1, runtime.Module.CountOf("stopLoop"));
    }

    [TestMethod]
    public async Task OnFrameAsync_AfterSettling_RaisesCompletedOnlyOnce()
    {
        // Arrange
        var simulation = new FakeDiceSimulation { SettleAfterSteps = 1 };
        var (_, _, loop) = await CreateStartedAsync(simulation);
        await using var _unused = loop;
        var completions = 0;
        loop.AnimationCompleted += _ =>
        {
            completions++;
            return Task.CompletedTask;
        };

        // Act
        await loop.OnFrameAsync(0.016);
        await loop.OnFrameAsync(0.016);
        await loop.OnFrameAsync(0.016);

        // Assert
        Assert.AreEqual(1, completions);
    }

    [TestMethod]
    public async Task OnFrameAsync_WhenSettledWithNoSubscriber_StillStopsScheduler()
    {
        // Arrange
        var simulation = new FakeDiceSimulation { SettleAfterSteps = 1 };
        var (runtime, _, loop) = await CreateStartedAsync(simulation);
        await using var _ = loop;

        // Act
        await loop.OnFrameAsync(0.016);

        // Assert
        Assert.IsFalse(loop.IsRunning);
        Assert.AreEqual(1, runtime.Module.CountOf("stopLoop"));
    }

    [TestMethod]
    public async Task OnFrameAsync_AfterReplacementRoll_RaisesCompletedForTheNewRoll()
    {
        // Arrange
        var simulation = new FakeDiceSimulation { SettleAfterSteps = 1 };
        var (_, _, loop) = await CreateStartedAsync(simulation);
        await using var _unused = loop;
        var completions = 0;
        loop.AnimationCompleted += _ =>
        {
            completions++;
            return Task.CompletedTask;
        };
        await loop.OnFrameAsync(0.016);

        // Act
        await loop.StartAsync(VisualDiceFactory.Create(3), DiceTraySize.Default, null, CancellationToken.None);
        await loop.OnFrameAsync(0.016);

        // Assert
        Assert.AreEqual(2, completions);
    }

    [TestMethod]
    public async Task OnFrameAsync_WhenSimulationThrows_RaisesFailedAndStops()
    {
        // Arrange
        var simulation = new FakeDiceSimulation();
        var (runtime, _, loop) = await CreateStartedAsync(simulation);
        await using var _ = loop;
        simulation.StepException = new InvalidOperationException("physics failed");
        Exception? failure = null;
        loop.AnimationFailed += ex =>
        {
            failure = ex;
            return Task.CompletedTask;
        };

        // Act
        await loop.OnFrameAsync(0.016);

        // Assert
        Assert.IsInstanceOfType<InvalidOperationException>(failure);
        Assert.IsFalse(loop.IsRunning);
        Assert.AreEqual(1, runtime.Module.CountOf("stopLoop"));
    }

    [TestMethod]
    public async Task OnFrameAsync_WhenRenderThrows_RaisesFailedWithoutChangingResults()
    {
        // Arrange
        var (runtime, simulation, loop) = await CreateStartedAsync();
        await using var _ = loop;
        var values = simulation.LastDice.Select(d => d.Value).ToArray();
        runtime.Module.InvokeExceptions["render"] = new JSException("canvas failed");
        Exception? failure = null;
        loop.AnimationFailed += ex =>
        {
            failure = ex;
            return Task.CompletedTask;
        };

        // Act
        await loop.OnFrameAsync(0.016);

        // Assert
        Assert.IsInstanceOfType<JSException>(failure);
        Assert.AreSequenceEqual(values, [.. simulation.LastDice.Select(d => d.Value)]);
    }

    [TestMethod]
    public async Task OnFrameAsync_WhenFailingAndStopLoopAlsoFails_SwallowsTeardownException()
    {
        // Arrange
        var simulation = new FakeDiceSimulation();
        var (runtime, _, loop) = await CreateStartedAsync(simulation);
        await using var _ = loop;
        simulation.StepException = new InvalidOperationException("physics failed");
        runtime.Module.InvokeExceptions["stopLoop"] = new JSDisconnectedException("gone");
        Exception? failure = null;
        loop.AnimationFailed += ex =>
        {
            failure = ex;
            return Task.CompletedTask;
        };

        // Act
        await loop.OnFrameAsync(0.016);

        // Assert
        Assert.IsInstanceOfType<InvalidOperationException>(failure);
        Assert.IsFalse(loop.IsRunning);
    }

    [TestMethod]
    public async Task OnFrameAsync_WhenFailingWithNoSubscriber_DoesNotThrow()
    {
        // Arrange
        var simulation = new FakeDiceSimulation();
        var (_, _, loop) = await CreateStartedAsync(simulation);
        await using var _unused = loop;
        simulation.StepException = new InvalidOperationException("physics failed");

        // Act
        await loop.OnFrameAsync(0.016);

        // Assert
        Assert.IsFalse(loop.IsRunning);
    }

    [TestMethod]
    public async Task FrameCount_BeforeAnyFrame_IsZeroWithNoTimings()
    {
        // Arrange
        var (_, _, loop) = await CreateStartedAsync();
        await using var _unused = loop;

        // Act
        var average = loop.AverageFrameMilliseconds;

        // Assert
        Assert.AreEqual(0, loop.FrameCount);
        Assert.AreEqual(0d, average);
        Assert.AreEqual(0d, loop.WorstFrameMilliseconds);
        Assert.AreEqual(0d, loop.LastFrameMilliseconds);
    }

    [TestMethod]
    public async Task OnFrameAsync_ForSeveralFrames_RecordsFrameDiagnostics()
    {
        // Arrange
        var (_, _, loop) = await CreateStartedAsync();
        await using var _ = loop;

        // Act
        await loop.OnFrameAsync(0.016);
        await loop.OnFrameAsync(0.016);
        await loop.OnFrameAsync(0.016);

        // Assert
        Assert.AreEqual(3, loop.FrameCount);
        Assert.IsGreaterThanOrEqualTo(loop.AverageFrameMilliseconds, loop.WorstFrameMilliseconds);
        Assert.IsGreaterThanOrEqualTo(0d, loop.LastFrameMilliseconds);
    }

    [TestMethod]
    public async Task StartAsync_ForANewRoll_ResetsFrameDiagnostics()
    {
        // Arrange
        var (_, _, loop) = await CreateStartedAsync();
        await using var _ = loop;
        await loop.OnFrameAsync(0.016);
        await loop.OnFrameAsync(0.016);

        // Act
        await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, CancellationToken.None);

        // Assert
        Assert.AreEqual(0, loop.FrameCount);
        Assert.AreEqual(0d, loop.WorstFrameMilliseconds);
        Assert.AreEqual(0d, loop.AverageFrameMilliseconds);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenCalled_DisposesSimulationAndRenderer()
    {
        // Arrange
        var (runtime, simulation, loop) = await CreateStartedAsync();

        // Act
        await loop.DisposeAsync();

        // Assert
        Assert.IsTrue(simulation.IsDisposed);
        Assert.IsTrue(runtime.Module.IsDisposed);
        Assert.IsFalse(loop.IsRunning);
    }

    [TestMethod]
    public async Task DisposeAsync_CalledTwice_DisposesOnce()
    {
        // Arrange
        var (runtime, _, loop) = await CreateStartedAsync();

        // Act
        await loop.DisposeAsync();
        await loop.DisposeAsync();

        // Assert
        Assert.AreEqual(1, runtime.Module.CountOf("dispose"));
    }
}
