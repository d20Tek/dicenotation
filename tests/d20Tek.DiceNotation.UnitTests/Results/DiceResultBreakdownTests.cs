using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests.Results;

[TestClass]
public class DiceResultBreakdownTests
{
    private const string _diceTermType = "DiceTerm";
    private const string _rollerType = "RandomDieRoller";
    private readonly MockDiceConfiguration config = new();

    [TestMethod]
    public void KeptResults_WithMixedRoles_ReturnsOnlyKeptDice()
    {
        // Arrange
        var termList = CreateRoleTerms();

        // Act
        var result = new DiceResult("4d6k3", termList, _rollerType, config);

        // Assert
        Assert.HasCount(3, result.KeptResults);
        Assert.IsTrue(result.KeptResults.All(r => r.Roles.HasFlag(DieRollRole.Kept)));
    }

    [TestMethod]
    public void DroppedResults_WithMixedRoles_ReturnsOnlyDroppedDice()
    {
        // Arrange
        var termList = CreateRoleTerms();

        // Act
        var result = new DiceResult("4d6k3", termList, _rollerType, config);

        // Assert
        Assert.HasCount(1, result.DroppedResults);
        Assert.IsTrue(result.DroppedResults.All(r => r.Roles.HasFlag(DieRollRole.Dropped)));
    }

    [TestMethod]
    public void ExplodedResults_WithExplodedDie_ReturnsOnlyExplodedDice()
    {
        // Arrange
        var termList = CreateRoleTerms();

        // Act
        var result = new DiceResult("4d6k3", termList, _rollerType, config);

        // Assert
        Assert.HasCount(1, result.ExplodedResults);
        Assert.IsTrue(result.ExplodedResults.All(r => r.Roles.HasFlag(DieRollRole.Exploded)));
    }

    [TestMethod]
    public void RerolledResults_WithRerolledDie_ReturnsOnlyRerolledDice()
    {
        // Arrange
        List<TermResult> termList =
        [
            new() { Scalar = 1, Type = _diceTermType, Value = 1, Roles = DieRollRole.Rerolled },
            new() { Scalar = 1, Type = _diceTermType, Value = 6, Roles = DieRollRole.Kept },
        ];

        // Act
        var result = new DiceResult("2d6r1", termList, _rollerType, config);

        // Assert
        Assert.HasCount(1, result.RerolledResults);
        Assert.IsTrue(result.RerolledResults.All(r => r.Roles.HasFlag(DieRollRole.Rerolled)));
    }

    [TestMethod]
    public void BreakdownHelpers_WithNullResults_ReturnEmpty()
    {
        // Arrange
        var result = new DiceResult { Results = null! };

        // Act & Assert
        Assert.IsEmpty(result.KeptResults);
        Assert.IsEmpty(result.DroppedResults);
        Assert.IsEmpty(result.ExplodedResults);
        Assert.IsEmpty(result.RerolledResults);
        Assert.IsEmpty(result.VerboseDisplayText);
    }

    [TestMethod]
    public void BreakdownHelpers_IgnoreNonDiceTermResults()
    {
        // Arrange
        List<TermResult> termList =
        [
            new() { Scalar = 1, Type = _diceTermType, Value = 6, Roles = DieRollRole.Kept },
            new() { Scalar = 1, Type = "ConstantTerm", Value = 2, Roles = DieRollRole.Kept },
        ];

        // Act
        var result = new DiceResult("d6+2", termList, _rollerType, config);

        // Assert
        Assert.HasCount(1, result.KeptResults);
    }

    [TestMethod]
    public void VerboseDisplayText_WithPlainDice_ShowsValuesOnly()
    {
        // Arrange
        List<TermResult> termList =
        [
            new() { Scalar = 1, Type = _diceTermType, Value = 4, Roles = DieRollRole.Kept },
            new() { Scalar = 1, Type = _diceTermType, Value = 5, Roles = DieRollRole.Kept },
        ];

        // Act
        var result = new DiceResult("2d6", termList, _rollerType, config);

        // Assert
        Assert.AreEqual("4, 5", result.VerboseDisplayText);
    }

    [TestMethod]
    public void VerboseDisplayText_WithAnnotatedDice_ShowsRoles()
    {
        // Arrange
        var termList = CreateRoleTerms();

        // Act
        var result = new DiceResult("4d6k3", termList, _rollerType, config);

        // Assert
        Assert.AreEqual("6 (exploded), 5, 4, 2 (dropped)", result.VerboseDisplayText);
    }

    [TestMethod]
    public void VerboseDisplayText_WithExplodedAndDroppedDie_ShowsBothRoles()
    {
        // Arrange
        List<TermResult> termList =
        [
            new() { Scalar = 1, Type = _diceTermType, Value = 6, Roles = DieRollRole.Exploded | DieRollRole.Dropped, AppliesToResultCalculation = false },
        ];

        // Act
        var result = new DiceResult("d6", termList, _rollerType, config);

        // Assert
        Assert.AreEqual("6 (exploded, dropped)", result.VerboseDisplayText);
    }

    private static List<TermResult> CreateRoleTerms() =>
    [
        new() { Scalar = 1, Type = _diceTermType, Value = 6, Roles = DieRollRole.Kept | DieRollRole.Exploded },
        new() { Scalar = 1, Type = _diceTermType, Value = 5, Roles = DieRollRole.Kept },
        new() { Scalar = 1, Type = _diceTermType, Value = 4, Roles = DieRollRole.Kept },
        new() { Scalar = 1, Type = _diceTermType, Value = 2, Roles = DieRollRole.Dropped, AppliesToResultCalculation = false },
    ];
}
