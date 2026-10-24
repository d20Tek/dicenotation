using d20Tek.DiceNotation.Blazor.Services;

namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public class AetherDiceSimulationEnergyTests
{
    private static AetherDiceSimulation CreateSimulation(DiceSimulationSettings? settings = null) =>
        new(settings, new Random(1234));

    [TestMethod]
    public void GetSnapshot_ImmediatelyAfterStartRoll_ReportsMovingDice()
    {
        // Arrange
        using var simulation = CreateSimulation();

        // Act
        simulation.StartRoll(VisualDiceFactory.Create(4), DiceTraySize.Default);
        var snapshot = simulation.GetSnapshot();

        // Assert
        // Dice are launched with randomized speed and spin, so at least one must register
        // as moving on the very first frame.
        Assert.Contains(d => d.Energy > 0f, snapshot.Dice);
    }

    [TestMethod]
    public void GetSnapshot_ForEveryDie_ReportsEnergyWithinNormalizedRange()
    {
        // Arrange
        using var simulation = CreateSimulation();
        simulation.StartRoll(VisualDiceFactory.Create(10), DiceTraySize.Default);

        // Act
        simulation.Step(1f / 60f);
        var snapshot = simulation.GetSnapshot();

        // Assert
        Assert.IsTrue(snapshot.Dice.All(d => d.Energy is >= 0f and <= 1f));
    }

    [TestMethod]
    public void GetSnapshot_WhenSceneSettles_ReportsEnergyAtOrNearZero()
    {
        // Arrange
        using var simulation = CreateSimulation();
        var settings = new DiceSimulationSettings();
        simulation.StartRoll(VisualDiceFactory.Create(4), DiceTraySize.Default);

        // Act
        while (!simulation.IsSettled)
        {
            simulation.Step(settings.FixedStep);
        }

        var snapshot = simulation.GetSnapshot();

        // Assert
        Assert.IsTrue(snapshot.Dice.All(d => d.Energy < 0.1f));
    }

    [TestMethod]
    public void GetSnapshot_AsDiceSlowDown_ReportsDecreasingEnergy()
    {
        // Arrange
        using var simulation = CreateSimulation();
        var settings = new DiceSimulationSettings();
        simulation.StartRoll(VisualDiceFactory.Create(6), DiceTraySize.Default);
        var initial = simulation.GetSnapshot().Dice.Max(d => d.Energy);

        // Act
        for (var i = 0; i < 120 && !simulation.IsSettled; i++)
        {
            simulation.Step(settings.FixedStep);
        }

        var later = simulation.GetSnapshot().Dice.Max(d => d.Energy);

        // Assert
        Assert.IsLessThan(initial, later, $"Expected energy to decay from {initial} but got {later}.");
    }

    [TestMethod]
    public void GetSnapshot_WithZeroLaunchSettings_ReportsZeroEnergy()
    {
        // Arrange
        // Dividing by a zero launch maximum must not produce infinity or NaN.
        var settings = new DiceSimulationSettings { MaxLaunchSpeed = 0f, MaxLaunchSpin = 0f };
        using var simulation = CreateSimulation(settings);

        // Act
        simulation.StartRoll(VisualDiceFactory.Create(3), DiceTraySize.Default);
        var snapshot = simulation.GetSnapshot();

        // Assert
        Assert.IsTrue(snapshot.Dice.All(d => d.Energy == 0f));
    }

    [TestMethod]
    public void GetSnapshot_WithEmptyScene_ReportsNoDice()
    {
        // Arrange
        using var simulation = CreateSimulation();

        // Act
        var snapshot = simulation.GetSnapshot();

        // Assert
        Assert.IsEmpty(snapshot.Dice);
    }
}
