using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

[TestClass]
public class RandomDieRollerTests
{
    [TestMethod]
    public void RandomDieRoller_DefaultConstructorTest()
    {
        // arrange

        // act
        var die = new RandomDieRoller();

        // assert
        Assert.IsNotNull(die);
        Assert.IsInstanceOfType<IDieRoller>(die);
        Assert.IsInstanceOfType<RandomDieRoller>(die);
    }

    [TestMethod]
    public void RandomDieRoller_ConstructorRandomGeneratorTest()
    {
        // arrange
        Random rand = new(42);

        // act
        var die = new RandomDieRoller(rand, null);

        // assert
        Assert.IsNotNull(die);
        Assert.IsInstanceOfType<IDieRoller>(die);
        Assert.IsInstanceOfType<RandomDieRoller>(die);
    }

    [TestMethod]
    [DataRow(20)]
    [DataRow(4)]
    [DataRow(6)]
    [DataRow(8)]
    [DataRow(10)]
    [DataRow(12)]
    [DataRow(100)]
    [DataRow(7)]
    [DataRow(1)]
    public void RandomDieRoller_RollDiceTest(int sides)
    {
        // arrange
        var die = new RandomDieRoller();

        // act
        int result = die.Roll(sides);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, sides, result);
    }
}
