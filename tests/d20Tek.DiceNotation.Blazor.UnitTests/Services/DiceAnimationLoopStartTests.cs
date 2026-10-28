using d20Tek.DiceNotation.Blazor.Interop;

namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public class DiceAnimationLoopStartTests
{
    private static async Task<(FakeJSRuntime Runtime, FakeDiceSimulation Simulation, DiceAnimationLoop Loop)>
        CreateAsync()
    {
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);
        await renderer.InitializeAsync(default, CancellationToken.None);

        var simulation = new FakeDiceSimulation();
        return (runtime, simulation, new DiceAnimationLoop(simulation, renderer));
    }

    [TestMethod]
    public void Constructor_WithNullSimulation_ThrowsArgumentNullException()
    {
        // Arrange
        var renderer = new CanvasRenderer(new FakeJSRuntime());

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => _ = new DiceAnimationLoop(null!, renderer);
    }

    [TestMethod]
    public void Constructor_WithNullRenderer_ThrowsArgumentNullException()
    {
        // Arrange
        var simulation = new FakeDiceSimulation();

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => _ = new DiceAnimationLoop(simulation, null!);
    }

    [TestMethod]
    public void Constructor_WithInvalidSettings_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var simulation = new FakeDiceSimulation();
        var renderer = new CanvasRenderer(new FakeJSRuntime());
        var settings = new DiceSimulationSettings { MaxFrameSeconds = 0f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => _ = new DiceAnimationLoop(simulation, renderer, settings);
    }

    [TestMethod]
    public async Task IsRunning_BeforeStart_ReturnsFalse()
    {
        // Arrange
        var (_, _, loop) = await CreateAsync();

        // Act
        await using var _unused = loop;

        // Assert
        Assert.IsFalse(loop.IsRunning);
    }

    [TestMethod]
    public async Task StartAsync_WhenCalled_StartsRollAndScheduler()
    {
        // Arrange
        var (runtime, simulation, loop) = await CreateAsync();
        await using var _ = loop;
        var dice = VisualDiceFactory.Create(3);

        // Act
        await loop.StartAsync(dice, DiceTraySize.Default, null, null, CancellationToken.None);

        // Assert
        Assert.AreEqual(1, simulation.StartRollCount);
        Assert.AreEqual(1, runtime.Module.CountOf("startLoop"));
        Assert.IsTrue(loop.IsRunning);
    }

    [TestMethod]
    public async Task StartAsync_WhenCalled_DrawsLaunchFrameBeforeScheduling()
    {
        // Arrange
        var (runtime, _, loop) = await CreateAsync();
        await using var _ = loop;

        // Act
        await loop.StartAsync(VisualDiceFactory.Create(2), DiceTraySize.Default, null, null, CancellationToken.None);

        // Assert
        var identifiers = runtime.Module.Identifiers.ToList();
        Assert.IsLessThan(identifiers.IndexOf("startLoop"), identifiers.IndexOf("render"));
    }

    [TestMethod]
    public async Task StartAsync_WithCustomSettings_PassesConfiguredFrameClamp()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);
        await renderer.InitializeAsync(default, CancellationToken.None);
        var settings = new DiceSimulationSettings { MaxFrameSeconds = 0.25f };
        await using var loop = new DiceAnimationLoop(new FakeDiceSimulation(), renderer, settings);

        // Act
        await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, null, CancellationToken.None);

        // Assert
        Assert.AreEqual(0.25f, runtime.Module.ArgsOf("startLoop")![2]);
    }

    [TestMethod]
    public async Task StartAsync_WithTheme_RendersUsingThatTheme()
    {
        // Arrange
        var (runtime, _, loop) = await CreateAsync();
        await using var _ = loop;

        // Act
        await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, DiceTheme.Parchment, null, CancellationToken.None);

        // Assert
        var payload = (RenderScene)runtime.Module.ArgsOf("render")![1]!;
        Assert.AreEqual(DiceTheme.Parchment.Name, payload.Theme.Name);
    }

    [TestMethod]
    public async Task StartAsync_WithNullDice_ThrowsArgumentNullException()
    {
        // Arrange
        var (_, _, loop) = await CreateAsync();
        await using var _unused = loop;

        // Act and Assert
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        async Task Act() => await loop.StartAsync(null!, DiceTraySize.Default, null, null, CancellationToken.None);
    }

    [TestMethod]
    public async Task StartAsync_WithNullTraySize_ThrowsArgumentNullException()
    {
        // Arrange
        var (_, _, loop) = await CreateAsync();
        await using var _unused = loop;

        // Act and Assert
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        async Task Act() => await loop.StartAsync(VisualDiceFactory.Create(1), null!, null, null, CancellationToken.None);
    }

    [TestMethod]
    public async Task StartAsync_WhileRunning_ReplacesTheActiveAnimation()
    {
        // Arrange
        var (runtime, simulation, loop) = await CreateAsync();
        await using var _ = loop;
        await loop.StartAsync(VisualDiceFactory.Create(2), DiceTraySize.Default, null, null, CancellationToken.None);

        // Act
        await loop.StartAsync(VisualDiceFactory.Create(5), DiceTraySize.Default, null, null, CancellationToken.None);

        // Assert
        Assert.AreEqual(2, simulation.StartRollCount);
        Assert.AreEqual(1, runtime.Module.CountOf("stopLoop"));
        Assert.AreEqual(2, runtime.Module.CountOf("startLoop"));
        Assert.HasCount(5, simulation.LastDice);
    }

    [TestMethod]
    public async Task StartAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var (_, _, loop) = await CreateAsync();
        await loop.DisposeAsync();

        // Act and Assert
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(Act);

        [ExcludeFromCodeCoverage]
        async Task Act() => await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, null, CancellationToken.None);
    }

    [TestMethod]
    public async Task StopAsync_WhenRunning_StopsScheduler()
    {
        // Arrange
        var (runtime, _, loop) = await CreateAsync();
        await using var _ = loop;
        await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, null, CancellationToken.None);

        // Act
        await loop.StopAsync(CancellationToken.None);

        // Assert
        Assert.IsFalse(loop.IsRunning);
        Assert.AreEqual(1, runtime.Module.CountOf("stopLoop"));
    }

    [TestMethod]
    public async Task StopAsync_WhenNotRunning_DoesNothing()
    {
        // Arrange
        var (runtime, _, loop) = await CreateAsync();
        await using var _ = loop;

        // Act
        await loop.StopAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(0, runtime.Module.CountOf("stopLoop"));
    }

    [TestMethod]
    public async Task StopAsync_AfterDispose_DoesNothing()
    {
        // Arrange
        var (runtime, _, loop) = await CreateAsync();
        await loop.StartAsync(VisualDiceFactory.Create(1), DiceTraySize.Default, null, null, CancellationToken.None);
        await loop.DisposeAsync();
        var stopCount = runtime.Module.CountOf("stopLoop");

        // Act
        await loop.StopAsync(CancellationToken.None);

        // Assert
        Assert.IsFalse(loop.IsRunning);
        Assert.AreEqual(stopCount, runtime.Module.CountOf("stopLoop"));
    }

    public TestContext TestContext { get; set; }
}
