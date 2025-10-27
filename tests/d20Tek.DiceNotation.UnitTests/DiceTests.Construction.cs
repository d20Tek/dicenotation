namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceTests_Construction
{
    [TestMethod]
    public void Dice_ConstructorTest()
    {
        // arrange

        // act
        var dice = new Dice();

        // assert
        dice.AssertConstruction();
    }

    [TestMethod]
    public void Dice_ConstructorWithConfigurationTest()
    {
        // arrange
        var config = new DiceConfiguration();

        // act
        var dice = new Dice(config);

        // assert
        dice.AssertConstruction();
    }

    [TestMethod]
    public void Dice_ConstantTest()
    {
        // arrange
        var dice = new Dice();

        // act
        IDice result = dice.Constant(5);

        // assert
        result.AssertNotation("5");
    }

    [TestMethod]
    public void Dice_DiceSidesTest()
    {
        // arrange
        IDice dice = new Dice();

        // act
        IDice result = dice.Dice(8);

        // assert
        result.AssertNotation("1d8");
    }

    [TestMethod]
    public void Dice_DiceChainingTest()
    {
        // arrange
        IDice dice = new Dice();

        // act
        IDice result = dice.Dice(6, 4, choose: 3).Dice(8).Constant(5);

        // assert
        result.AssertNotation("4d6k3+1d8+5");
    }

    [TestMethod]
    public void Dice_DiceClearTest()
    {
        // arrange
        IDice dice = new Dice();
        dice = dice.Dice(6, 4, choose: 3).Dice(8).Constant(5);

        // act
        dice.Clear();
        IDice result = dice.Dice(6, 1);

        // assert
        result.AssertNotation("1d6");
    }

    [TestMethod]
    public void Dice_FudgeDiceNumberTest()
    {
        // arrange
        var dice = new Dice();

        // act
        IDice result = dice.FudgeDice(3, null);

        // assert
        result.AssertNotation("3f");
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
        result.AssertNotation("4d6k3+1d8+5");
    }

    [TestMethod]
    public void Dice_DiceConcat_WithNullOther()
    {
        // arrange
        IDice dice1 = new Dice();
        dice1.Dice(6, 4, choose: 3);

        // act - assert
        Assert.ThrowsExactly<ArgumentNullException>([ExcludeFromCodeCoverage] () => dice1.Concat(null));
    }
}
