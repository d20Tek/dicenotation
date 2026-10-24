namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public class AetherDiceSimulationStartRollTests
{
    private static AetherDiceSimulation CreateSimulation(DiceSimulationSettings? settings = null) =>
        new(settings, new Random(1234));

    [TestMethod]
    public void Constructor_WithNullSettings_UsesValidatedDefaults()
    {
        // Arrange
        // Act
        using var simulation = new AetherDiceSimulation();

        // Assert
        Assert.AreEqual(0f, simulation.ElapsedSeconds);
        Assert.IsFalse(simulation.IsSettled);
    }

    [TestMethod]
    public void Constructor_WithInvalidSettings_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { FixedStep = 0f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => _ = new AetherDiceSimulation(settings);
    }

    [TestMethod]
    public void StartRoll_WithDice_CreatesOneStateForEachDie()
    {
        // Arrange
        using var simulation = CreateSimulation();
        var dice = VisualDiceFactory.Create(4);

        // Act
        simulation.StartRoll(dice, DiceTraySize.Default);

        // Assert
        Assert.HasCount(4, simulation.GetSnapshot().Dice);
    }

    [TestMethod]
    public void StartRoll_WithDice_RetainsAssignedIdsAndValues()
    {
        // Arrange
        using var simulation = CreateSimulation();
        var dice = VisualDiceFactory.CreateFromValues(1, 3, 5, 6);

        // Act
        simulation.StartRoll(dice, DiceTraySize.Default);

        // Assert
        var snapshot = simulation.GetSnapshot();
        Assert.AreSequenceEqual([.. dice.Select(d => d.Id)], [.. snapshot.Dice.Select(d => d.Id)]);
        Assert.AreSequenceEqual([.. dice.Select(d => d.Value)], [.. snapshot.Dice.Select(d => d.Value)]);
    }

    [TestMethod]
    public void StartRoll_WithDice_PlacesEveryDieInsideTray()
    {
        // Arrange
        using var simulation = CreateSimulation();
        var dice = VisualDiceFactory.Create(10);

        // Act
        simulation.StartRoll(dice, DiceTraySize.Default);

        // Assert
        Assert.IsTrue(simulation.AllDiceInsideTray());
    }

    [TestMethod]
    public void StartRoll_CalledTwice_ResetsElapsedTimeAndDice()
    {
        // Arrange
        using var simulation = CreateSimulation();
        simulation.StartRoll(VisualDiceFactory.Create(4), DiceTraySize.Default);
        simulation.Step(0.5f);

        // Act
        simulation.StartRoll(VisualDiceFactory.Create(2), DiceTraySize.Default);

        // Assert
        Assert.AreEqual(0f, simulation.ElapsedSeconds);
        Assert.HasCount(2, simulation.GetSnapshot().Dice);
    }

    [TestMethod]
    public void StartRoll_WithNullDice_ThrowsArgumentNullException()
    {
        // Arrange
        using var simulation = CreateSimulation();

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => simulation.StartRoll(null!, DiceTraySize.Default);
    }

    [TestMethod]
    public void StartRoll_WithNullTraySize_ThrowsArgumentNullException()
    {
        // Arrange
        using var simulation = CreateSimulation();

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => simulation.StartRoll(VisualDiceFactory.Create(1), null!);
    }

    [TestMethod]
    public void StartRoll_WithEmptyDice_ThrowsArgumentException()
    {
        // Arrange
        using var simulation = CreateSimulation();

        // Act and Assert
        Assert.ThrowsExactly<ArgumentException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => simulation.StartRoll([], DiceTraySize.Default);
    }

    [TestMethod]
    public void StartRoll_WithTooManyDice_ThrowsArgumentException()
    {
        // Arrange
        using var simulation = CreateSimulation(new DiceSimulationSettings { MaxDiceCount = 3 });

        // Act and Assert
        Assert.ThrowsExactly<ArgumentException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => simulation.StartRoll(VisualDiceFactory.Create(4), DiceTraySize.Default);
    }

    [TestMethod]
    public void StartRoll_WithDuplicateIds_ThrowsArgumentException()
    {
        // Arrange
        using var simulation = CreateSimulation();
        var dice = new[]
        {
            new VisualDie("die-0", "d6", 1),
            new VisualDie("die-0", "d6", 2),
        };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => simulation.StartRoll(dice, DiceTraySize.Default);
    }

    [TestMethod]
    public void StartRoll_WithNullDieEntry_ThrowsArgumentException()
    {
        // Arrange
        using var simulation = CreateSimulation();
        var dice = new VisualDie[] { null! };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => simulation.StartRoll(dice, DiceTraySize.Default);
    }

    [TestMethod]
    public void StartRoll_WithInvalidTraySize_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        using var simulation = CreateSimulation();

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => simulation.StartRoll(VisualDiceFactory.Create(1), new DiceTraySize(0f, 5f));
    }
}
