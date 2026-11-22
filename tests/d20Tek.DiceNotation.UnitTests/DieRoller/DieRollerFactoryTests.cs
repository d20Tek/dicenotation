using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.UnitTests.Fakes;

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
    public void CreateSeeded_WithMathNetProvider_ReturnsMathNetRoller()
    {
        // Arrange
        ISeededDieRollerProvider[] providers = [new MathNetSeededDieRollerProvider()];

        // Act
        var die = DieRollerFactory.CreateSeeded(_seed, DieRollerAlgorithm.MathNetMersenneTwister, null, providers);

        // Assert
        Assert.IsInstanceOfType<MathNetDieRoller>(die);
        Assert.AreEqual(DieRollerAlgorithm.MathNetMersenneTwister, die.ReplayToken!.Algorithm);
    }

    [TestMethod]
    public void CreateSeeded_WithMathNetAlgorithmAndNoProvider_ThrowsWithPackageHint()
    {
        // Arrange
        // Act / Assert
        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);
        Assert.Contains("d20tek-dicenotation-mathnet", ex.Message);

        [ExcludeFromCodeCoverage]
        static void Act() => DieRollerFactory.CreateSeeded(_seed, DieRollerAlgorithm.MathNetMersenneTwister);
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
    public void CreateFromReplayToken_WithMathNetTokenAndProvider_ReproducesOriginalSequence()
    {
        // Arrange
        var original = new MathNetDieRoller(_seed);
        var expected = RollSequence(original);

        // Act
        var replayed = DieRollerFactory.CreateFromReplayToken(
            original.ReplayToken!, null, [new MathNetSeededDieRollerProvider()]);
        var actual = RollSequence(replayed);

        // Assert
        Assert.AreSequenceEqual(expected, actual);
    }

    [TestMethod]
    public void CreateSeeded_WithProviderForAlgorithm_CreatesRollerThroughProvider()
    {
        // Arrange
        var provider = new FakeSeededDieRollerProvider();

        // Act
        var die = DieRollerFactory.CreateSeeded(_seed, FakeSeededDieRollerProvider.ExternalAlgorithm, null, [provider]);

        // Assert
        Assert.IsNotNull(die);
        Assert.AreEqual(1, provider.CreateCount);
    }

    [TestMethod]
    public void CreateSeeded_WithProviderAndTracker_PassesTrackerToProvider()
    {
        // Arrange
        var provider = new FakeSeededDieRollerProvider();
        var tracker = new DieRollTracker();

        // Act
        DieRollerFactory.CreateSeeded(_seed, FakeSeededDieRollerProvider.ExternalAlgorithm, tracker, [provider]);

        // Assert
        Assert.AreSame(tracker, provider.LastTracker);
    }

    [TestMethod]
    public void CreateSeeded_WithTwoProvidersForAlgorithm_UsesLastProvider()
    {
        // Arrange
        var first = new FakeSeededDieRollerProvider();
        var last = new FakeSeededDieRollerProvider();

        // Act
        DieRollerFactory.CreateSeeded(_seed, FakeSeededDieRollerProvider.ExternalAlgorithm, null, [first, last]);

        // Assert
        Assert.AreEqual(0, first.CreateCount);
        Assert.AreEqual(1, last.CreateCount);
    }

    [TestMethod]
    public void CreateSeeded_WithBuiltInAlgorithm_IgnoresProviders()
    {
        // Arrange
        var provider = new FakeSeededDieRollerProvider(DieRollerAlgorithm.Random);

        // Act
        var die = DieRollerFactory.CreateSeeded(_seed, DieRollerAlgorithm.Random, null, [provider]);

        // Assert
        Assert.IsInstanceOfType<RandomDieRoller>(die);
        Assert.AreEqual(0, provider.CreateCount);
    }

    [TestMethod]
    public void CreateSeeded_WithNoMatchingProvider_ThrowsWithProviderHint()
    {
        // Arrange
        var provider = new FakeSeededDieRollerProvider();

        // Act / Assert
        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);
        Assert.Contains(nameof(ISeededDieRollerProvider), ex.Message);
        Assert.AreEqual(0, provider.CreateCount);

        [ExcludeFromCodeCoverage]
        void Act() => DieRollerFactory.CreateSeeded(_seed, (DieRollerAlgorithm)999, null, [provider]);
    }

    [TestMethod]
    public void CreateSeeded_WithNullProviders_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        static void Act() => DieRollerFactory.CreateSeeded(_seed, DieRollerAlgorithm.Random, null, null!);
    }

    [TestMethod]
    public void CreateFromReplayToken_WithProviderToken_ReproducesOriginalSequence()
    {
        // Arrange
        var provider = new FakeSeededDieRollerProvider();
        var original = provider.Create(_seed);
        var expected = RollSequence(original);
        var token = new ReplayToken(FakeSeededDieRollerProvider.ExternalAlgorithm, _seed);

        // Act
        var replayed = DieRollerFactory.CreateFromReplayToken(token, null, [provider]);
        var actual = RollSequence(replayed);

        // Assert
        Assert.AreSequenceEqual(expected, actual);
    }

    [TestMethod]
    public void CreateFromReplayToken_WithNullTokenAndProviders_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        static void Act() => DieRollerFactory.CreateFromReplayToken(null!, null, []);
    }

    [TestMethod]
    public void CreateFromReplayToken_WithNullProviders_ThrowsArgumentNullException()
    {
        // Arrange
        var token = new ReplayToken(DieRollerAlgorithm.Random, _seed);

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => DieRollerFactory.CreateFromReplayToken(token, null, null!);
    }

    private static List<int> RollSequence(IDieRoller die) =>
        [.. Enumerable.Range(0, _sequenceLength).Select(_ => die.Roll(_sides))];
}
