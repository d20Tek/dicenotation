namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

using d20Tek.DiceNotation.Results;

[TestClass]
public sealed class VisualDieFactoryDiceTests
{
    private static DiceResult Result(params TermResult[] terms) =>
        new() { DiceExpression = "test", Results = terms, Value = 0 };

    private static TermResult Dice(int sides, int value, DieRollRole roles = DieRollRole.Kept) =>
        new(1, value, $"DiceTerm.d{sides}") { Roles = roles };

    [TestMethod]
    public void CreateDice_WithNullResult_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        static void Act() => VisualDieFactory.CreateDice(null!);
    }

    [TestMethod]
    public void CreateDice_WithNoResults_ReturnsEmptyCollection()
    {
        // Arrange
        var result = Result();

        // Act
        var dice = VisualDieFactory.CreateDice(result);

        // Assert
        Assert.IsEmpty(dice);
    }

    [TestMethod]
    public void CreateDice_WithNullResultsCollection_ReturnsEmptyCollection()
    {
        // Arrange
        var result = new DiceResult { Results = null! };

        // Act
        var dice = VisualDieFactory.CreateDice(result);

        // Assert
        Assert.IsEmpty(dice);
    }

    [TestMethod]
    public void CreateDice_WithNullTermEntry_SkipsThatEntry()
    {
        // Arrange
        var result = Result(Dice(6, 4), null!, Dice(6, 2));

        // Act
        var dice = VisualDieFactory.CreateDice(result);

        // Assert
        Assert.HasCount(2, dice);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(6)]
    [DataRow(8)]
    [DataRow(10)]
    [DataRow(12)]
    [DataRow(20)]
    [DataRow(100)]
    public void CreateDice_ForStandardDie_ParsesSideCountFromTermType(int sides)
    {
        // Arrange
        var result = Result(Dice(sides, 1));

        // Act
        var dice = VisualDieFactory.CreateDice(result);

        // Assert
        Assert.AreEqual(sides, dice[0].Sides);
        Assert.AreEqual(1, dice[0].MinValue);
        Assert.AreEqual($"d{sides}", dice[0].DieType);
    }

    [TestMethod]
    public void CreateDice_ForFudgeDie_UsesNegativeToPositiveFaceRange()
    {
        // Arrange
        // Fudge dice are three-sided but read as -1, 0, and +1.
        var result = Result(new TermResult(1, -1, "FudgeDiceTerm.dF"));

        // Act
        var dice = VisualDieFactory.CreateDice(result);

        // Assert
        Assert.AreEqual(3, dice[0].Sides);
        Assert.AreEqual(-1, dice[0].MinValue);
        Assert.AreEqual("dF", dice[0].DieType);
    }

    [TestMethod]
    [DataRow("DiceTerm.dX")]
    [DataRow("DiceTerm.d0")]
    [DataRow("DiceTerm.d-4")]
    [DataRow("DiceTerm")]
    public void CreateDice_WithUnparsableSideCount_FallsBackToSixSides(string type)
    {
        // Arrange
        // The input is library-produced, so a malformed type must not fail the animation.
        var result = Result(new TermResult(1, 3, type));

        // Act
        var dice = VisualDieFactory.CreateDice(result);

        // Assert
        Assert.HasCount(1, dice);
        Assert.AreEqual(6, dice[0].Sides);
        Assert.AreEqual(1, dice[0].MinValue);
    }

    [TestMethod]
    public void CreateDice_WithConstantTerms_ExcludesThemFromTheDice()
    {
        // Arrange
        var result = Result(Dice(6, 5), new TermResult(1, 3, "ConstantTerm"), Dice(6, 2));

        // Act
        var dice = VisualDieFactory.CreateDice(result);

        // Assert
        Assert.HasCount(2, dice);
        Assert.AreSequenceEqual([5, 2], [.. dice.Select(d => d.Value)]);
    }

    [TestMethod]
    public void CreateDice_ForEachDie_AssignsUniqueSequentialIds()
    {
        // Arrange
        var result = Result(Dice(6, 1), new TermResult(1, 2, "ConstantTerm"), Dice(6, 3), Dice(6, 5));

        // Act
        var dice = VisualDieFactory.CreateDice(result);

        // Assert
        // The constant term must not consume an id, so the ids stay contiguous.
        Assert.AreSequenceEqual(["die-0", "die-1", "die-2"], [.. dice.Select(d => d.Id)]);
    }

    [TestMethod]
    public void CreateDice_ForEachDie_PreservesTheRolledValue()
    {
        // Arrange
        var result = Result(Dice(20, 17), Dice(20, 3));

        // Act
        var dice = VisualDieFactory.CreateDice(result);

        // Assert
        Assert.AreSequenceEqual([17, 3], [.. dice.Select(d => d.Value)]);
    }

    [TestMethod]
    [DataRow(DieRollRole.Kept)]
    [DataRow(DieRollRole.Dropped)]
    [DataRow(DieRollRole.Exploded)]
    [DataRow(DieRollRole.Rerolled)]
    [DataRow(DieRollRole.Kept | DieRollRole.Exploded)]
    [DataRow(DieRollRole.None)]
    public void CreateDice_ForAnyRole_CarriesTheRoleThrough(DieRollRole roles)
    {
        // Arrange
        var result = Result(Dice(6, 4, roles));

        // Act
        var dice = VisualDieFactory.CreateDice(result);

        // Assert
        Assert.AreEqual(roles, dice[0].Roles);
    }

    [TestMethod]
    public void CreateDice_WithDroppedDice_IncludesThemAlongsideKeptDice()
    {
        // Arrange
        // Every die animates, so a 4d6k3 roll still shows all four dice.
        var result = Result(
            Dice(6, 6),
            Dice(6, 5),
            Dice(6, 4),
            Dice(6, 1, DieRollRole.Dropped));

        // Act
        var dice = VisualDieFactory.CreateDice(result);

        // Assert
        Assert.HasCount(4, dice);
        Assert.ContainsSingle(d => d.Roles.HasFlag(DieRollRole.Dropped), dice);
    }
}
