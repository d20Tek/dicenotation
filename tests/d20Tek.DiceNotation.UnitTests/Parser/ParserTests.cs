using d20Tek.DiceNotation.Parser;
using System.Collections.Immutable;
using DiceExpr = d20Tek.DiceNotation.Parser.DiceExpression;
using Parse = d20Tek.DiceNotation.Parser.Parser;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class ParserTests
{
    private static readonly ImmutableList<Modifier> _emptyModifiers = [];

    [TestMethod]
    public void DiceParser_ConstructorTest()
    {
        // arrange
        var lexer = new Lexer("1d20");

        // act
        Parse parser = new(lexer);

        // assert
        Assert.IsNotNull(parser);
        Assert.IsInstanceOfType<Parse>(parser);
    }

    [ExcludeFromCodeCoverage]
    private static IEnumerable<object[]> SimpleDiceCases =>
    [
        ["3d6", new DiceExpr(new NumberExpression(3, new(0)), false, new NumberExpression(6, new(2)), _emptyModifiers, new(1))],
        ["d20", new DiceExpr(null, false, new NumberExpression(20, new(1)), _emptyModifiers, new(0))],
        ["2d4+3", new BinaryExpression(new DiceExpr(new NumberExpression(2, new(0)), false, new NumberExpression(4, new(2)), _emptyModifiers, new(1)), BinaryOperator.Add, new NumberExpression(3, new(4)), new(3))],
        ["d12-2", new BinaryExpression(new DiceExpr(null, false, new NumberExpression(12, new(1)), _emptyModifiers, new(0)), BinaryOperator.Subtract, new NumberExpression(2, new(4)), new(3))],
        ["2d8x10", new BinaryExpression(new DiceExpr(new NumberExpression(2, new(0)), false, new NumberExpression(8, new(2)), _emptyModifiers, new(1)), BinaryOperator.Multiply, new NumberExpression(10, new(4)), new(3))],
        ["10*2d8", new BinaryExpression(new NumberExpression(10, new(0)), BinaryOperator.Multiply, new DiceExpr(new NumberExpression(2, new(3)), false, new NumberExpression(8, new(5)), _emptyModifiers, new(4)), new(2))],
        ["2+1d20+2+3x3-10", new BinaryExpression(new BinaryExpression(new BinaryExpression(new BinaryExpression(new NumberExpression(2, new(0)), BinaryOperator.Add, new DiceExpr(new NumberExpression(1, new(2)), false, new NumberExpression(20, new(4)), _emptyModifiers, new(3)), new(1)), BinaryOperator.Add, new NumberExpression(2, new(7)), new(6)), BinaryOperator.Add, new BinaryExpression(new NumberExpression(3, new(9)), BinaryOperator.Multiply, new NumberExpression(3, new(11)), new(10)), new(8)), BinaryOperator.Subtract, new NumberExpression(10, new(13)), new(12))],
        ["3d10 / 2", new BinaryExpression(new DiceExpr(new NumberExpression(3, new(0)), false, new NumberExpression(10, new(2)), _emptyModifiers, new(1)), BinaryOperator.Divide, new NumberExpression(2, new(7)), new(5))],
        ["40 / 1d6", new BinaryExpression(new NumberExpression(40, new(0)), BinaryOperator.Divide, new DiceExpr(new NumberExpression(1, new(5)), false, new NumberExpression(6, new(7)), _emptyModifiers, new(6)), new(3))],
        ["100 - 2d12", new BinaryExpression(new NumberExpression(100, new(0)), BinaryOperator.Subtract, new DiceExpr(new NumberExpression(2, new(6)), false, new NumberExpression(12, new(8)), _emptyModifiers, new(7)), new(4))],
        ["-5 + 4d6", new BinaryExpression(new UnaryExpression(UnaryOperator.Negative, new NumberExpression(5, new(1)), new(0)), BinaryOperator.Add, new DiceExpr(new NumberExpression(4, new(5)), false, new NumberExpression(6, new(7)), _emptyModifiers, new(6)), new(3))],
        ["6 + d20 - 3", new BinaryExpression(new BinaryExpression(new NumberExpression(6, new(0)), BinaryOperator.Add, new DiceExpr(null, false, new NumberExpression(20, new(5)), _emptyModifiers, new(4)), new(2)), BinaryOperator.Subtract, new NumberExpression(3, new(10)), new(8))],
        ["d%+5", new BinaryExpression(new DiceExpr(null, true, null, _emptyModifiers, new(0)), BinaryOperator.Add, new NumberExpression(5, new(3)), new(2))],
        ["f", new FudgeExpression(null, _emptyModifiers, new(0))],
        ["3f", new FudgeExpression(new NumberExpression(3, new(0)), _emptyModifiers, new(1))],
        ["3f+1", new BinaryExpression(new FudgeExpression(new NumberExpression(3, new(0)), _emptyModifiers, new(1)), BinaryOperator.Add, new NumberExpression(1, new(3)), new(2))],
    ];

    [TestMethod]
    [DynamicData(nameof(SimpleDiceCases))]
    public void DiceParser_ParseSimpleDice(string notation, object expectedExpression)
    {
        // arrange
        var lexer = new Lexer(notation);
        var parser = new Parse(lexer);

        // act
        var result = parser.ParseExpression();

        // assert
        Assert.AreEqual(expectedExpression.ToString(), result.ToString());
        Assert.AreEqual(expectedExpression, result);
    }

    private static IEnumerable<object[]> ConstantsCases =>
    [
        ["42", new NumberExpression(42, new(0))],
        ["4 + 2", new BinaryExpression(new NumberExpression(4, new(0)), BinaryOperator.Add, new NumberExpression(2, new(4)), new(2))],
        ["4-2", new BinaryExpression(new NumberExpression(4, new(0)), BinaryOperator.Subtract, new NumberExpression(2, new(2)), new(1))],
        ["4x2", new BinaryExpression(new NumberExpression(4, new(0)), BinaryOperator.Multiply, new NumberExpression(2, new(2)), new(1))],
        ["4/2", new BinaryExpression(new NumberExpression(4, new(0)), BinaryOperator.Divide, new NumberExpression(2, new(2)), new(1))]
    ];

    [TestMethod]
    [DynamicData(nameof(ConstantsCases))]
    public void DiceParser_ParseConstants(string notation, object expectedExpression)
    {
        // arrange
        var lexer = new Lexer(notation);
        var parser = new Parse(lexer);

        // act
        var result = parser.ParseExpression();

        // assert
        Assert.AreEqual(expectedExpression.ToString(), result.ToString());
        Assert.AreEqual(expectedExpression, result);
    }

    private static IEnumerable<object[]> GroupingCases =>
    [
        ["(1d20+2)", new GroupExpression(new BinaryExpression(new DiceExpr(new NumberExpression(1, new(1)), false, new NumberExpression(20, new(3)), _emptyModifiers, new(2)), BinaryOperator.Add, new NumberExpression(2, new(6)), new(5)), new(0))],
        ["(1+3)d8", new DiceExpr(new GroupExpression(new BinaryExpression(new NumberExpression(1, new(1)), BinaryOperator.Add, new NumberExpression(3, new(3)), new(2)), new(0)), false, new NumberExpression(8, new(6)), _emptyModifiers, new(5))],
        ["2d(2x3)+2", new BinaryExpression(new DiceExpr(new NumberExpression(2, new(0)), false, new GroupExpression(new BinaryExpression(new NumberExpression(2, new(3)), BinaryOperator.Multiply, new NumberExpression(3, new(5)), new(4)), new(2)), _emptyModifiers, new(1)), BinaryOperator.Add, new NumberExpression(2, new(8)), new(7, 1, 8))],
        ["(2d10+1) * 10", new BinaryExpression(new GroupExpression(new BinaryExpression(new DiceExpr(new NumberExpression(2, new(1)), false, new NumberExpression(10, new(3)), _emptyModifiers, new(2)), BinaryOperator.Add, new NumberExpression(1, new(6)), new(5)), new(0)), BinaryOperator.Multiply, new NumberExpression(10, new(11)), new(9))],
        ["(4d10-2) / (1+1)", new BinaryExpression(new GroupExpression(new BinaryExpression(new DiceExpr(new NumberExpression(4, new(1)), false, new NumberExpression(10, new(3)), _emptyModifiers, new(2)), BinaryOperator.Subtract, new NumberExpression(2, new(6)), new(5)), new(0)), BinaryOperator.Divide, new GroupExpression(new BinaryExpression(new NumberExpression(1, new(12)), BinaryOperator.Add, new NumberExpression(1, new(14)), new(13)), new(11)), new(9))],
        ["(2+1d20+(2+3))x3-10", new BinaryExpression(new BinaryExpression(new GroupExpression(new BinaryExpression(new BinaryExpression(new NumberExpression(2, new(1)), BinaryOperator.Add, new DiceExpr(new NumberExpression(1, new(3)), false, new NumberExpression(20, new(5)), _emptyModifiers, new(4)), new(2)), BinaryOperator.Add, new GroupExpression(new BinaryExpression(new NumberExpression(2, new(9)), BinaryOperator.Add, new NumberExpression(3, new(11)), new(10)), new(8)), new(7)), new(0)), BinaryOperator.Multiply, new NumberExpression(3, new(15)), new(14)), BinaryOperator.Subtract, new NumberExpression(10, new(17)), new(16))],
        ["(2+1d20+(2+3))x3-10+(3)", new BinaryExpression(new BinaryExpression(new BinaryExpression(new GroupExpression(new BinaryExpression(new BinaryExpression(new NumberExpression(2, new(1)), BinaryOperator.Add, new DiceExpr(new NumberExpression(1, new(3)), false, new NumberExpression(20, new(5)), _emptyModifiers, new(4)), new(2)), BinaryOperator.Add, new GroupExpression(new BinaryExpression(new NumberExpression(2, new(9)), BinaryOperator.Add, new NumberExpression(3, new(11)), new(10)), new(8)), new(7)), new(0)), BinaryOperator.Multiply, new NumberExpression(3, new(15)), new(14)), BinaryOperator.Subtract, new NumberExpression(10, new(17)), new(16)), BinaryOperator.Add, new GroupExpression(new NumberExpression(3, new(21)), new(20)), new(19))],
        ["(((2+1d20)+(2+3))x3-10+(3))", new GroupExpression(new BinaryExpression(new BinaryExpression(new BinaryExpression(new GroupExpression(new BinaryExpression(new GroupExpression(new BinaryExpression(new NumberExpression(2, new(3)), BinaryOperator.Add, new DiceExpr(new NumberExpression(1, new(5)), false, new NumberExpression(20, new(7)), _emptyModifiers, new(6)), new(4)), new(2)), BinaryOperator.Add, new GroupExpression(new BinaryExpression(new NumberExpression(2, new(12)), BinaryOperator.Add, new NumberExpression(3, new(14)), new(13)), new(11)), new(10)), new(1)), BinaryOperator.Multiply, new NumberExpression(3, new(18)), new(17)), BinaryOperator.Subtract, new NumberExpression(10, new(20)), new(19)), BinaryOperator.Add, new GroupExpression(new NumberExpression(3, new(24)), new(23)), new(22)), new(0))],
    ];

    [TestMethod]
    [DynamicData(nameof(GroupingCases))]
    public void DiceParser_ParseGroupings(string notation, object expectedExpression)
    {
        // arrange
        var lexer = new Lexer(notation);
        var parser = new Parse(lexer);

        // act
        var result = parser.ParseExpression();

        // assert
        Assert.AreEqual(expectedExpression.ToString(), result.ToString());
        Assert.AreEqual(expectedExpression, result);
    }
}
