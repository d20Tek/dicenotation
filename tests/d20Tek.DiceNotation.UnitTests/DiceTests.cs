using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceTests
{
    private const string _rollerType = "RandomDieRoller";
    private const string _diceTermType = "DiceTerm";
    private readonly IDieRoller _roller = new RandomDieRoller();

    [TestMethod]
    public void Dice_RollConstantTest()
    {
        // arrange
        IDice dice = new Dice().Constant(3);

        // act
        var result = dice.Roll(_roller);

        // assert
        Assert.Contains(_rollerType, result.DieRollerUsed);
        Assert.AreEqual(3, result.Value);
        result.AssertDiceChoose("3", "ConstantTerm", 1, 1);
    }

    [TestMethod]
    public void Dice_RollSingleDieTest()
    {
        // arrange
        IDice dice = new Dice();
        dice.Dice(20);

        // act
        var result = dice.Roll(_roller);

        // assert
        Assert.Contains(_rollerType, result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(1, 20, result.Value);
        result.AssertDiceChoose("1d20", _diceTermType, 1, 1);
    }

    [TestMethod]
    public void Dice_RollMultipleDiceTest()
    {
        // arrange
        IDice dice = new Dice();
        dice.Dice(6, 3);

        // act
        var result = dice.Roll(_roller);

        // assert
        Assert.Contains(_rollerType, result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(3, 18, result.Value);
        result.AssertDiceChoose("3d6", _diceTermType, 3, 3);
    }

    [TestMethod]
    public void Dice_RollScalarMultiplierDiceTest()
    {
        // arrange
        IDice dice = new Dice();
        dice.Dice(8, 2, 10);

        // act
        var result = dice.Roll(_roller);

        // assert
        Assert.Contains(_rollerType, result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(20, 160, result.Value);
        Assert.HasCount(2, result.Results);
        int sum = 0;
        foreach (TermResult r in result.Results)
        {
            AssertHelpers.IsWithinRangeInclusive(1, 8, r.Value);
            sum += (int)(r.Value * r.Scalar);
        }
        Assert.AreEqual(sum, result.Value);
        Assert.AreEqual("2d8x10", result.DiceExpression);
    }

    [TestMethod]
    public void Dice_RollChooseDiceTest()
    {
        // arrange
        IDice dice = new Dice();
        dice.Dice(6, 4, choose: 3);

        // act
        var result = dice.Roll(_roller);

        // assert
        Assert.Contains(_rollerType, result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(3, 18, result.Value);
        AssertHelpers.AssertDiceChoose(result, "4d6k3", "DiceTerm.d6", 4, 3);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void Dice_RollExplodingDiceTest()
    {
        // arrange
        IDice dice = new Dice();
        dice.Dice(6, 4, exploding: 6);

        // act
        var result = dice.Roll(_roller);

        // assert
        Assert.Contains(_rollerType, result.DieRollerUsed);
        int sum = 0, count = 4;
        foreach (TermResult r in result.Results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(1, 6, r.Value);
            Assert.AreEqual("DiceTerm.d6", r.Type);
            sum += r.Value;
            if (r.Value >= 6) count++;
        }
        Assert.HasCount(count, result.Results);
        Assert.AreEqual(sum, result.Value);
        Assert.AreEqual("4d6!6", result.DiceExpression);
    }

    [TestMethod]
    public void Dice_RollChainedDiceTest()
    {
        // arrange
        IDice dice = new Dice();
        dice.Dice(6, 4, choose: 3).Dice(8).Constant(5);

        // act
        var result = dice.Roll(new ConstantDieRoller(1));

        // assert
        Assert.Contains("ConstantDieRoller", result.DieRollerUsed);
        Assert.AreEqual(9, result.Value);
        AssertHelpers.AssertDiceChoose(result, "4d6k3+1d8+5", "", 6, 5);
    }

    [TestMethod]
    public void Dice_RollNullRollerTest()
    {
        // arrange
        IDice dice = new Dice();
        dice.Dice(20);

        // act
        var actual = dice.Roll(null);

        // assert
        Assert.That.InRange(actual.Value, 1, 20);
    }

    [TestMethod]
    public void Dice_DiceWith1Side()
    {
        // arrange
        IDice dice = new Dice();

        // act
        var result = dice.Dice(1);
        var dr = result.Roll();

        // assert
        result.AssertNotation("1d1");
        Assert.AreEqual(1, dr.Value);
    }
}
