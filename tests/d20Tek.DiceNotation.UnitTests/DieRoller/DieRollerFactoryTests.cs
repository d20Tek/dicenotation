using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

[TestClass]
public class DieRollerFactoryTests
{
    private const int _seed = 987654;
    private const int _sides = 12;
    private const int _sequenceLength = 25;

    [TestMethod]
    public void CreateSeeded_WithDefaultAlgorithm_ReturnsRandomRoller()
    {
        // Arrange

        // Act
        var die = DieRollerFactory.CreateSeeded(_seed);

        // Assert
        Assert.IsInstanceOfType<RandomDieRoller>(die);
        Assert.AreEqual(DieRollerAlgorithm.Random, die.ReplayToken!.Algorithm);
    }

    [TestMethod]
    public void CreateSeeded_WithMathNetAlgorithm_ReturnsMathNetRoller()
    {
        // Arrange

        // Act
        var die = DieRollerFactory.CreateSeeded(_seed, DieRollerAlgorithm.MathNetMersenneTwister);

        // Assert
        Assert.IsInstanceOfType<MathNetDieRoller>(die);
        Assert.AreEqual(DieRollerAlgorithm.MathNetMersenneTwister, die.ReplayToken!.Algorithm);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void CreateSeeded_WithUnsupportedAlgorithm_ThrowsArgumentOutOfRangeException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => DieRollerFactory.CreateSeeded(_seed, (DieRollerAlgorithm)999));
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void CreateFromReplayToken_WithNullToken_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(
            () => DieRollerFactory.CreateFromReplayToken(null!));
    }

    [TestMethod]
    public void CreateFromReplayToken_WithRandomToken_ReproducesOriginalSequence()
    {
        // Arrange
        var original = new RandomDieRoller(_seed);
        var expected = RollSequence(original);

        // Act
        var replayed = DieRollerFactory.CreateFromReplayToken(original.ReplayToken!);
        var actual = RollSequence(replayed);

        // Assert
        Assert.AreSequenceEqual(expected, actual);
    }

    [TestMethod]
    public void CreateFromReplayToken_WithMathNetToken_ReproducesOriginalSequence()
    {
        // Arrange
        var original = new MathNetDieRoller(_seed);
        var expected = RollSequence(original);

        // Act
        var replayed = DieRollerFactory.CreateFromReplayToken(original.ReplayToken!);
        var actual = RollSequence(replayed);

        // Assert
        Assert.AreSequenceEqual(expected, actual);
    }

    private static List<int> RollSequence(IDieRoller die) =>
        [.. Enumerable.Range(0, _sequenceLength).Select(_ => die.Roll(_sides))];
}
