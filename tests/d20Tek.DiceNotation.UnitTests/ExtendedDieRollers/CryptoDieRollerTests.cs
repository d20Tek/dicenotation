using d20Tek.DiceNotation.DieRoller;
using D20Tek.DiceNotation.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace d20Tek.DiceNotation.UnitTests.ExtendedDieRollers;

[TestClass]
public class CryptoDieRollerTests
{
    private readonly CryptoDieRoller _die = new();

    [TestMethod]
    public void CryptoDieRoller_DefaultConstructorTest()
    {
        // arrange

        // act
        var die = new CryptoDieRoller();

        // assert
        Assert.IsNotNull(die);
        Assert.IsInstanceOfType<IDieRoller>(die);
        Assert.IsInstanceOfType<CryptoDieRoller>(die);
    }

    [TestMethod]
    public void CryptoDieRoller_Rolld20Test()
    {
        // arrange

        // act
        var result = _die.Roll(20);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 20, result);
    }

    [TestMethod]
    public void CryptoDieRoller_Rolld4Test()
    {
        // arrange

        // act
        var result = _die.Roll(4);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 4, result);
    }

    [TestMethod]
    public void CryptoDieRoller_Rolld6Test()
    {
        // arrange

        // act
        var result = _die.Roll(6);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 6, result);
    }

    [TestMethod]
    public void CryptoDieRoller_Rolld8Test()
    {
        // arrange

        // act
        var result = _die.Roll(8);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 8, result);
    }

    [TestMethod]
    public void CryptoDieRoller_Rolld12Test()
    {
        // arrange

        // act
        var result = _die.Roll(12);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 12, result);
    }

    [TestMethod]
    public void CryptoDieRoller_Rolld100Test()
    {
        // arrange

        // act
        var result = _die.Roll(100);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 100, result);
    }

    [TestMethod]
    public void CryptoDieRoller_Rolld7Test()
    {
        // arrange

        // act
        var result = _die.Roll(7);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 7, result);
    }

    [TestMethod]
    public void CryptoDieRoller_RollFudgeTest()
    {
        // arrange

        // act
        var result = _die.Roll(3, -2);

        // assert
        AssertHelpers.IsWithinRangeInclusive(-1, 1, result);
    }

    [TestMethod]
    public void CryptoDieRoller_RollMultipleFudgeTest()
    {
        // arrange

        // act
        for (var i = 0; i < 100; i++)
        {
            var result = _die.Roll(3, -2);

            // assert
            AssertHelpers.IsWithinRangeInclusive(-1, 1, result);
        }
    }

    [TestMethod]
    public void CryptoDieRoller_RollThousandD6Test()
    {
        // arrange

        // act
        for (var i = 0; i < 1000; i++)
        {
            var result = _die.Roll(6);

            // assert
            AssertHelpers.IsWithinRangeInclusive(1, 6, result);
        }
    }
}
