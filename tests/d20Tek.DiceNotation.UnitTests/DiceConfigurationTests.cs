using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceConfigurationTests
{
    private readonly Dice _dice = new();
    private readonly IDieRoller _roller = new ConstantDieRoller(2);

    [TestMethod]
    public void DiceConfiguration_SetUnboundedResultTest()
    {
        // arrange

        // act
        _dice.Configuration.SetHasBoundedResult(false);
        var result = _dice.Roll("d12-3", _roller);

        // assert
        Assert.AreEqual("d12-3", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(-1, result.Value);
    }

    [TestMethod]
    public void DiceConfiguration_SetBoundedResultMinimumTest()
    {
        // arrange

        // act
        _dice.Configuration.SetBoundedMinimumResult(3);
        var result = _dice.Roll("d7-3", _roller);

        // assert
        Assert.AreEqual("d7-3", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(3, result.Value);
    }

    // todo: default die sides not supported in latest parser, removing test until we bring back support.
    //[TestMethod]
    //public void DiceConfiguration_SetDefaultDieSidesTest()
    //{
    //    // arrange

    //    // act
    //    _dice.Configuration.SetDefaultDieSides(10);
    //    var result = _dice.Roll("4dk3+3", _roller);

    //    // assert
    //    result.AssertDiceChoose("4dk3+3", "DiceTerm.d10", 4, 3, 3);
    //}

    [TestMethod]
    public void DiceConfiguration_SetDefaultDieSidesErrorTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _dice.Configuration.SetDefaultDieSides(1));
    }

    [TestMethod]
    public void DiceConfiguration_SetDefaultDieSidesWithGreater_ErrorTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _dice.Configuration.SetDefaultDieSides(2001));
    }

    [TestMethod]
    public void DiceConfiguration_SetBoundedMinimumResultErrorTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _dice.Configuration.SetBoundedMinimumResult(0));
    }

    [TestMethod]
    public void DiceConfiguration_SetConstantDefaultDieRoller()
    {
        // arrange

        // act
        _dice.Configuration.SetDefaultDieRoller(new ConstantDieRoller(10));
        var result = _dice.Roll("1d20");

        // assert
        Assert.AreEqual(10, result.Value);
    }
}
