using d20Tek.DiceNotation.Parser;
using DiceExpr = d20Tek.DiceNotation.Parser.DiceExpression;
using Parse = d20Tek.DiceNotation.Parser.Parser;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class ParserTestsWithModifiers
{
    private static IEnumerable<object[]> ChooseDiceCases =>
    [
        ["4d6k3", new DiceExpr(new NumberExpression(4, new(0)), false, new NumberExpression(6, new(2)), [new SelectModifier(SelectKind.KeepHigh, new NumberExpression(3, new(4)), new(3))], new(1))],
        ["6d6p2", new DiceExpr(new NumberExpression(6, new(0)), false, new NumberExpression(6, new(2)), [new SelectModifier(SelectKind.DropLow, new NumberExpression(2, new(4)), new(3))], new(1))],
        ["4d6p1", new DiceExpr(new NumberExpression(4, new(0)), false, new NumberExpression(6, new(2)), [new SelectModifier(SelectKind.DropLow, new NumberExpression(1, new(4)), new(3))], new(1))],
        ["6d6l2", new DiceExpr(new NumberExpression(6, new(0)), false, new NumberExpression(6, new(2)), [new SelectModifier(SelectKind.KeepLow, new NumberExpression(2, new(4)), new(3))], new(1))],
        ["4d6l1", new DiceExpr(new NumberExpression(4, new(0)), false, new NumberExpression(6, new(2)), [new SelectModifier(SelectKind.KeepLow, new NumberExpression(1, new(4)), new(3))], new(1))],
        [" 4  d6 k 3    ", new DiceExpr(new NumberExpression(4, new(1)), false, new NumberExpression(6, new(5)), [new SelectModifier(SelectKind.KeepHigh, new NumberExpression(3, new(9)), new(7))], new(4))],
        ["4d(2x3)k(1+2)", new DiceExpr(new NumberExpression(4, new(0)), false, new GroupExpression(new BinaryExpression(new NumberExpression(2, new(3)), BinaryOperator.Multiply, new NumberExpression(3, new(5)), new(4)), new(2)), [new SelectModifier(SelectKind.KeepHigh, new GroupExpression(new BinaryExpression(new NumberExpression(1, new(9)), BinaryOperator.Add, new NumberExpression(2, new(11)), new(10)), new(8)), new(7))], new(1))],
        //["4d6k3+1", new DiceExpr(new NumberExpression(4, new(0)), false, new NumberExpression(6, new(2)), [new SelectModifier(SelectKind.KeepHigh, new NumberExpression(3, new(4)), new(3))], new(1))],
        ["6fk4", new FudgeExpression(new NumberExpression(6, new(0)), [new SelectModifier(SelectKind.KeepHigh, new NumberExpression(4, new(3)), new(2))], new(1))],
        ["6fp3", new FudgeExpression(new NumberExpression(6, new(0)), [new SelectModifier(SelectKind.DropLow, new NumberExpression(3, new(3)), new(2))], new(1))],
    ];

    [TestMethod]
    [DynamicData(nameof(ChooseDiceCases))]
    public void DiceParser_ParseDiceWithChoose(string notation, object expectedExpression)
    {
        // arrange
        var lexer = new Lexer(notation);
        var parser = new Parse(lexer);

        // act
        var result = parser.ParseExpression();

        // assert
        if (result is FudgeExpression expr)
            AssertExpressionsAreEquivalent((FudgeExpression)expectedExpression, expr);
        else
            AssertExpressionsAreEquivalent((DiceExpr)expectedExpression, (DiceExpr)result);
    }

    private static IEnumerable<object[]> ExplodingDiceCases =>
    [
        ["6d6!6", new DiceExpr(new NumberExpression(6, new(0)), false, new NumberExpression(6, new(2)), [new ExplodingModifier(new NumberExpression(6, new(4)), new(3))], new(1))],
        ["10d6!6", new DiceExpr(new NumberExpression(10, new(0)), false, new NumberExpression(6, new(3)), [new ExplodingModifier(new NumberExpression(6, new(5)), new(4))], new(2))],
        ["6d6!", new DiceExpr(new NumberExpression(6, new(0)), false, new NumberExpression(6, new(2)), [new ExplodingModifier(null, new(3))], new(1))],
        ["10d6!", new DiceExpr(new NumberExpression(10, new(0)), false, new NumberExpression(6, new(3)), [new ExplodingModifier(null, new(4))], new(2))],
        //["6d6!+2", new BinaryExpression(new DiceExpr(new NumberExpression(6, new(0)), false, new NumberExpression(6, new(2)), [new ExplodingModifier(null, new(3))], new(1)), BinaryOperator.Add, new NumberExpression(2, new(5)), new(4))],
    ];

    [TestMethod]
    [DynamicData(nameof(ExplodingDiceCases))]
    public void DiceParser_ParseDiceWithExploding(string notation, object expectedExpression)
    {
        // arrange
        var lexer = new Lexer(notation);
        var parser = new Parse(lexer);

        // act
        var result = parser.ParseExpression();

        // assert
        AssertExpressionsAreEquivalent((DiceExpr)expectedExpression, (DiceExpr)result);
    }

    [ExcludeFromCodeCoverage]
    private static void AssertExpressionsAreEquivalent(DiceExpr expected, DiceExpr actual)
    {
        Assert.AreEqual(expected.CountArg, actual.CountArg);
        Assert.AreEqual(expected.SidesArg, actual.SidesArg);
        Assert.AreEqual(expected.HasPercentSides, actual.HasPercentSides);
        Assert.HasCount(expected.Modifiers.Count, actual.Modifiers);
        Assert.IsTrue(expected.Modifiers.Zip(actual.Modifiers).All(p => p.First == p.Second));
    }

    [ExcludeFromCodeCoverage]
    private static void AssertExpressionsAreEquivalent(FudgeExpression expected, FudgeExpression actual)
    {
        Assert.AreEqual(expected.CountArg, actual.CountArg);
        Assert.HasCount(expected.Modifiers.Count, actual.Modifiers);
        Assert.IsTrue(expected.Modifiers.Zip(actual.Modifiers).All(p => p.First == p.Second));
    }
}
