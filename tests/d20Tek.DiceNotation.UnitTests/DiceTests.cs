using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceTests
{
    private readonly IDieRoller _roller = new RandomDieRoller();

    [TestMethod]
    public void Dice_ConstructorTest()
    {
        // arrange

        // act
        var dice = new Dice();

        // assert
        Assert.IsNotNull(dice);
        Assert.IsInstanceOfType<IDice>(dice);
        Assert.IsInstanceOfType<Dice>(dice);
        Assert.IsTrue(string.IsNullOrEmpty(dice.ToString()));
        Assert.IsTrue(dice.Configuration.HasBoundedResult);
        Assert.AreEqual(1, dice.Configuration.BoundedResultMinimum);
    }

    [TestMethod]
    public void Dice_ConstructorWithConfigurationTest()
    {
        // arrange
        var config = new DiceConfiguration();

        // act
        var dice = new Dice(config);

        // assert
        Assert.IsNotNull(dice);
        Assert.IsInstanceOfType<IDice>(dice);
        Assert.IsInstanceOfType<Dice>(dice);
        Assert.IsTrue(string.IsNullOrEmpty(dice.ToString()));
        Assert.AreEqual(config, dice.Configuration);
        Assert.IsTrue(dice.Configuration.HasBoundedResult);
        Assert.AreEqual(1, dice.Configuration.BoundedResultMinimum);
    }

    [TestMethod]
    public void Dice_ConstantTest()
    {
        // arrange
        var dice = new Dice();

        // act
        IDice result = dice.Constant(5);

        // assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<IDice>(result);
        Assert.IsInstanceOfType<Dice>(result);
        Assert.AreEqual("5", dice.ToString());
    }

    [TestMethod]
    public void Dice_DiceSidesTest()
    {
        // arrange
        IDice dice = new Dice();

        // act
        IDice result = dice.Dice(8);

        // assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<IDice>(result);
        Assert.IsInstanceOfType<Dice>(result);
        Assert.AreEqual("1d8", dice.ToString());
    }

    [TestMethod]
    public void Dice_DiceChainingTest()
    {
        // arrange
        IDice dice = new Dice();

        // act
        IDice result = dice.Dice(6, 4, choose: 3).Dice(8).Constant(5);

        // assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<IDice>(result);
        Assert.IsInstanceOfType<Dice>(result);
        Assert.AreEqual("4d6k3+1d8+5", dice.ToString());
    }

    [TestMethod]
    public void Dice_DiceClearTest()
    {
        // arrange
        IDice dice = new Dice();
        dice = dice.Dice(6, 4, choose: 3).Dice(8).Constant(5);

        // act
        dice.Clear();
        dice = dice.Dice(6, 1);

        // assert
        Assert.IsNotNull(dice);
        Assert.IsInstanceOfType<IDice>(dice);
        Assert.IsInstanceOfType<Dice>(dice);
        Assert.AreEqual("1d6", dice.ToString());
    }

    [TestMethod]
    public void Dice_FudgeDiceNumberTest()
    {
        // arrange
        var dice = new Dice();

        // act
        IDice result = dice.FudgeDice(3, null);

        // assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<IDice>(result);
        Assert.IsInstanceOfType<Dice>(result);
        Assert.AreEqual("3f", dice.ToString());
    }

    [TestMethod]
    public void Dice_RollConstantTest()
    {
        // arrange
        IDice dice = new Dice().Constant(3);

        // act
        DiceResult result = dice.Roll(_roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
        Assert.AreEqual(3, result.Value);
        Assert.HasCount(1, result.Results);
        foreach(TermResult r in result.Results)
        {
            Assert.AreEqual(3, r.Value);
        }
        Assert.AreEqual("3", result.DiceExpression);
    }

    [TestMethod]
    public void Dice_RollSingleDieTest()
    {
        // arrange
        IDice dice = new Dice();
        dice.Dice(20);

        // act
        DiceResult result = dice.Roll(_roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(1, 20, result.Value);
        Assert.HasCount(1, result.Results);
        int sum = 0;
        foreach (TermResult r in result.Results)
        {
            AssertHelpers.IsWithinRangeInclusive(1, 20, r.Value);
            sum += r.Value;
        }
        Assert.AreEqual(sum, result.Value);
        Assert.AreEqual("1d20", result.DiceExpression);
    }

    [TestMethod]
    public void Dice_RollMultipleDiceTest()
    {
        // arrange
        IDice dice = new Dice();
        dice.Dice(6, 3);

        // act
        DiceResult result = dice.Roll(_roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(3, 18, result.Value);
        Assert.HasCount(3, result.Results);
        int sum = 0;
        foreach (TermResult r in result.Results)
        {
            AssertHelpers.IsWithinRangeInclusive(1, 6, r.Value);
            sum += r.Value;
        }
        Assert.AreEqual(sum, result.Value);
        Assert.AreEqual("3d6", result.DiceExpression);
    }

    [TestMethod]
    public void Dice_RollScalarMultiplierDiceTest()
    {
        // arrange
        IDice dice = new Dice();
        dice.Dice(8, 2, 10);

        // act
        DiceResult result = dice.Roll(_roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
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
        DiceResult result = dice.Roll(_roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
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
        DiceResult result = dice.Roll(_roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
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
        DiceResult result = dice.Roll(new ConstantDieRoller(1));

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.ConstantDieRoller", result.DieRollerUsed);
        Assert.AreEqual(9, result.Value);
        AssertHelpers.AssertDiceChoose(result, "4d6k3+1d8+5", "", 6, 5);
    }

    [TestMethod]
    public void Dice_RollFudgeSingleDieTest()
    {
        // arrange
        var dice = new Dice();
        dice.FudgeDice(1, null);

        // act
        DiceResult result = dice.Roll(this._roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(-1, 1, result.Value);
        Assert.HasCount(1, result.Results);
        int sum = 0;
        foreach (TermResult r in result.Results)
        {
            AssertHelpers.IsWithinRangeInclusive(-1, 1, r.Value);
            sum += r.Value;
        }
        Assert.AreEqual(sum, result.Value);
        Assert.AreEqual("1f", result.DiceExpression);
    }

    [TestMethod]
    public void Dice_RollMultipleFudgeDiceTest()
    {
        // arrange
        IDice dice = new Dice().FudgeDice(6, null);

        // act
        DiceResult result = dice.Roll(this._roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(-6, 6, result.Value);
        Assert.HasCount(6, result.Results);
        int sum = 0;
        foreach (TermResult r in result.Results)
        {
            AssertHelpers.IsWithinRangeInclusive(-1, 1, r.Value);
            sum += r.Value;
        }
        Assert.AreEqual(sum, result.Value);
        Assert.AreEqual("6f", result.DiceExpression);
    }

    [TestMethod]
    public void Dice_RollFudgeChooseDiceTest()
    {
        // arrange
        IDice dice = new Dice().FudgeDice(6, 3);

        // act
        DiceResult result = dice.Roll(this._roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(-3, 3, result.Value);
        AssertHelpers.AssertDiceChoose(result, "6fk3", "FudgeDiceTerm.dF", 6, 3);
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
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(5, 20, result.Value);
        int sum = 0;
        foreach (TermResult r in result.Results)
        {
            AssertHelpers.IsWithinRangeInclusive(1, 6, r.Value);
            sum += r.Value;
        }
        sum += 2;
        Assert.AreEqual(sum, result.Value);
        Assert.AreEqual("3d6+2", result.DiceExpression);
    }

    [TestMethod]
    public void Dice_ParseChainedDiceTest()
    {
        // arrange
        var dice = new Dice();
        
        // act
        DiceResult result = dice.Roll("4d6k3 + 1d8 + 5", new ConstantDieRoller(1));

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.ConstantDieRoller", result.DieRollerUsed);
        Assert.AreEqual(9, result.Value);
        AssertHelpers.AssertDiceChoose(result, "4d6k3+1d8+5", "DiceTerm", 5, 4, 5);
    }

    [TestMethod]
    public void Dice_RollWithNegativeResultUnboundedTest()
    {
        // arrange
        DiceParser parser = new();

        // act
        var dice = new Dice();
        dice.Configuration.HasBoundedResult = false;
        DiceResult result = parser.Parse("d12-3", dice.Configuration, new ConstantDieRoller(1));

        // validate results
        Assert.IsNotNull(result);
        Assert.AreEqual("d12-3", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(-2, result.Value);
    }

    [TestMethod]
    public void Dice_DiceConcat()
    {
        // arrange
        IDice dice1 = new Dice();
        IDice dice2 = new Dice();

        // act
        dice1.Dice(6, 4, choose: 3);
        dice2.Dice(8).Constant(5);
        IDice result = dice1.Concat(dice2);

        // assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<IDice>(result);
        Assert.IsInstanceOfType<Dice>(result);
        Assert.AreEqual("4d6k3+1d8+5", result.ToString());
    }

    [TestMethod]
    public void Dice_DiceConcat_WithNullOther()
    {
        // arrange
        IDice dice1 = new Dice();
        dice1.Dice(6, 4, choose: 3);

        // act
        Assert.ThrowsExactly<ArgumentNullException>([ExcludeFromCodeCoverage]() => dice1.Concat(null));

        // assert
    }

    [TestMethod]
    public void Dice_DiceWith1Side()
    {
        // arrange
        IDice dice = new Dice();

        // act
        var result = dice.Dice(1);
        DiceResult dr = result.Roll();

        // assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<IDice>(result);
        Assert.IsInstanceOfType<Dice>(result);
        Assert.AreEqual("1d1", dice.ToString());
        Assert.AreEqual(1, dr.Value);
    }

    [TestMethod]
    public void Dice_DiceStringWith1Side()
    {
        // arrange
        var dice = new Dice();

        // act
        DiceResult dr = dice.Roll("1d1");

        // assert
        Assert.AreEqual("1d1", dr.DiceExpression);
        Assert.AreEqual(1, dr.Value);
    }

    [TestMethod]
    public void Dice_RollDiceRequest_SingleDieTest()
    {
        // arrange
        var request = new DiceRequest(1, 20);
        var dice = new Dice();

        // act
        DiceResult result = dice.Roll(request);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(1, 20, result.Value);
        Assert.HasCount(1, result.Results);
        int sum = 0;
        foreach (TermResult r in result.Results)
        {
            AssertHelpers.IsWithinRangeInclusive(1, 20, r.Value);
            sum += r.Value;
        }
        Assert.AreEqual(sum, result.Value);
        Assert.AreEqual("1d20", result.DiceExpression);
    }

    [TestMethod]
    public void Dice_RollDiceRequest_BonusTest()
    {
        // arrange
        var request = new DiceRequest(1, 20, 5);
        var dice = new Dice();

        // act
        DiceResult result = dice.Roll(request);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
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
        DiceResult result = dice.Roll(request, _roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(3, 18, result.Value);
        AssertHelpers.AssertDiceChoose(result, "4d6k3", "DiceTerm.d6", 4, 3);
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
        Assert.IsNotNull(result);
        Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
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
