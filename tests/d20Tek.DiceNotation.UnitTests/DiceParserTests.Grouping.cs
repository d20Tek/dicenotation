using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceParserGroupingTests
{
    private readonly DiceConfiguration _config = new();
    private readonly IDieRoller _testRoller = new ConstantDieRoller(2);
    private readonly IDieRoller _roller = new RandomDieRoller();
    private readonly DiceParser _parser = new();

    [TestMethod]
    public void DiceParser_ParseParensSimpleTest()
    {
        // arrange

        // act
        var result = _parser.Parse("(1d20+2)", _config, _testRoller);

        // assert
        result.AssertResult("(1d20+2)", 1, 4);
    }

    [TestMethod]
    public void DiceParser_ParseParensNumDiceTest()
    {
        // arrange

        // act
        var result = _parser.Parse("(1+3)d8", _config, _testRoller);

        // assert
        result.AssertResult("(1+3)d8", 4, 8);
    }

    [TestMethod]
    public void DiceParser_ParseParensSidesTest()
    {
        // arrange

        // act
        var result = _parser.Parse("2d(2x3)+2", _config, _testRoller);

        // assert
        result.AssertResult("2d(2x3)+2", 2, 6);
    }

    [TestMethod]
    public void DiceParser_ParseParensChooseTest()
    {
        // arrange

        // act
        var result = _parser.Parse("4d(2x3)k(1+2)", _config, _testRoller);

        // assert
        result.AssertDiceChoose("4d(2x3)k(1+2)", "DiceTerm.d6", 4, 3);
    }

    [TestMethod]
    public void DiceParser_ParseParensMultiplyTest()
    {
        // arrange

        // act
        var result = _parser.Parse("(2d10+1) * 10", _config, _testRoller);

        // assert
        result.AssertResult("(2d10+1)*10", 2, 50);
    }


    [TestMethod]
    public void DiceParser_ParseParensDivideTest()
    {
        // arrange

        // act
        var result = _parser.Parse("(4d10-2) / (1+1)", _config, _testRoller);

        // assert
        result.AssertResult("(4d10-2)/(1+1)", 4, 3);
    }

    [TestMethod]
    public void DiceParser_ParseParensFudgeTest()
    {
        // arrange

        // act
        var result = _parser.Parse("(10f-2) / (1+1)", _config, _roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("(10f-2)/(1+1)", result.DiceExpression);
        Assert.HasCount(10, result.Results);
        AssertHelpers.IsWithinRangeInclusive(-5, 5, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseParensComplexTest()
    {
        // arrange

        // act
        var result = _parser.Parse("(2+1d20+(2+3))x3-10", _config, _testRoller);

        // assert
        result.AssertResult("(2+1d20+(2+3))x3-10", 1, 17);
    }

    [TestMethod]
    public void DiceParser_ParseParensComplex2Test()
    {
        // arrange

        // act
        var result = _parser.Parse("(2+1d20+(2+3))x3-10+()", _config, _testRoller);

        // assert
        result.AssertResult("(2+1d20+(2+3))x3-10+()", 1, 17);
    }

    [TestMethod]
    public void DiceParser_ParseParensComplex3Test()
    {
        // arrange

        // act
        var result = _parser.Parse("(2+1d20+(2+3))x3-10+(3)", _config, _testRoller);

        // assert
        result.AssertResult("(2+1d20+(2+3))x3-10+(3)", 1, 20);
    }

    [TestMethod]
    public void DiceParser_ParseParensComplex4Test()
    {
        // arrange

        // act
        var result = _parser.Parse("(((2+1d20)+(2+3))x3-10+(3))", _config, _testRoller);

        // assert
        result.AssertResult("(((2+1d20)+(2+3))x3-10+(3))", 1, 20);
    }
}
