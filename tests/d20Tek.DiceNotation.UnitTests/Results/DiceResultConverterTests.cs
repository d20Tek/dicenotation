using d20Tek.DiceNotation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace d20Tek.DiceNotation.UnitTests.Results;

[TestClass]
public class DiceResultConverterTests
{
    private readonly DiceResultConverter _conv = new();

    [TestMethod]
    public void DiceResultConverter_ConstructorTest()
    {
        // arrange

        // act
        var conv = new DiceResultConverter();

        // assert
        Assert.IsNotNull(conv);
        Assert.IsInstanceOfType<DiceResultConverter>(conv);
    }


    [TestMethod]
    public void DiceResultConverter_ConvertTextTest()
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
        var result = _conv.Convert(diceResult, typeof(string), null, "en-us") as string;

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("3 (d6)", result);
    }

    [TestMethod]
    public void DiceResultConverter_ConvertComplexTextTest()
    {
        // arrange
        //DiceResult diceResult = this.dice.Roll("4d6k3 + d8 + 5", this.roller);
        DiceResult diceResult = new()
        {
            DiceExpression = "4d6k3+d8+5",
            DieRollerUsed = "ConstantDieRoller",
            Results =
            [
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
            ],
            Value = 17,
        };

        // act
        var result = _conv.Convert(diceResult, typeof(string), null, "en-us") as string;

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("17 (4d6k3+d8+5)", result);
    }

    [TestMethod]
    public void DiceResultConverter_ConvertErrorTargetTypeTest()
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
            [ExcludeFromCodeCoverage] () => _conv.Convert(diceResult, typeof(int), null, "en-us"));
    }

    [TestMethod]
    public void DiceResultConverter_ConvertErrorValueNullTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _conv.Convert(null, typeof(string), null, "en-us"));
    }

    [TestMethod]
    public void DiceResultConverter_ConvertErrorValueTypeTest()
    {
        // arrange
        var value = "testString";

        // act - assert
        Assert.ThrowsExactly<ArgumentException>(
            [ExcludeFromCodeCoverage] () => _conv.Convert(value, typeof(string), null, "en-us"));
    }

    [TestMethod]
    public void DiceResultConverter_ConvertBackTest()
    {
        // arrange
        var value = "testString";

        // act - assert
        Assert.ThrowsExactly<NotSupportedException>(
            [ExcludeFromCodeCoverage] () => _conv.ConvertBack(value, typeof(string), null, "en-us"));
    }
}
