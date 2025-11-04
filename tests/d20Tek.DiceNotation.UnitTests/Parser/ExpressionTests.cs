using d20Tek.DiceNotation.Parser;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class ExpressionTests
{
    private readonly static Position _expectedPos = new(1, 2, 3);

    [TestMethod]
    public void NumberExpression_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var expr = new NumberExpression(0, new());

        // act
        var result = expr with { Value = 42, Pos = new(1, 2, 3) };

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(42, result.Value);
        Assert.AreEqual(_expectedPos, result.Pos);
    }

    [TestMethod]
    public void GroupExpression_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var inner = new NumberExpression(4, new());
        var expr = new GroupExpression(null, new());

        // act
        var result = expr with { Inner = inner, Pos = new(1, 2, 3) };

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(inner, result.Inner);
        Assert.AreEqual(_expectedPos, result.Pos);
    }

    [TestMethod]
    public void UnaryExpression_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var operand = new NumberExpression(5, new());
        var expr = new UnaryExpression(UnaryOperator.Positive, null, new());

        // act
        var result = expr with { Operator = UnaryOperator.Negative, Operand = operand, Pos = new(1, 2, 3) };

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(UnaryOperator.Negative, result.Operator);
        Assert.AreEqual(operand, result.Operand);
        Assert.AreEqual(_expectedPos, result.Pos);
    }

    [TestMethod]
    public void BinaryExpression_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var left = new NumberExpression(1, new());
        var right = new NumberExpression(9, new());
        var expr = new BinaryExpression(null, BinaryOperator.Subtract, null, new());

        // act
        var result = expr with { Left = left, Operator = BinaryOperator.Add, Right = right, Pos = new(1, 2, 3) };

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(BinaryOperator.Add, result.Operator);
        Assert.AreEqual(left, result.Left);
        Assert.AreEqual(right, result.Right);
        Assert.AreEqual(_expectedPos, result.Pos);
    }

    [TestMethod]
    public void DiceExpression_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var count = new NumberExpression(1, new());
        var sides = new NumberExpression(20, new());
        var mods = new List<Modifier> { new ExplodingModifier(null, new()) };
        var expr = new DiceNotation.Parser.DiceExpression(null, true, null, [], new());

        // act
        var result = expr with
        {
            CountArg = count,
            HasPercentSides = false,
            SidesArg = sides,
            Modifiers = mods,
            Pos = new(1, 2, 3)
        };

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(count, result.CountArg);
        Assert.AreEqual(sides, result.SidesArg);
        Assert.IsFalse(result.HasPercentSides);
        CollectionAssert.AreEqual(mods, result.Modifiers.ToArray());
        Assert.AreEqual(_expectedPos, result.Pos);
    }

    [TestMethod]
    public void FudgeDiceExpression_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var count = new NumberExpression(1, new());
        var mods = new List<Modifier> { new ExplodingModifier(null, new()) };
        var expr = new FudgeExpression(null, [], new());

        // act
        var result = expr with
        {
            CountArg = count,
            Modifiers = mods,
            Pos = new(1, 2, 3)
        };

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(count, result.CountArg);
        CollectionAssert.AreEqual(mods, result.Modifiers.ToArray());
        Assert.AreEqual(_expectedPos, result.Pos);
    }

    [TestMethod]
    public void ExplodingModifier_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var threshold = new NumberExpression(6, new());
        var expr = new ExplodingModifier(null, new());

        // act
        var result = expr with { ThresholdArg = threshold, Pos = new(1, 2, 3) };

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(threshold, result.ThresholdArg);
        Assert.AreEqual(_expectedPos, result.Pos);
    }

    [TestMethod]
    public void SelectModifier_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var count = new NumberExpression(3, new());
        var expr = new SelectModifier(SelectKind.KeepHigh, null, new());

        // act
        var result = expr with { Kind = SelectKind.KeepHigh, CountArg = count, Pos = new(1, 2, 3) };

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(SelectKind.KeepHigh, result.Kind);
        Assert.AreEqual(count, result.CountArg);
        Assert.AreEqual(_expectedPos, result.Pos);
    }
}
