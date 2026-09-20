using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;
using d20Tek.DiceNotation.UnitTests.Fakes;

namespace d20Tek.DiceNotation.UnitTests.DiceTerms;

[TestClass]
public class DiceTermRolesTests
{
    [TestMethod]
    public void CalculateResults_WithSimpleDice_MarksAllKept()
    {
        // Arrange
        var term = new DiceTerm(3, 6);

        // Act
        var results = term.CalculateResults(new ConstantDieRoller(4));

        // Assert
        Assert.IsTrue(results.All(r => r.Roles.HasFlag(DieRollRole.Kept)));
        Assert.IsTrue(results.All(r => !r.Roles.HasFlag(DieRollRole.Dropped)));
    }

    [TestMethod]
    public void CalculateResults_WithChoose_MarksDroppedDice()
    {
        // Arrange
        var term = new DiceTerm(5, 6, choose: 3);

        // Act
        var results = term.CalculateResults(new ConstantDieRoller(4));

        // Assert
        Assert.HasCount(3, results.Where(r => r.Roles.HasFlag(DieRollRole.Kept)).ToList());
        Assert.HasCount(2, results.Where(r => r.Roles.HasFlag(DieRollRole.Dropped)).ToList());
        Assert.IsTrue(results.Where(r => r.Roles.HasFlag(DieRollRole.Dropped)).All(r => !r.AppliesToResultCalculation));
    }

    [TestMethod]
    public void CalculateResults_WithExplodingThresholdMet_MarksExplodedDice()
    {
        // Arrange
        var term = new DiceTerm(1, 6, exploding: 6);

        // Act
        var results = term.CalculateResults(new SequenceDieRoller(6, 3));

        // Assert
        Assert.Contains(r => r.Roles.HasFlag(DieRollRole.Exploded), results);
        Assert.IsTrue(results.Where(r => r.Value == 6).All(r => r.Roles.HasFlag(DieRollRole.Exploded)));
        Assert.IsTrue(results.Where(r => r.Value == 3).All(r => !r.Roles.HasFlag(DieRollRole.Exploded)));
    }

    [TestMethod]
    public void CalculateResults_WithExplodingThresholdNotMet_DoesNotMarkExploded()
    {
        // Arrange
        var term = new DiceTerm(3, 6, exploding: 6);

        // Act
        var results = term.CalculateResults(new ConstantDieRoller(4));

        // Assert
        Assert.IsTrue(results.All(r => !r.Roles.HasFlag(DieRollRole.Exploded)));
    }
}
