using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceTests_RollMany
{
    private const string _rollerType = "RandomDieRoller";

    [TestMethod]
    public void RollMany_WithNotation_ReturnsRequestedCount()
    {
        // Arrange
        var dice = new Dice();

        // Act
        var results = dice.RollMany("1d20", 5, new ConstantDieRoller(4));

        // Assert
        Assert.HasCount(5, results);
        foreach (var result in results)
        {
            Assert.AreEqual(4, result.Value);
            Assert.Contains("ConstantDieRoller", result.DieRollerUsed);
        }
    }

    [TestMethod]
    public void RollMany_WithExpression_ReturnsRequestedCount()
    {
        // Arrange
        var dice = new Dice();
        var expression = DiceExpression.Create().AddDice(6, 2);

        // Act
        var results = dice.RollMany(expression, 3, new ConstantDieRoller(2));

        // Assert
        Assert.HasCount(3, results);
        foreach (var result in results)
        {
            Assert.AreEqual(4, result.Value);
            Assert.Contains("ConstantDieRoller", result.DieRollerUsed);
        }
    }

    [TestMethod]
    public void RollMany_WithNotation_AndNullRoller_UsesDefaultRoller()
    {
        // Arrange
        var dice = new Dice();

        // Act
        var results = dice.RollMany("1d20", 4, null);

        // Assert
        Assert.HasCount(4, results);
        foreach (var result in results)
        {
            Assert.Contains(_rollerType, result.DieRollerUsed);
            AssertHelpers.IsWithinRangeInclusive(1, 20, result.Value);
        }
    }

    [TestMethod]
    public void RollMany_WithExpression_AndNullRoller_UsesDefaultRoller()
    {
        // Arrange
        var dice = new Dice();
        var expression = DiceExpression.Create().AddDice(20);

        // Act
        var results = dice.RollMany(expression, 4, null);

        // Assert
        Assert.HasCount(4, results);
        foreach (var result in results)
        {
            Assert.Contains(_rollerType, result.DieRollerUsed);
            AssertHelpers.IsWithinRangeInclusive(1, 20, result.Value);
        }
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void RollMany_WithNotation_AndCountLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var dice = new Dice();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => dice.RollMany("1d20", 0));
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void RollMany_WithExpression_AndCountLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var dice = new Dice();
        var expression = DiceExpression.Create().AddDice(20);

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => dice.RollMany(expression, -1));
    }
}
