using Microsoft.VisualStudio.TestTools.UnitTesting;
using d20Tek.DiceNotation.DieRoller;
using System;
using System.Diagnostics.CodeAnalysis;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
[ExcludeFromCodeCoverage]
public class DiceParserErrorTests
{
    private readonly DiceConfiguration _config = new();
    private readonly IDieRoller _roller = new ConstantDieRoller(2);
    private readonly DiceParser _parser = new();

    [TestMethod]
    public void DiceParser_UnrecognizedOperatorTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("1d20g4", _config, _roller));
    }

    [TestMethod]
    public void DiceParser_ParseDiceDropNumberErrorTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("4d6p4", _config, _roller));
    }

    [TestMethod]
    public void DiceParser_ParseDiceOperatorNoValueTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("2d4x", _config, _roller));
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("2d4/", _config, _roller));
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("2d4k", _config, _roller));
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("2d4l", _config, _roller));
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("2+l2d4", _config, _roller));
    }

    [TestMethod]
    public void DiceParser_ParseDiceOperatorMultipleTimesTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("2d4k1k2", _config, _roller));
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("2d4l1l2", _config, _roller));
    }

    [TestMethod]
    public void DiceParser_ParseRandomStringsTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("eosnddik+9", _config, _roller));
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("2drk4/9", _config, _roller));
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("7y+2d4k4", _config, _roller));
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("7!y+2d4", _config, _roller));
    }

    [TestMethod]
    public void DiceParser_ParseDicePercentilErrorTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("2d6%3", _config, _roller));
    }

    [TestMethod]
    public void DiceParser_ParseDiceFudgeErrorTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("2d6f", _config, _roller));
        Assert.ThrowsExactly<FormatException>(() => _parser.Parse("6fd", _config, _roller));
    }

    [TestMethod]
    public void DiceParser_ParseDiceEmptyNullExpressionTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentNullException>(() => _parser.Parse("", _config, _roller));
        Assert.ThrowsExactly<ArgumentNullException>(() => _parser.Parse(null, _config, _roller));
    }
}
