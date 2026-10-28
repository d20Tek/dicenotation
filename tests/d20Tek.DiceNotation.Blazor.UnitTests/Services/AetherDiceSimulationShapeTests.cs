namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public class AetherDiceSimulationShapeTests
{
    private static IReadOnlyList<VisualDie> CreateDice(params int[] sides) =>
        [.. sides.Select((s, i) => new VisualDie($"die-{i}", $"d{s}", 1, s))];

    [TestMethod]
    [DataRow(2)]
    [DataRow(4)]
    [DataRow(6)]
    [DataRow(8)]
    [DataRow(10)]
    [DataRow(12)]
    [DataRow(20)]
    [DataRow(100)]
    public void StartRoll_WithAnyDieType_CreatesASimulatableBody(int sides)
    {
        // Arrange
        using var simulation = new AetherDiceSimulation(random: new Random(42));

        // Act
        simulation.StartRoll(CreateDice(sides), DiceTraySize.Default);

        // Assert
        Assert.HasCount(1, simulation.GetSnapshot().Dice);
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(4)]
    [DataRow(6)]
    [DataRow(20)]
    public void Step_WithAnyDieType_SettlesInsideTheTray(int sides)
    {
        // Arrange
        using var simulation = new AetherDiceSimulation(random: new Random(7));
        simulation.StartRoll(CreateDice(sides, sides, sides), DiceTraySize.Default);

        // Act
        for (var i = 0; i < 1200 && !simulation.IsSettled; i++)
        {
            simulation.Step(1f / 60f);
        }

        // Assert
        Assert.IsTrue(simulation.IsSettled);
        Assert.IsTrue(simulation.AllDiceInsideTray());
    }

    [TestMethod]
    public void Step_WithMixedDieTypes_PreservesEveryAssignedValue()
    {
        // Arrange
        using var simulation = new AetherDiceSimulation(random: new Random(11));
        var dice = CreateDice(4, 6, 8, 10, 12, 20);
        simulation.StartRoll(dice, DiceTraySize.Default);

        // Act
        for (var i = 0; i < 1200 && !simulation.IsSettled; i++)
        {
            simulation.Step(1f / 60f);
        }

        // Assert
        var snapshot = simulation.GetSnapshot();
        Assert.AreSequenceEqual(dice.Select(d => d.Value).ToList(), [.. snapshot.Dice.Select(d => d.Value)]);
    }

    [TestMethod]
    public void GetSnapshot_WithMixedDieTypes_ReportsEachDieSideCount()
    {
        // Arrange
        using var simulation = new AetherDiceSimulation(random: new Random(3));
        simulation.StartRoll(CreateDice(4, 20), DiceTraySize.Default);

        // Act
        var snapshot = simulation.GetSnapshot();

        // Assert
        Assert.AreEqual(4, snapshot.Dice[0].Sides);
        Assert.AreEqual(20, snapshot.Dice[1].Sides);
    }
}
