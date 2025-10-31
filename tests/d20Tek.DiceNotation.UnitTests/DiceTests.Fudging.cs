using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceTests_Fudging
{
    private const string _rollerType = "RandomDieRoller";
    private const string _termType = "FudgeDiceTerm.dF";
    private readonly IDieRoller _roller = new RandomDieRoller();

    [TestMethod]
    public void Dice_RollFudgeSingleDieTest()
    {
        // arrange
        var dice = new Dice();
        var expression = DiceExpression.Create().AddFudgeDice(1, null);

        // act
        var result = dice.Roll(expression, _roller);

        // assert
        Assert.Contains(_rollerType, result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(-1, 1, result.Value);
        result.AssertDiceChoose("1f", _termType, 1, 1);
    }

    [TestMethod]
    public void Dice_RollMultipleFudgeDiceTest()
    {
        // arrange
        var dice = new Dice();
        var expression = DiceExpression.Create().AddFudgeDice(6, null);

        // act
        var result = dice.Roll(expression, _roller);

        // assert
        Assert.Contains(_rollerType, result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(-6, 6, result.Value);
        result.AssertDiceChoose("6f", _termType, 6, 6);
    }

    [TestMethod]
    public void Dice_RollFudgeChooseDiceTest()
    {
        // arrange
        var dice = new Dice();
        var expression = DiceExpression.Create().AddFudgeDice(6, 3);

        // act
        var result = dice.Roll(expression, _roller);

        // assert
        Assert.Contains(_rollerType, result.DieRollerUsed);
        AssertHelpers.IsWithinRangeInclusive(-3, 3, result.Value);
        result.AssertDiceChoose("6fk3", _termType, 6, 3);
    }
}
