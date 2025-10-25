using d20Tek.DiceNotation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace d20Tek.DiceNotation.UnitTests.Results;

[TestClass]
public class TermResultListConverterTests
{
    private readonly TermResultListConverter _conv = new();

    [TestMethod]
    public void TermResultListConverter_ConstructorTest()
    {
        // arrange

        // act
        TermResultListConverter conv = new();

        // assert
        Assert.IsNotNull(conv);
        Assert.IsInstanceOfType<TermResultListConverter>(conv);
    }

    [TestMethod]
    public void TermResultListConverter_ConvertTextTest()
    {
        // arrange
        DiceResult diceResult = new()
        {
            DiceExpression = "d6",
            DieRollerUsed = "ConstantDieRoller",
            Results =
            [
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
            ],
            Value = 3,
        };

        // act
        var result = _conv.Convert(diceResult.Results, typeof(string), null, "en-us") as string;

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("3", result);
    }

    [TestMethod]
    public void TermResultListConverter_ConvertChooseTextTest()
    {
        // arrange
        DiceResult diceResult = new()
        {
            DiceExpression = "6d6k3",
            DieRollerUsed = "ConstantDieRoller",
            Results =
            [
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = false },
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = false },
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = false },
            ],
            Value = 9,
        };

        // act
        var result = _conv.Convert(diceResult.Results, typeof(string), null, "en-us") as string;

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("3, 3, 3, 3*, 3*, 3*", result);
    }

    [TestMethod]
    public void TermResultListConverter_ConvertComplexTextTest()
    {
        // arrange
        DiceResult diceResult = new()
        {
            DiceExpression = "4d6k3+d8+5",
            DieRollerUsed = "ConstantDieRoller",
            Results =
            [
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = false },
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
            ],
            Value = 17,
        };

        // act
        var result = _conv.Convert(diceResult.Results, typeof(string), null, "en-us") as string;

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("3, 3, 3, 3*, 3", result);
    }

    [TestMethod]
    public void TermResultListConverter_ConvertEmptyResultListTest()
    {
        // arrange
        IReadOnlyList<TermResult> list = [];

        // act
        var result = _conv.Convert(list, typeof(string), null, "en-us") as string;

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void TermResultListConverter_ConvertErrorTargetTypeTest()
    {
        // arrange
        DiceResult diceResult = new()
        {
            DiceExpression = "d20",
            DieRollerUsed = "ConstantDieRoller",
            Results =
            [
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
            ],
            Value = 3,
        };

        // act - assert
        Assert.ThrowsExactly<ArgumentException>(
            [ExcludeFromCodeCoverage] () => _conv.Convert(diceResult.Results, typeof(int), null, "en-us"));
    }

    [TestMethod]
    public void TermResultListConverter_ConvertErrorValueNullTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _conv.Convert(null, typeof(string), null, "en-us"));
    }

    [TestMethod]
    public void TermResultListConverter_ConvertErrorValueTypeTest()
    {
        // arrange
        var value = "testString";

        // act - assert
        Assert.ThrowsExactly<ArgumentException>(
            [ExcludeFromCodeCoverage] () => _conv.Convert(value, typeof(string), null, "en-us"));
    }

    [TestMethod]
    public void TermResultListConverter_ConvertBackTest()
    {
        // arrange
        var value = "testString";

        // act - assert
        Assert.ThrowsExactly<NotSupportedException>(
            [ExcludeFromCodeCoverage] () => _conv.ConvertBack(value, typeof(string), null, "en-us"));
    }
}
