using d20Tek.DiceNotation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace d20Tek.DiceNotation.UnitTests.Results;

[TestClass]
public class DiceResultTests
{
    private readonly Mock<IDiceConfiguration> config = new();

    public DiceResultTests()
    {
        config.Setup(x => x.HasBoundedResult).Returns(true);
        config.Setup(x => x.BoundedResultMinimum).Returns(1);
    }

    [TestMethod]
    public void ValidateProperties()
    {
        // arrange
        List<TermResult> termList =
        [
            new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
        ];

        // act
        DiceResult result = new()
        {
            DiceExpression = "d6",
            DieRollerUsed = "ConstantDieRoller",
            Results = termList,
            Value = 3,
        };

        // assert
        Assert.AreEqual("d6", result.DiceExpression);
        Assert.AreEqual("ConstantDieRoller", result.DieRollerUsed);
        CollectionAssert.AreEqual(termList, new List<TermResult>(result.Results));
        Assert.AreEqual(3, result.Value);
        Assert.AreEqual("3", result.RollsDisplayText);
    }

    [TestMethod]
    public void GetRollsDisplayText_WithNullList()
    {
        // arrange

        // act
        DiceResult result = new()
        {
            DiceExpression = "d6",
            DieRollerUsed = "ConstantDieRoller",
            Results = null,
            Value = 3,
        };

        // assert
        Assert.AreEqual("d6", result.DiceExpression);
        Assert.AreEqual("ConstantDieRoller", result.DieRollerUsed);
        Assert.IsNull(result.Results);
        Assert.AreEqual(3, result.Value);
        Assert.AreEqual("", result.RollsDisplayText);
    }

    [TestMethod]
    public void Constructor_WithValueSpecified()
    {
        // arrange
        List<TermResult> termList =
        [
            new() { Scalar = 1, Type = "DiceTerm", Value = 5, AppliesToResultCalculation = true },
        ];

        // act
        var result = new DiceResult("d6", 5, termList, "RandomDieRoller", config.Object);

        // assert
        Assert.AreEqual("d6", result.DiceExpression);
        Assert.AreEqual("RandomDieRoller", result.DieRollerUsed);
        CollectionAssert.AreEqual(termList, new List<TermResult>(result.Results));
        Assert.AreEqual(5, result.Value);
    }

    [TestMethod]
    public void Constructor_WithValueCalculated()
    {
        // arrange
        List<TermResult> termList =
        [
            new() { Scalar = 1, Type = "DiceTerm", Value = 5, AppliesToResultCalculation = true },
        ];

        // act
        var result = new DiceResult("d6", termList, "RandomDieRoller", config.Object);

        // assert
        Assert.AreEqual("d6", result.DiceExpression);
        Assert.AreEqual("RandomDieRoller", result.DieRollerUsed);
        CollectionAssert.AreEqual(termList, new List<TermResult>(result.Results));
        Assert.AreEqual(5, result.Value);
    }

    [TestMethod]
    public void Constructor_WithFudgeDice()
    {
        // arrange
        List<TermResult> termList =
        [
            new() { Scalar = 1, Type = "DiceTerm", Value = 1, AppliesToResultCalculation = true },
        ];

        // act
        var result = new DiceResult("1f", 1, termList, "FudgeDieRoller", config.Object);

        // assert
        Assert.AreEqual("1f", result.DiceExpression);
        Assert.AreEqual("FudgeDieRoller", result.DieRollerUsed);
        CollectionAssert.AreEqual(termList, new List<TermResult>(result.Results));
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public void Constructor_WithNonCalculatedResult()
    {
        // arrange
        List<TermResult> termList =
        [
            new() { Scalar = 1, Type = "DiceTerm", Value = 5, AppliesToResultCalculation = true },
            new() { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = false },
        ];

        // act
        var result = new DiceResult("2d6", termList, "RandomDieRoller", config.Object);

        // assert
        Assert.AreEqual("2d6", result.DiceExpression);
        Assert.AreEqual("RandomDieRoller", result.DieRollerUsed);
        CollectionAssert.AreEqual(termList, new List<TermResult>(result.Results));
        Assert.AreEqual(5, result.Value);
    }
}
