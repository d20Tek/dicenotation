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
    [DataRow(20)]
    [DataRow(4)]
    [DataRow(6)]
    [DataRow(8)]
    [DataRow(10)]
    [DataRow(12)]
    [DataRow(100)]
    [DataRow(7)]
    public void MathNetDieRoller_RolldDiceTest(int sides)
    {
        // arrange

        // act
        var result = _die.Roll(sides);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, sides, result);
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
        var rolls = Enumerable.Range(0, 100).Select(_ => _die.Roll(3, -2)).ToList();

        // assert
        rolls.ForEach(r => AssertHelpers.IsWithinRangeInclusive(-1, 1, r));
    }

    [TestMethod]
    public void MathNetDieRoller_RollThousanD6Test()
    {
        // arrange

        // act
        var rolls = Enumerable.Range(0, 100).Select(_ => _die.Roll(6)).ToList();

        // assert
        rolls.ForEach(r => AssertHelpers.IsWithinRangeInclusive(1, 6, r));
    }

    [TestMethod]
    public void MathNetDieRoller_RollWithAnotherRandomSourceTest()
    {
        // arrange - try out a different MathNet random source (there are many)
        var d = new MathNetDieRoller(new WH2006());

        // act
        var rolls = Enumerable.Range(0, 1000).Select(_ => _die.Roll(6)).ToList();

        // assert
        rolls.ForEach(r => AssertHelpers.IsWithinRangeInclusive(1, 6, r));
    }

    [TestMethod]
    public void CryptoDieRoller_RollErrorTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>([ExcludeFromCodeCoverage] () => _die.Roll(0));
    }
}
