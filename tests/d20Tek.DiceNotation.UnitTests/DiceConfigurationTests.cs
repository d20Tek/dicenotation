using d20Tek.DiceNotation;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;
using D20Tek.DiceNotation.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceConfigurationTests
{
    readonly IDieRoller _roller = new ConstantDieRoller(2);

    [TestMethod]
    public void DiceConfiguration_SetUnboundedResultTest()
    {
        // arrange
        var dice = new Dice();

        // act
        dice.Configuration.HasBoundedResult = false;
        DiceResult result = dice.Roll("d12-3", _roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d12-3", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(-1, result.Value);
    }

    [TestMethod]
    public void DiceConfiguration_SetBoundedResultMinimumTest()
    {
        // arrange
        var dice = new Dice();

        // act
        dice.Configuration.BoundedResultMinimum = 3;
        DiceResult result = dice.Roll("d7-3", _roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d7-3", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(3, result.Value);
    }

    [TestMethod]
    public void DiceConfiguration_SetDefaultDieSidesTest()
    {
        // arrange
        var dice = new Dice();

        // act
        dice.Configuration.DefaultDieSides = 10;
        DiceResult result = dice.Roll("4dk3+3", _roller);

        // assert
        Assert.IsNotNull(result);
        AssertHelpers.AssertDiceChoose(result, "4dk3+3", "DiceTerm.d10", 4, 3, 3);
    }

    [TestMethod]
    public void DiceConfiguration_SetDefaultDieSidesErrorTest()
    {
        // arrange
        var dice = new Dice();

        // act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => dice.Configuration.DefaultDieSides = 1);

        // assert
    }

    [TestMethod]
    public void DiceConfiguration_SetConstantDefaultDieRoller()
    {
        // arrange
        var dice = new Dice();

        // act
        dice.Configuration.DefaultDieRoller = new ConstantDieRoller(10);
        DiceResult result = dice.Roll("1d20");

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(10, result.Value);
    }

    [TestMethod]
    public void DiceConfiguration_SetDefaultDieRollerErrorTest()
    {
        // arrange
        var dice = new Dice();

        // act
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => dice.Configuration.DefaultDieRoller = null);

        // assert
    }
}
