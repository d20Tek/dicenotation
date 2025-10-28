using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceParserTests
{
    private readonly DiceConfiguration _config = new();
    private readonly IDieRoller _testRoller = new ConstantDieRoller(2);
    private readonly DiceParser _parser = new();

    [TestMethod]
    public void DiceParser_ConstructorTest()
    {
        // arrange

        // act
        DiceParser parser = new();

        // assert
        Assert.IsNotNull(parser);
        Assert.IsInstanceOfType<DiceParser>(parser);
    }

    [TestMethod]
    [DataRow("3d6", 3, 6)]
    [DataRow("d20", 1, 2)]
    [DataRow("2d4+3", 2, 7)]
    [DataRow("d12-2", 1, 1)]
    [DataRow("2d8x10", 2, 40)]
    [DataRow("10*2d8", 2, 40)]
    [DataRow("2+1d20+2+3x3-10", 1, 5)]
    [DataRow("d", 1, 2)]
    [DataRow("2d+3", 2, 7)]
    public void DiceParser_ParseSimpleDiceTest(string expression, int expectedCount, int expectedResult)
    {
        // arrange

        // act
        var result = _parser.Parse(expression, _config, _testRoller);

        // assert
        result.AssertResult(expression, expectedCount, expectedResult);
    }

    [TestMethod]
    [DataRow("4d6k3", 4, 3)]
    [DataRow("6d6p2", 6, 4)]
    [DataRow("4d6p1", 4, 3)]
    [DataRow("6d6l2", 6, 2)]
    [DataRow("4d6l1", 4, 1)]
    public void DiceParser_ParseDiceWithChooseTest(string expression, int expectedCount, int expectedResult)
    {
        // arrange

        // act
        var result = _parser.Parse(expression, _config, _testRoller);

        // assert
        result.AssertDiceChoose(expression, "DiceTerm.d6", expectedCount, expectedResult);
    }

    [TestMethod]
    [DataRow(" 4  d6 k 3+  2    ", "4d6k3+2", 4, 3, 2)]
    [DataRow("4d6k3 + d8 + 2", "4d6k3+d8+2", 5, 4, 2)]
    [DataRow("2 + 4d6k3 + d8", "2+4d6k3+d8", 5, 4, 2)]
    public void DiceParser_ParseDiceChooseWithWhitepaceTest(
        string inputExpression,
        string expectedExpression,
        int expectedCount,
        int expectedResult,
        int modifier)
    {
        // arrange

        // act
        var result = _parser.Parse(inputExpression, _config, _testRoller);

        // assert
        result.AssertDiceChoose(expectedExpression, "DiceTerm", expectedCount, expectedResult, modifier);
    }

    [TestMethod]
    [DataRow("3d10 / 2", "3d10/2", 3, 3)]
    [DataRow("40 / 1d6", "40/1d6", 1, 20)]
    [DataRow("100 - 2d12", "100-2d12", 2, 96)]
    [DataRow("-5 + 4d6", "-5+4d6", 4, 3)]
    [DataRow("6 + d20 - 3", "6+d20-3", 1, 5)]
    [DataRow("d%+5", "d100+5", 1, 7)]
    public void DiceParser_ParseDiceWithWhitespaceTest(
        string inputExpression,
        string expectedExpression,
        int expectedCount,
        int expectedResult)
    {
        // arrange

        // act
        var result = _parser.Parse(inputExpression, _config, _testRoller);

        // assert
        result.AssertResult(expectedExpression, expectedCount, expectedResult);
    }

    [TestMethod]
    [DataRow("42", "42", 0, 42)]
    [DataRow("4 + 2", "4+2", 0, 6)]
    [DataRow("4x2", "4x2", 0, 8)]
    [DataRow("4/2", "4/2", 0, 2)]
    public void DiceParser_ParseConstantTests(
        string inputExpression,
        string expectedExpression,
        int expectedCount,
        int expectedResult)
    {
        // arrange

        // act
        var result = _parser.Parse(inputExpression, _config, _testRoller);

        // assert
        result.AssertResult(expectedExpression, expectedCount, expectedResult);
    }
}
