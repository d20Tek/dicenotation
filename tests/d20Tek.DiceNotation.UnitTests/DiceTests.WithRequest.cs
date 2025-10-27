using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceTests_WithRequest
{
    private const string _rollerType = "RandomDieRoller";
    private const string _diceTermType = "DiceTerm";
    private readonly IDieRoller _roller = new RandomDieRoller();

    [TestMethod]
    public void Dice_RollDiceRequest_SingleDieTest()
    {
        // arrange
        var request = new DiceRequest(1, 20);
        var dice = new Dice();

        // act
        var result = dice.Roll(request);

        // assert
        Assert.Contains(_rollerType, result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(1, 20, result.Value);
        result.AssertDiceChoose("1d20", _diceTermType, 1, 1);
    }

    [TestMethod]
    public void Dice_RollDiceRequest_BonusTest()
    {
        // arrange
        var request = new DiceRequest(1, 20, 5);
        var dice = new Dice();

        // act
        var result = dice.Roll(request);

        // assert
        Assert.Contains(_rollerType, result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(6, 25, result.Value);
        Assert.HasCount(2, result.Results);
        int sum = 0;
        foreach (TermResult r in result.Results)
        {
            AssertHelpers.IsWithinRangeInclusive(1, 20, r.Value);
            sum += r.Value;
        }
        Assert.AreEqual(sum, result.Value);
        Assert.AreEqual("1d20+5", result.DiceExpression);
    }

    [TestMethod]
    public void Dice_RollDiceRequest_ChooseDiceTest()
    {
        // arrange
        var dice = new Dice();
        var request = new DiceRequest(4, 6, choose: 3);

        // act
        var result = dice.Roll(request, _roller);

        // assert
        Assert.Contains(_rollerType, result.DieRollerUsed);
        AssertHelpers.AssertDiceChoose(result, "4d6k3", _diceTermType, 4, 3);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void Dice_RollDiceRequest_ExplodingDiceTest()
    {
        // setup test
        IDice dice = new Dice();
        var request = new DiceRequest(4, 6, exploding: 6);

        // run test
        DiceResult result = dice.Roll(request, _roller);

        // validate results
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
}
