using Microsoft.VisualStudio.TestTools.UnitTesting;
using d20Tek.DiceNotation.DieRoller;
using D20Tek.DiceNotation.UnitTests.Helpers;
using System;

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
    public void RandomDieRoller_Rolld20Test()
    {
        // arrange
        var die = new RandomDieRoller();

        // act
        int result = die.Roll(20);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 20, result);
    }

    [TestMethod]
    public void RandomDieRoller_Rolld4Test()
    {
        // arrange
        var die = new RandomDieRoller();

        // act
        int result = die.Roll(4);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 4, result);
    }

    [TestMethod]
    public void RandomDieRoller_Rolld6Test()
    {
        // arrange
        var die = new RandomDieRoller();

        // act
        int result = die.Roll(6);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 6, result);
    }

    [TestMethod]
    public void RandomDieRoller_Rolld8Test()
    {
        // arrange
        var die = new RandomDieRoller();

        // act
        int result = die.Roll(8);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 8, result);
    }

    [TestMethod]
    public void RandomDieRoller_Rolld12Test()
    {
        // arrange
        var die = new RandomDieRoller();

        // act
        int result = die.Roll(12);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 12, result);
    }

    [TestMethod]
    public void RandomDieRoller_Rolld100Test()
    {
        // arrange
        var die = new RandomDieRoller();

        // act
        int result = die.Roll(100);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 100, result);
    }

    [TestMethod]
    public void RandomDieRoller_Rolld7Test()
    {
        // arrange
        var die = new RandomDieRoller();

        // act
        int result = die.Roll(7);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 7, result);
    }

    [TestMethod]
    public void RandomDieRoller_Rolld1Test()
    {
        // arrange
        var die = new RandomDieRoller();

        // act
        int result = die.Roll(1);

        // assert
        Assert.AreEqual(1, result);
    }
}
