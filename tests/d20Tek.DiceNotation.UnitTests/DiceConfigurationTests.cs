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
        _dice.Configuration.HasBoundedResult = false;
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
        _dice.Configuration.BoundedResultMinimum = 3;
        var result = _dice.Roll("d7-3", _roller);

        // assert
        Assert.AreEqual("d7-3", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(3, result.Value);
    }

    [TestMethod]
    public void DiceConfiguration_SetDefaultDieSidesTest()
    {
        // arrange

        // act
        _dice.Configuration.DefaultDieSides = 10;
        var result = _dice.Roll("4dk3+3", _roller);

        // assert
        result.AssertDiceChoose("4dk3+3", "DiceTerm.d10", 4, 3, 3);
    }

    [TestMethod]
    public void DiceConfiguration_SetDefaultDieSidesErrorTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _dice.Configuration.DefaultDieSides = 1);
    }

    [TestMethod]
    public void DiceConfiguration_SetConstantDefaultDieRoller()
    {
        // arrange

        // act
        _dice.Configuration.DefaultDieRoller = new ConstantDieRoller(10);
        var result = _dice.Roll("1d20");

        // assert
        Assert.AreEqual(10, result.Value);
    }

    [TestMethod]
    public void DiceConfiguration_SetDefaultDieRollerErrorTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _dice.Configuration.DefaultDieRoller = null);
    }
}
