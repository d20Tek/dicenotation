using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.ExtendedDieRollers;

[TestClass]
public class MathNetSeededDieRollerProviderTests
{
    private const int _seed = 2468;

    [TestMethod]
    public void Algorithm_Default_IsMathNetMersenneTwister()
    {
        // Arrange
        var provider = new MathNetSeededDieRollerProvider();

        // Act
        var algorithm = provider.Algorithm;

        // Assert
        Assert.AreEqual(DieRollerAlgorithm.MathNetMersenneTwister, algorithm);
    }

    [TestMethod]
    public void Create_WithSeed_ReturnsSeededMathNetRoller()
    {
        // Arrange
        var provider = new MathNetSeededDieRollerProvider();

        // Act
        var die = provider.Create(_seed);

        // Assert
        Assert.IsInstanceOfType<MathNetDieRoller>(die);
        Assert.AreEqual(new ReplayToken(DieRollerAlgorithm.MathNetMersenneTwister, _seed), die.ReplayToken);
    }

    [TestMethod]
    public void Create_WithTracker_RecordsRollsInTracker()
    {
        // Arrange
        var provider = new MathNetSeededDieRollerProvider();
        var tracker = new AggregateRollTracker();
        var die = provider.Create(_seed, tracker);

        // Act
        die.Roll(6);

        // Assert
        Assert.HasCount(1, tracker.GetFrequencyDataView());
    }
}
