using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Parser;
using d20Tek.DiceNotation.Parser.Evaluatorlets;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class EvaluatorTests
{
    private readonly DiceConfiguration _config = new();
    private readonly IDieRoller _testRoller = new ConstantDieRoller(2);

    [TestMethod]
    public void Evaluator_ConstructorTest()
    {
        // arrange

        // act
        Evaluator eval = new();

        // assert
        Assert.IsNotNull(eval);
        Assert.IsInstanceOfType<Evaluator>(eval);
    }

    [TestMethod]
    [DataRow("3d6", 3, 6)]
    [DataRow("d20", 1, 2)]
    [DataRow("2d4+3", 2, 7)]
    [DataRow("d12-2", 1, 1)]
    [DataRow("2d8x10", 2, 40)]
    [DataRow("10*2d8", 2, 40)]
    [DataRow("2+1d20+2+3x3-10", 1, 5)]
    [DataRow("+1+d20", 1, 3)]
    [DataRow("-1+2d6", 2, 3)]
    public void Evaluator_ParseSimpleDiceTest(string expression, int expectedCount, int expectedResult)
    {
        // arrange
        var eval = new Evaluator();

        // act
        var result = eval.Evaluate(expression, _testRoller, _config);

        // assert
        result.AssertResult(expression, expectedCount, expectedResult);
    }

    [TestMethod]
    [DataRow("4d6k3", 4, 3)]
    [DataRow("6d6p2", 6, 4)]
    [DataRow("4d6p1", 4, 3)]
    [DataRow("6d6l2", 6, 2)]
    [DataRow("4d6l1", 4, 1)]
    public void Evaluator_ParseDiceWithChooseTest(string expression, int expectedCount, int expectedResult)
    {
        // arrange
        var eval = new Evaluator();

        // act
        var result = eval.Evaluate(expression, _testRoller, _config);

        // assert
        result.AssertDiceChoose(expression, "DiceTerm.d6", expectedCount, expectedResult);
    }

    [TestMethod]
    [DataRow("6d6!6", "6d6!6", 6, 12)]
    [DataRow("10d6 ! 6", "10d6!6", 10, 20)]
    [DataRow("6d6 ! ", "6d6!", 6, 12)]
    [DataRow("6d6! + 3", "6d6!+3", 6, 15)]
    public void Evaluator_ParseDiceWithExplodeTest(
        string expression,
        string expectedExpression,
        int expectedCount,
        int expectedResult)
    {
        // arrange
        var eval = new Evaluator();

        // act
        var result = eval.Evaluate(expression, _testRoller, _config);

        // assert
        result.AssertResult(expectedExpression, expectedCount, expectedResult);
    }

    [TestMethod]
    [DataRow(" 4  d6 k 3+  2    ", "4d6k3+2", 4, 3, 2)]
    [DataRow("4d6k3 + d8 + 2", "4d6k3+d8+2", 5, 4, 2)]
    [DataRow("2 + 4d6k3 + d8", "2+4d6k3+d8", 5, 4, 2)]
    public void Evaluator_ParseDiceChooseWithWhitepaceTest(
        string inputExpression,
        string expectedExpression,
        int expectedCount,
        int expectedResult,
        int modifier)
    {
        // arrange
        var eval = new Evaluator();

        // act
        var result = eval.Evaluate(inputExpression, _testRoller, _config);

        // assert
        result.AssertDiceChoose(expectedExpression, "DiceTerm", expectedCount, expectedResult, modifier);
    }

    [TestMethod]
    [DataRow("3d10 / 2", "3d10/2", 3, 3)]
    [DataRow("40 / 1d6", "40/1d6", 1, 20)]
    [DataRow("100 - 2d12", "100-2d12", 2, 96)]
    [DataRow("-5 + 4d6", "-5+4d6", 4, 3)]
    [DataRow("6 + d20 - 3", "6+d20-3", 1, 5)]
    [DataRow("d%+5", "d%+5", 1, 7)]
    [DataRow("42", "42", 0, 42)]
    [DataRow("4 + 2", "4+2", 0, 6)]
    [DataRow("4x2", "4x2", 0, 8)]
    [DataRow("4/2", "4/2", 0, 2)]
    public void Evaluator_ParseDiceWithWhitespaceTest(
        string inputExpression,
        string expectedExpression,
        int expectedCount,
        int expectedResult)
    {
        // arrange
        var eval = new Evaluator();

        // act
        var result = eval.Evaluate(inputExpression, _testRoller, _config);

        // assert
        result.AssertResult(expectedExpression, expectedCount, expectedResult);
    }

    [TestMethod]
    [DataRow("f", "f", 1, 1, 0)]
    [DataRow(" 3  f   ", "3f", 3, 3, 0)]
    [DataRow("3f + 1", "3f+1", 3, 3, 1)]
    [DataRow("6fk4", "6fk4", 6, 4, 0)]
    [DataRow("6fp3", "6fp3", 6, 3, 0)]
    [DataRow("6 f l 2", "6fl2", 6, 2, 0)]
    public void Evaluator_ParseFudgeDiceTest(
        string inputExpression,
        string expectedExpression,
        int expectedCount,
        int expectedResult,
        int modifier)
    {
        // arrange
        var eval = new Evaluator();

        // act
        var result = eval.Evaluate(inputExpression, _testRoller, _config);

        // assert
        result.AssertDiceChoose(expectedExpression, "FudgeDiceTerm.dF", expectedCount, expectedResult, modifier);
    }

    [TestMethod]
    [DataRow("(1d20+2)", 1, 4)]
    [DataRow("(1+3)d8", 4, 8)]
    [DataRow("2d(2x3)+2", 2, 6)]
    [DataRow("(2d10+1)*10", 2, 50)]
    [DataRow("(4d10-2)/(1+1)", 4, 3)]
    [DataRow("(10f-2)/(1+1)", 10, 9)]
    [DataRow("(2+1d20+(2+3))x3-10", 1, 17)]
    [DataRow("(2+1d20+(2+3))x3-10+(3)", 1, 20)]
    [DataRow("(((2+1d20)+(2+3))x3-10+(3))", 1, 20)]
    public void Evaluator_EvaluateGroupingTest(string expression, int expectedCount, int expectedResult)
    {
        // arrange
        var eval = new Evaluator();

        // act
        var result = eval.Evaluate(expression, _testRoller, _config);

        // assert
        result.AssertResult(expression, expectedCount, expectedResult);
    }

    [TestMethod]
    public void Evaluator_EvaluateWithParseError()
    {
        // arrange
        var eval = new Evaluator();

        // act
        var result = eval.Evaluate("2d+3", _testRoller, _config);

        // assert
        Assert.AreEqual("2d+3", result.DiceExpression);
        Assert.IsNotEmpty(result.Error);
        Assert.IsTrue(result.HasError);
    }

    [TestMethod]
    public void Evaluator_EvaluateWithEvalError()
    {
        // arrange
        var eval = new Evaluator();

        // act
        var result = eval.Evaluate("2d8+2/0", _testRoller, _config);

        // assert
        Assert.AreEqual("2d8+2/0", result.DiceExpression);
        Assert.IsNotEmpty(result.Error);
        Assert.IsTrue(result.HasError);
    }

    [TestMethod]
    public void ProcessException_WithUnknowException_ReturnErrorDiceResult()
    {
        // arrange

        // act
        var result = Evaluator.ProcessException(new NotImplementedException(), "d20");

        // assert
        Assert.AreEqual("d20", result.DiceExpression);
        Assert.IsNotEmpty(result.Error);
        Assert.IsTrue(result.HasError);
    }

    [TestMethod]
    public void EvalBinary_WithUnknownOperation_ThrowsEvalException()
    {
        // arrange
        var evaluator = new Evaluator();
        var eval = new BinaryEvaluator();
        var expr = new BinaryExpression(
            new NumberExpression(3, new(0)),
            (BinaryOperator)99,
            new NumberExpression(2, new(2)), new(1));

        // act - assert
        Assert.ThrowsExactly<EvalException>([ExcludeFromCodeCoverage] () =>
            eval.Eval(evaluator, _testRoller, [], expr));
    }

    [TestMethod]
    public void EvalInternal_WithUnknownExpression_ThrowsEvalException()
    {
        // arrange
        var unkExpr = new UnknownExpression(new(0, 1, 1));
        var eval = new Evaluator();

        // act - assert
        Assert.ThrowsExactly<EvalException>([ExcludeFromCodeCoverage] () =>
            eval.EvalInternal(unkExpr, _testRoller, []));
    }

    [ExcludeFromCodeCoverage]
    internal record UnknownExpression(Position Pos) : Expression(Pos) { }

    [TestMethod]
    public void EvalDice_WithNullSidesArg_ReturnsSidesDefault()
    {
        // arrange
        var evaluator = new Evaluator();
        var mods = new ModifierEvaluator();
        var expr = new DiceNotation.Parser.DiceExpression(new NumberExpression(2, new(0)), false, null, [], new(1));

        var eval = new DiceEvaluator(mods);

        // act
        var result = eval.Eval(evaluator, new ConstantDieRoller(1), [], expr);

        // assert
        Assert.AreEqual(2, result);
    }
}
