namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public class DiceAnimationLoopFactoryTests
{
    [TestMethod]
    public void Constructor_WithNullJSRuntime_ThrowsArgumentNullException()
    {
        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        static void Act() => _ = new DiceAnimationLoopFactory(null!);
    }

    [TestMethod]
    public void Create_WithValidSettings_ReturnsRendererAndLoop()
    {
        // Arrange
        var factory = new DiceAnimationLoopFactory(new FakeJSRuntime());

        // Act
        var (renderer, loop) = factory.Create(new DiceSimulationSettings());

        // Assert
        Assert.IsNotNull(renderer);
        Assert.IsNotNull(loop);
        Assert.IsFalse(loop.IsRunning);
    }

    [TestMethod]
    public void Create_WhenCalledTwice_ReturnsDistinctInstances()
    {
        // Arrange
        var factory = new DiceAnimationLoopFactory(new FakeJSRuntime());
        var settings = new DiceSimulationSettings();

        // Act
        var first = factory.Create(settings);
        var second = factory.Create(settings);

        // Assert
        Assert.AreNotSame(first.Renderer, second.Renderer);
        Assert.AreNotSame(first.Loop, second.Loop);
    }

    [TestMethod]
    public void Create_WithNullSettings_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new DiceAnimationLoopFactory(new FakeJSRuntime());

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => factory.Create(null!);
    }
}
