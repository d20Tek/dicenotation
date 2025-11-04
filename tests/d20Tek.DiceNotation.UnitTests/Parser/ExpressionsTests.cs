//using d20Tek.DiceNotation.Parser;
//using DiceExpr = d20Tek.DiceNotation.Parser.DiceExpression;

//namespace d20Tek.DiceNotation.UnitTests.Parser;

//[TestClass]
//public class ExpressionsTests
//{
//    [TestMethod]
//    public void NumberExpression_WithChanges_ReturnsUpdatedRecord()
//    {
//        // arrange
//        var expression = new NumberExpression(1);

//        // act
//        var next = expression with { Value = 7 };

//        // assert
//        Assert.AreNotEqual(expression, next);
//        Assert.AreEqual(7, next.Value);
//    }

//    [TestMethod]
//    public void DiceExpression_WithChanges_ReturnsUpdatedRecord()
//    {
//        // arrange
//        var expression = new DiceExpr(DieKind.Standard, new NumberExpression(1), new NumberExpression(4), null, null);

//        // act
//        var next = expression with
//        {
//            Kind = DieKind.Standard,
//            Count = new NumberExpression(4),
//            Sides = new NumberExpression(6),
//            Keep = new NumberExpression(3),
//            Explode = new NumberExpression(6)
//        };

//        // assert
//        Assert.AreNotEqual(expression, next);
//        Assert.AreEqual(DieKind.Standard, next.Kind);
//        Assert.AreEqual(new NumberExpression(4), next.Count);
//        Assert.AreEqual(new NumberExpression(6), next.Sides);
//        Assert.AreEqual(new NumberExpression(3), next.Keep);
//        Assert.AreEqual(new NumberExpression(6), next.Explode);
//    }

//    [TestMethod]
//    public void BinaryExpression_WithChanges_ReturnsUpdatedRecord()
//    {
//        // arrange
//        var left = new DiceExpr(DieKind.Standard, new NumberExpression(1), new NumberExpression(20), null, null);
//        var right = new NumberExpression(3);
//        var expression = new BinaryExpression(null, "", null);

//        // act
//        var next = expression with { Left = left, Operator = "+", Right = right };

//        // assert
//        Assert.AreNotEqual(expression, next);
//        Assert.AreEqual(left, next.Left);
//        Assert.AreEqual("+", next.Operator);
//        Assert.AreEqual(right, next.Right);
//    }

//    [TestMethod]
//    public void UnaryExpression_WithChanges_ReturnsUpdatedRecord()
//    {
//        // arrange
//        var right = new NumberExpression(3);
//        var expression = new UnaryExpression("", null);

//        // act
//        var next = expression with { Operator = "-", Operand = right };

//        // assert
//        Assert.AreNotEqual(expression, next);
//        Assert.AreEqual("-", next.Operator);
//        Assert.AreEqual(right, next.Operand);
//    }

//    [TestMethod]
//    public void GroupExpression_WithChanges_ReturnsUpdatedRecord()
//    {
//        // arrange
//        var inner = new DiceExpr(DieKind.Standard, new NumberExpression(1), new NumberExpression(20), null, null);
//        var expression = new GroupExpression(null);

//        // act
//        var next = expression with { Inner = inner };

//        // assert
//        Assert.AreNotEqual(expression, next);
//        Assert.AreEqual(inner, next.Inner);
//    }
//}
