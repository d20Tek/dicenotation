using d20Tek.DiceNotation.Parser;
using DiceExpr = d20Tek.DiceNotation.Parser.DiceExpression;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class ExpressionsTests
{
    [TestMethod]
    public void NumberExpression_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var expression = new NumberExpression(1);

        // act
        var next = expression with { Value = 7 };

        // assert
        Assert.AreNotEqual(expression, next);
        Assert.AreEqual(7, next.Value);
    }

    [TestMethod]
    public void DiceExpression_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var expression = new DiceExpr(1, 4, null, null);

        // act
        var next = expression with { Count = 4, Sides = 6, Keep = 3, Explode = 6 };

        // assert
        Assert.AreNotEqual(expression, next);
        Assert.AreEqual(4, next.Count);
        Assert.AreEqual(6, next.Sides);
        Assert.AreEqual(3, next.Keep);
        Assert.AreEqual(6, next.Explode);
    }

    [TestMethod]
    public void BinaryExpression_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var left = new DiceExpr(1, 20, null, null);
        var right = new NumberExpression(3);
        var expression = new BinaryExpression(null, "", null);

        // act
        var next = expression with { Left = left, Operator = "+", Right = right };

        // assert
        Assert.AreNotEqual(expression, next);
        Assert.AreEqual(left, next.Left);
        Assert.AreEqual("+", next.Operator);
        Assert.AreEqual(right, next.Right);
    }

    [TestMethod]
    public void UnaryExpression_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var right = new NumberExpression(3);
        var expression = new UnaryExpression("", null);

        // act
        var next = expression with { Operator = "-", Operand = right };

        // assert
        Assert.AreNotEqual(expression, next);
        Assert.AreEqual("-", next.Operator);
        Assert.AreEqual(right, next.Operand);
    }

    [TestMethod]
    public void GroupExpression_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var inner = new DiceExpr(1, 20, null, null);
        var expression = new GroupExpression(null);

        // act
        var next = expression with { Inner = inner };

        // assert
        Assert.AreNotEqual(expression, next);
        Assert.AreEqual(inner, next.Inner);
    }
}
