using d20Tek.DiceNotation.DieRoller;
using D20Tek.DiceNotation.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

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
        Assert.IsNotNull(result);
        Assert.AreEqual("(1d20+2)", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(4, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseParensNumDiceTest()
    {
        // arrange

        // act
        var result = _parser.Parse("(1+3)d8", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("(1+3)d8", result.DiceExpression);
        Assert.HasCount(4, result.Results);
        Assert.AreEqual(8, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseParensSidesTest()
    {
        // arrange

        // act
        var result = _parser.Parse("2d(2x3)+2", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("2d(2x3)+2", result.DiceExpression);
        Assert.HasCount(2, result.Results);
        Assert.AreEqual(6, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseParensChooseTest()
    {
        // arrange

        // act
        var result = _parser.Parse("4d(2x3)k(1+2)", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        AssertHelpers.AssertDiceChoose(result, "4d(2x3)k(1+2)", "DiceTerm.d6", 4, 3);
    }

    [TestMethod]
    public void DiceParser_ParseParensMultiplyTest()
    {
        // arrange

        // act
        var result = _parser.Parse("(2d10+1) * 10", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("(2d10+1)*10", result.DiceExpression);
        Assert.HasCount(2, result.Results);
        Assert.AreEqual(50, result.Value);
    }


    [TestMethod]
    public void DiceParser_ParseParensDivideTest()
    {
        // arrange

        // act
        var result = _parser.Parse("(4d10-2) / (1+1)", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("(4d10-2)/(1+1)", result.DiceExpression);
        Assert.HasCount(4, result.Results);
        Assert.AreEqual(3, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseParensFudgeTest()
    {
        // arrange

        // act
        var result = _parser.Parse("(10f-2) / (1+1)", this._config, this._roller);

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
        Assert.IsNotNull(result);
        Assert.AreEqual("(2+1d20+(2+3))x3-10", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(17, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseParensComplex2Test()
    {
        // arrange

        // act
        var result = _parser.Parse("(2+1d20+(2+3))x3-10+()", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("(2+1d20+(2+3))x3-10+()", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(17, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseParensComplex3Test()
    {
        // arrange

        // act
        var result = _parser.Parse("(2+1d20+(2+3))x3-10+(3)", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("(2+1d20+(2+3))x3-10+(3)", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(20, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseParensComplex4Test()
    {
        // arrange

        // act
        var result = _parser.Parse("(((2+1d20)+(2+3))x3-10+(3))", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("(((2+1d20)+(2+3))x3-10+(3))", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(20, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseParensMismatchEndTest()
    {
        // arrange

        // act
        Assert.ThrowsExactly<ArithmeticException>(
            [ExcludeFromCodeCoverage]() => _parser.Parse("(2+1d20+(2+3)x3-10+(3)", _config, _testRoller));
    }

    [TestMethod]
    public void DiceParser_ParseParensMismatchStartTest()
    {
        // arrange

        // act
        Assert.ThrowsExactly<FormatException>(
            [ExcludeFromCodeCoverage] () => _parser.Parse("(2+1d20+2+3))x3-10+(3)", _config, _testRoller));
    }
}
