using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

[TestClass]
public class ReplayableDieRollerTests
{
    private const int _seed = 12345;
    private const int _sides = 20;
    private const int _sequenceLength = 25;

    [TestMethod]
    public void RandomDieRoller_WithSeed_ExposesReplayToken()
    {
        // Arrange

        // Act
        var die = new RandomDieRoller(_seed);

        // Assert
        Assert.IsInstanceOfType<IReplayableDieRoller>(die);
        Assert.IsNotNull(die.ReplayToken);
        Assert.AreEqual(DieRollerAlgorithm.Random, die.ReplayToken.Algorithm);
        Assert.AreEqual(_seed, die.ReplayToken.Seed);
    }

    [TestMethod]
    public void RandomDieRoller_WithExternalGenerator_HasNullReplayToken()
    {
        // Arrange
        var die = new RandomDieRoller(new Random(_seed), null);

        // Act & Assert
        Assert.IsNull(die.ReplayToken);
    }

    [TestMethod]
    public void RandomDieRoller_WithSameSeed_ProducesSameSequence()
    {
        // Arrange
        var first = new RandomDieRoller(_seed);
        var second = new RandomDieRoller(_seed);

        // Act
        var firstSequence = RollSequence(first);
        var secondSequence = RollSequence(second);

        // Assert
        Assert.AreSequenceEqual(firstSequence, secondSequence);
    }

    [TestMethod]
    public void MathNetDieRoller_WithSeed_ExposesReplayToken()
    {
        // Arrange

        // Act
        var die = new MathNetDieRoller(_seed);

        // Assert
        Assert.IsInstanceOfType<IReplayableDieRoller>(die);
        Assert.IsNotNull(die.ReplayToken);
        Assert.AreEqual(DieRollerAlgorithm.MathNetMersenneTwister, die.ReplayToken.Algorithm);
        Assert.AreEqual(_seed, die.ReplayToken.Seed);
    }

    [TestMethod]
    public void MathNetDieRoller_WithExternalSource_HasNullReplayToken()
    {
        // Arrange
        var die = new MathNetDieRoller();

        // Act & Assert
        Assert.IsNull(die.ReplayToken);
    }

    [TestMethod]
    public void MathNetDieRoller_WithSameSeed_ProducesSameSequence()
    {
        // Arrange
        var first = new MathNetDieRoller(_seed);
        var second = new MathNetDieRoller(_seed);

        // Act
        var firstSequence = RollSequence(first);
        var secondSequence = RollSequence(second);

        // Assert
        Assert.AreSequenceEqual(firstSequence, secondSequence);
    }

    private static List<int> RollSequence(IDieRoller die) =>
        [.. Enumerable.Range(0, _sequenceLength).Select(_ => die.Roll(_sides))];
}
