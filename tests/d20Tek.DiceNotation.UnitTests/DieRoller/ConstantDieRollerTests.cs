using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace d20Tek.DiceNotation.DieRoller.UnitTests;

[TestClass]
public class ConstantDieRollerTests
{
    [TestMethod]
    public void ConstantDieRoller_DefaultConstructorTest()
    {
        // arrange

        // act
        var die = new ConstantDieRoller();

        // assert
        Assert.IsNotNull(die);
        Assert.IsInstanceOfType<IDieRoller>(die);
        Assert.IsInstanceOfType<ConstantDieRoller>(die);
    }

    [TestMethod]
    public void ConstantDieRoller_RollDefaultConstantTest()
    {
        // arrange
        var die = new ConstantDieRoller();

        // act
        int result = die.Roll(20);

        // assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void ConstantDieRoller_RollConstantTest()
    {
        // arrange
        var die = new ConstantDieRoller(3);

        // act
        int result = die.Roll(6);

        // assert
        Assert.AreEqual(3, result);
    }
}
