using d20Tek.DiceNotation.Parser;
using Parse = d20Tek.DiceNotation.Parser.DiceParser;
using DiceExpr = d20Tek.DiceNotation.Parser.DiceExpression;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class DiceParserTests
{
    private readonly Lexer _lexer = new();
    private readonly Parse _parser = new();

    [TestMethod]
    public void DiceParser_ConstructorTest()
    {
        // arrange

        // act
        Parse parser = new();

        // assert
        Assert.IsNotNull(parser);
        Assert.IsInstanceOfType<Parse>(parser);
    }

    private static IEnumerable<object[]> SimpleDiceCases =>
    [
        ["3d6", new DiceExpr(3, 6, null, null)],
        ["d20", new DiceExpr(1, 20, null, null)],
        ["2d4+3", new BinaryExpression(new DiceExpr(2, 4, null, null), "+", new NumberExpression(3))],
        ["d12-2", new BinaryExpression(new DiceExpr(1, 12, null, null), "-", new NumberExpression(2))],
        ["2d8x10", new BinaryExpression(new DiceExpr(2, 8, null, null), "x", new NumberExpression(10))],
        ["10*2d8", new BinaryExpression(new NumberExpression(10), "*", new DiceExpr(2, 8, null, null))],
        ["2+1d20+2+3x3-10", new BinaryExpression(new BinaryExpression(new BinaryExpression(new BinaryExpression(new NumberExpression(2), "+", new DiceExpr(1, 20, null, null)), "+", new NumberExpression(2)), "+", new BinaryExpression(new NumberExpression(3), "x", new NumberExpression(3))), "-", new NumberExpression(10))],
        ["d", new DiceExpr(1, 6, null, null)],
        ["2d+3", new BinaryExpression(new DiceExpr(2, 6, null, null), "+", new NumberExpression(3))],
        ["3d10 / 2", new BinaryExpression(new DiceExpr(3, 10, null, null), "/", new NumberExpression(2))],
        ["40 / 1d6", new BinaryExpression(new NumberExpression(40), "/", new DiceExpr(1, 6, null, null))],
        ["100 - 2d12", new BinaryExpression(new NumberExpression(100), "-", new DiceExpr(2, 12, null, null))],
        ["-5 + 4d6", new BinaryExpression(new UnaryExpression("-", new NumberExpression(5)), "+", new DiceExpr(4, 6, null, null))],
        ["6 + d20 - 3", new BinaryExpression(new BinaryExpression(new NumberExpression(6), "+", new DiceExpr(1, 20, null, null)), "-", new NumberExpression(3))],
        ["d%+5", new BinaryExpression(new DiceExpr(1, 100, null, null), "+", new NumberExpression(5))],
    ];

    [TestMethod]
    [DynamicData(nameof(SimpleDiceCases))]
    public void DiceParser_ParseSimpleDice(string notation, object expectedExpression)
    {
        // arrange
        var tokens = _lexer.Tokenize(notation);

        // act
        var result = _parser.ParseExpression(tokens);

        // assert
        Assert.AreEqual(expectedExpression, result);
    }

    private static IEnumerable<object[]> ChooseDiceCases =>
    [
        ["4d6k3", new DiceExpr(4, 6, 3, null)],
        ["6d6p2", new DiceExpr(6, 6, 4, null)],
        ["4d6p1", new DiceExpr(4, 6, 3, null)],
        ["6d6l2", new DiceExpr(6, 6, -2, null)],
        ["4d6l1", new DiceExpr(4, 6, -1, null)],
        [" 4  d6 k 3+  2    ", new BinaryExpression(new DiceExpr(4, 6, 3, null), "+", new NumberExpression(2))],
        ["4d6k3 + d8 + 2", new BinaryExpression(new BinaryExpression(new DiceExpr(4, 6, 3, null), "+", new DiceExpr(1, 8, null, null)), "+", new NumberExpression(2))],
        ["+ 2 + 4d6k3 + d8", new BinaryExpression(new BinaryExpression(new NumberExpression(2), "+", new DiceExpr(4, 6, 3, null)), "+", new DiceExpr(1, 8, null, null))]
    ];

    [TestMethod]
    [DynamicData(nameof(ChooseDiceCases))]
    public void DiceParser_ParseDiceWithChoose(string notation, object expectedExpression)
    {
        // arrange
        var tokens = _lexer.Tokenize(notation);

        // act
        var result = _parser.ParseExpression(tokens);

        // assert
        Assert.AreEqual(expectedExpression, result);
    }

    private static IEnumerable<object[]> ConstantsCases =>
    [
        ["42", new NumberExpression(42)],
        ["4 + 2", new BinaryExpression(new NumberExpression(4), "+", new NumberExpression(2))],
        ["4x2", new BinaryExpression(new NumberExpression(4), "x", new NumberExpression(2))],
        ["4/2", new BinaryExpression(new NumberExpression(4), "/", new NumberExpression(2))]
    ];

    [TestMethod]
    [DynamicData(nameof(ConstantsCases))]
    public void DiceParser_ParseConstants(string notation, object expectedExpression)
    {
        // arrange
        var tokens = _lexer.Tokenize(notation);

        // act
        var result = _parser.ParseExpression(tokens);

        // assert
        Assert.AreEqual(expectedExpression, result);
    }

    private static IEnumerable<object[]> GroupingCases =>
    [
        ["(1d20+2)", new GroupExpression(new BinaryExpression(new DiceExpr(1, 20, null, null), "+", new NumberExpression(2)))],
        //["(1+3)d8", new NumberExpression(4)],
    ];

    [TestMethod]
    [DynamicData(nameof(GroupingCases))]
    public void DiceParser_ParseGroupings(string notation, object expectedExpression)
    {
        // arrange
        var tokens = _lexer.Tokenize(notation);

        // act
        var result = _parser.ParseExpression(tokens);

        // assert
        Assert.AreEqual(expectedExpression, result);
    }
}
