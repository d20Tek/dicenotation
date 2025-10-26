using d20Tek.DiceNotation.DieRoller;
using MathNet.Numerics.Random;

namespace d20Tek.DiceNotation.UnitTests.ExtendedDieRollers;

[TestClass]
public class MathNetDieRollerTests
{
    private readonly MathNetDieRoller _die = new();

    [TestMethod]
    public void MathNetDieRoller_DefaultConstructorTest()
    {
        // arrange

        // act
        var die = new MathNetDieRoller();

        // assert
        Assert.IsNotNull(die);
        Assert.IsInstanceOfType<IDieRoller>(die);
        Assert.IsInstanceOfType<MathNetDieRoller>(die);
    }

    [TestMethod]
    public void MathNetDieRoller_Rolld20Test()
    {
        // arrange

        // act
        var result = _die.Roll(20);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 20, result);
    }

    [TestMethod]
    public void MathNetDieRoller_Rolld4Test()
    {
        // arrange

        // act
        var result = _die.Roll(4);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 4, result);
    }

    [TestMethod]
    public void MathNetDieRoller_Rolld6Test()
    {
        // arrange

        // act
        var result = _die.Roll(6);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 6, result);
    }

    [TestMethod]
    public void MathNetDieRoller_Rolld8Test()
    {
        // arrange

        // act
        var result = _die.Roll(8);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 8, result);
    }

    [TestMethod]
    public void MathNetDieRoller_Rolld12Test()
    {
        // arrange

        // act
        var result = _die.Roll(12);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 12, result);
    }

    [TestMethod]
    public void MathNetDieRoller_Rolld100Test()
    {
        // arrange

        // act
        var result = _die.Roll(100);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 100, result);
    }

    [TestMethod]
    public void MathNetDieRoller_Rolld7Test()
    {
        // arrange

        // act
        var result = _die.Roll(7);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 7, result);
    }

    [TestMethod]
    public void MathNetDieRoller_RollFudgeTest()
    {
        // arrange

        // act
        var result = _die.Roll(3, -2);

        // assert
        AssertHelpers.IsWithinRangeInclusive(-1, 1, result);
    }

    [TestMethod]
    public void MathNetDieRoller_RollMultipleFudgeTest()
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
    public void MathNetDieRoller_RollThousanD6Test()
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

    [TestMethod]
    public void MathNetDieRoller_RollWithAnotherRandomSourceTest()
    {
        // arrange - try out a different MathNet random source (there are many)
        var d = new MathNetDieRoller(new WH2006());

        // act
        for (var i = 0; i < 1000; i++)
        {
            var result = d.Roll(6);

            // assert
            AssertHelpers.IsWithinRangeInclusive(1, 6, result);
        }
    }

    [TestMethod]
    public void CryptoDieRoller_RollErrorTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _die.Roll(0));
    }
}
