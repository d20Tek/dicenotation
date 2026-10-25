namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

using d20Tek.DiceNotation;
using d20Tek.DiceNotation.Results;

[TestClass]
public sealed class VisualDieFactorySummaryTests
{
    private static DiceResult Result(int total, params TermResult[] terms) =>
        new() { DiceExpression = "test", Results = terms, Value = total };

    private static TermResult Dice(int sides, int value) => new(1, value, $"DiceTerm.d{sides}");

    [TestMethod]
    public void CalculateModifier_WithNullResult_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        static void Act() => VisualDieFactory.CalculateModifier(null!);
    }

    [TestMethod]
    public void CalculateModifier_WithNoModifier_ReturnsZero()
    {
        // Arrange
        var result = Result(7, Dice(6, 3), Dice(6, 4));

        // Act
        var modifier = VisualDieFactory.CalculateModifier(result);

        // Assert
        Assert.AreEqual(0, modifier);
    }

    [TestMethod]
    public void CalculateModifier_WithNullResultsCollection_ReturnsTheWholeTotal()
    {
        // Arrange
        // With no dice to account for, the entire total is non-dice value.
        var result = new DiceResult { Results = null!, Value = 4 };

        // Act
        var modifier = VisualDieFactory.CalculateModifier(result);

        // Assert
        Assert.AreEqual(4, modifier);
    }

    [TestMethod]
    public void CalculateModifier_WithNullTermEntry_SkipsThatEntry()
    {
        // Arrange
        var result = Result(5, null!);

        // Act
        var modifier = VisualDieFactory.CalculateModifier(result);

        // Assert
        Assert.AreEqual(5, modifier);
    }

    [TestMethod]
    public void CalculateModifier_WithPositiveModifier_ReturnsThatValue()
    {
        // Arrange
        // The parser folds constants into the total, so the modifier is what remains
        // after the dice are accounted for.
        var result = Result(10, Dice(6, 7));

        // Act
        var modifier = VisualDieFactory.CalculateModifier(result);

        // Assert
        Assert.AreEqual(3, modifier);
    }

    [TestMethod]
    public void CalculateModifier_WithNegativeModifier_ReturnsNegativeValue()
    {
        // Arrange
        var result = Result(1, Dice(6, 3));

        // Act
        var modifier = VisualDieFactory.CalculateModifier(result);

        // Assert
        Assert.AreEqual(-2, modifier);
    }

    [TestMethod]
    public void CalculateModifier_WithSeveralDice_SubtractsEveryDie()
    {
        // Arrange
        var result = Result(14, Dice(6, 4), Dice(6, 2), Dice(6, 3));

        // Act
        var modifier = VisualDieFactory.CalculateModifier(result);

        // Assert
        Assert.AreEqual(5, modifier);
    }

    [TestMethod]
    public void CalculateModifier_WithDroppedDie_ExcludesItFromTheDiceTotal()
    {
        // Arrange
        // A dropped die did not contribute to the total, so it must not be subtracted.
        var dropped = Dice(6, 6);
        dropped.AppliesToResultCalculation = false;
        var result = Result(5, Dice(6, 4), dropped);

        // Act
        var modifier = VisualDieFactory.CalculateModifier(result);

        // Assert
        Assert.AreEqual(1, modifier);
    }

    [TestMethod]
    public void CalculateModifier_WithScaledDiceTerm_AppliesTheScalar()
    {
        // Arrange
        // A negative scalar means the die subtracted from the total.
        var result = Result(-1, new TermResult(-1, 4, "DiceTerm.d6"));

        // Act
        var modifier = VisualDieFactory.CalculateModifier(result);

        // Assert
        Assert.AreEqual(3, modifier);
    }

    [TestMethod]
    public void CreateSummary_WithNullResult_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        static void Act() => VisualDieFactory.CreateSummary(null!);
    }

    [TestMethod]
    public void CreateSummary_WithFailedRoll_ThrowsArgumentException()
    {
        // Arrange
        var result = new DiceResult { Error = "invalid notation" };

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => VisualDieFactory.CreateSummary(result);
    }

    [TestMethod]
    public void CreateSummary_WithRoll_UsesTheLibraryTotal()
    {
        // Arrange
        // The total is taken from the library, never recomputed from the visible faces.
        var result = Result(12, Dice(6, 4), Dice(6, 5));

        // Act
        var summary = VisualDieFactory.CreateSummary(result);

        // Assert
        Assert.AreEqual(12, summary.Total);
        Assert.AreEqual(3, summary.Modifier);
        Assert.HasCount(2, summary.Dice);
    }

    [TestMethod]
    public void CreateSummary_WithRoll_PreservesTheExpression()
    {
        // Arrange
        var result = new DiceResult { DiceExpression = "2d6+3", Results = [Dice(6, 1)], Value = 4 };

        // Act
        var summary = VisualDieFactory.CreateSummary(result);

        // Assert
        Assert.AreEqual("2d6+3", summary.Expression);
    }

    [TestMethod]
    public void Empty_WhenRead_ContainsNoDiceAndZeroTotals()
    {
        // Arrange & Act
        var summary = DiceRollSummary.Empty;

        // Assert
        Assert.IsEmpty(summary.Dice);
        Assert.AreEqual(0, summary.Modifier);
        Assert.AreEqual(0, summary.Total);
        Assert.AreEqual(string.Empty, summary.Expression);
    }

    [TestMethod]
    public void CreateSummary_ForRealRoll_MatchesTheLibraryTotal()
    {
        // Arrange
        // Running through the real engine proves the mapping agrees with the library.
        var dice = new Dice();
        var result = dice.Roll("3d6+2", new SequenceDieRoller(4, 5, 6));

        // Act
        var summary = VisualDieFactory.CreateSummary(result);

        // Assert
        // The library orders term results by value, so compare the set rather than order.
        Assert.AreEqual(result.Value, summary.Total);
        Assert.AreEqual(2, summary.Modifier);
        CollectionAssert.AreEquivalent(new[] { 4, 5, 6 }, summary.Dice.Select(d => d.Value).ToArray());
        Assert.AreEqual(summary.Dice.Sum(d => d.Value) + summary.Modifier, summary.Total);
    }

    [TestMethod]
    public void CreateSummary_ForRealRollWithDroppedDice_AnimatesEveryDie()
    {
        // Arrange
        // 4d6k3 keeps three dice but all four were physically rolled, so all four animate.
        var dice = new Dice();
        var result = dice.Roll("4d6k3", new SequenceDieRoller(2, 3, 4, 5));

        // Act
        var summary = VisualDieFactory.CreateSummary(result);

        // Assert
        Assert.HasCount(4, summary.Dice);
        Assert.AreEqual(result.Value, summary.Total);
        Assert.ContainsSingle(d => d.Roles.HasFlag(DieRollRole.Dropped), summary.Dice);
    }

    [TestMethod]
    public void CreateSummary_ForRealRollWithNegativeModifier_ReportsNegativeModifier()
    {
        // Arrange
        var dice = new Dice();
        var result = dice.Roll("2d8-3", new SequenceDieRoller(7, 8));

        // Act
        var summary = VisualDieFactory.CreateSummary(result);

        // Assert
        Assert.AreEqual(-3, summary.Modifier);
        Assert.AreEqual(result.Value, summary.Total);
    }

    [TestMethod]
    public void CreateSummary_ForRealRoll_ReportsTheCorrectSideCount()
    {
        // Arrange
        var dice = new Dice();
        var result = dice.Roll("2d20", new SequenceDieRoller(11, 17));

        // Act
        var summary = VisualDieFactory.CreateSummary(result);

        // Assert
        Assert.IsTrue(summary.Dice.All(d => d.Sides == 20));
        Assert.IsTrue(summary.Dice.All(d => d.DieType == "d20"));
    }

    [TestMethod]
    public void CreateSummary_ForRealFudgeRoll_ReportsFudgeFaceRange()
    {
        // Arrange
        var dice = new Dice();
        var result = dice.Roll("4f", new SequenceDieRoller(1, 2, 3, 1));

        // Act
        var summary = VisualDieFactory.CreateSummary(result);

        // Assert
        Assert.HasCount(4, summary.Dice);
        Assert.IsTrue(summary.Dice.All(d => d.MinValue == -1));
        Assert.IsTrue(summary.Dice.All(d => d.DieType == "dF"));
        Assert.AreEqual(result.Value, summary.Total);
    }
}
