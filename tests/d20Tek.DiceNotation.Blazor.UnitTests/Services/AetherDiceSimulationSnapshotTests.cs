namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public class AetherDiceSimulationSnapshotTests
{
    private static AetherDiceSimulation CreateSimulation(DiceSimulationSettings? settings = null) =>
        new(settings, new Random(99));

    [TestMethod]
    public void GetSnapshot_BeforeStartRoll_ReturnsEmptyScene()
    {
        // Arrange
        using var simulation = CreateSimulation();

        // Act
        var snapshot = simulation.GetSnapshot();

        // Assert
        Assert.IsEmpty(snapshot.Dice);
        Assert.IsFalse(snapshot.IsSettled);
    }

    [TestMethod]
    public void GetSnapshot_AfterSteps_ContainsOnlyFiniteValues()
    {
        // Arrange
        var settings = new DiceSimulationSettings();
        using var simulation = CreateSimulation(settings);
        simulation.StartRoll(VisualDiceFactory.Create(6), DiceTraySize.Default);
        for (var i = 0; i < 60; i++)
        {
            simulation.Step(settings.FixedStep);
        }

        // Act
        var snapshot = simulation.GetSnapshot();

        // Assert
        foreach (var die in snapshot.Dice)
        {
            Assert.IsTrue(float.IsFinite(die.X));
            Assert.IsTrue(float.IsFinite(die.Y));
            Assert.IsTrue(float.IsFinite(die.Rotation));
            Assert.IsTrue(float.IsFinite(die.Width));
            Assert.IsTrue(float.IsFinite(die.Height));
        }
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void GetSnapshot_AfterStartRoll_ReportsConfiguredDieDimensions()
    {
        // Arrange
        var settings = new DiceSimulationSettings { DieSize = 0.9f };
        using var simulation = CreateSimulation(settings);
        simulation.StartRoll(VisualDiceFactory.Create(2), DiceTraySize.Default);

        // Act
        var snapshot = simulation.GetSnapshot();

        // Assert
        Assert.IsTrue(snapshot.Dice.All(d => d.Width == 0.9f && d.Height == 0.9f));
    }

    [TestMethod]
    public void GetSnapshot_AfterSteps_ReportsElapsedTime()
    {
        // Arrange
        var settings = new DiceSimulationSettings();
        using var simulation = CreateSimulation(settings);
        simulation.StartRoll(VisualDiceFactory.Create(2), DiceTraySize.Default);
        simulation.Step(settings.FixedStep * 2f);

        // Act
        var snapshot = simulation.GetSnapshot();

        // Assert
        Assert.AreEqual(simulation.ElapsedSeconds, snapshot.ElapsedSeconds);
    }

    [TestMethod]
    public void GetSnapshot_CalledTwice_ReturnsIndependentInstances()
    {
        // Arrange
        var settings = new DiceSimulationSettings();
        using var simulation = CreateSimulation(settings);
        simulation.StartRoll(VisualDiceFactory.Create(3), DiceTraySize.Default);
        var first = simulation.GetSnapshot();

        // Act
        simulation.Step(settings.FixedStep);
        var second = simulation.GetSnapshot();

        // Assert
        Assert.AreNotSame(first, second);
        Assert.AreNotSame(first.Dice, second.Dice);
    }
}
