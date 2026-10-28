namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public class AetherDiceSimulationDisposeTests
{
    private static AetherDiceSimulation CreateSimulation() => new(null, new Random(7));

    [TestMethod]
    public void Dispose_CalledOnce_ClearsScene()
    {
        // Arrange
        var simulation = CreateSimulation();
        simulation.StartRoll(VisualDiceFactory.Create(3), DiceTraySize.Default);

        // Act
        simulation.Dispose();

        // Assert
        Assert.ThrowsExactly<ObjectDisposedException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => simulation.GetSnapshot();
    }

    [TestMethod]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        // Arrange
        var simulation = CreateSimulation();
        simulation.StartRoll(VisualDiceFactory.Create(2), DiceTraySize.Default);

        // Act
        simulation.Dispose();
        simulation.Dispose();

        // Assert
        Assert.ThrowsExactly<ObjectDisposedException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => simulation.GetSnapshot();
    }

    [TestMethod]
    public void Dispose_WithoutStartRoll_DoesNotThrow()
    {
        // Arrange
        var simulation = CreateSimulation();

        // Act
        simulation.Dispose();

        // Assert
        Assert.ThrowsExactly<ObjectDisposedException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => simulation.GetSnapshot();
    }

    [TestMethod]
    public void StartRoll_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var simulation = CreateSimulation();
        simulation.Dispose();

        // Act and Assert
        Assert.ThrowsExactly<ObjectDisposedException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => simulation.StartRoll(VisualDiceFactory.Create(1), DiceTraySize.Default);
    }

    [TestMethod]
    public void Step_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var simulation = CreateSimulation();
        simulation.Dispose();

        // Act and Assert
        Assert.ThrowsExactly<ObjectDisposedException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => simulation.Step(0.016f);
    }

    [TestMethod]
    public void AllDiceInsideTray_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var simulation = CreateSimulation();
        simulation.Dispose();

        // Act and Assert
        Assert.ThrowsExactly<ObjectDisposedException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => simulation.AllDiceInsideTray();
    }
}
