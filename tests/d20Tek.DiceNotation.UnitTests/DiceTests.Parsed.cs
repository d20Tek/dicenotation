using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceTests_Parsed
{
    private const string _rollerType = "RandomDieRoller";
    private const string _diceTermType = "DiceTerm";
    private readonly IDieRoller _roller = new RandomDieRoller();

    [TestMethod]
    public void Dice_RollStringNullRollerTest()
    {
        // arrange
        var dice = new Dice();

        // act
        var actual = dice.Roll("1d20");

        // assert
        Assert.That.InRange(actual.Value, 1, 20);
    }

    [TestMethod]
    public void Dice_ParseMultipleDiceTest()
    {
        // arrange
        var dice = new Dice();

        // act
        DiceResult result = dice.Roll("3d6+2", _roller);

        // assert
        Assert.Contains(_rollerType, result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(5, 20, result.Value);
        result.AssertDiceChoose("3d6+2", _diceTermType, 3, 3, 2);
    }

    [TestMethod]
    public void Dice_ParseChainedDiceTest()
    {
        // arrange
        var dice = new Dice();

        // act
        var result = dice.Roll("4d6k3 + 1d8 + 5", new ConstantDieRoller(1));

        // assert
        Assert.Contains("ConstantDieRoller", result.DieRollerUsed);
        Assert.AreEqual(9, result.Value);
        result.AssertDiceChoose("4d6k3+1d8+5", _diceTermType, 5, 4, 5);
    }

    [TestMethod]
    public void Dice_DiceStringWith1Side()
    {
        // arrange
        var dice = new Dice();

        // act
        var dr = dice.Roll("1d1");

        // assert
        Assert.AreEqual("1d1", dr.DiceExpression);
        Assert.AreEqual(1, dr.Value);
    }

    [TestMethod]
    public void Dice_RollWithNegativeResultUnboundedTest()
    {
        // arrange
        DiceParser parser = new();

        // act
        var dice = new Dice();
        dice.Configuration.SetHasBoundedResult(false);
        var result = parser.Parse("d12-3", dice.Configuration, new ConstantDieRoller(1));

        // validate results
        Assert.IsNotNull(result);
        Assert.AreEqual("d12-3", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(-2, result.Value);
    }
}
