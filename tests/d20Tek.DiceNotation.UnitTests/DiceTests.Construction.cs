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
}
