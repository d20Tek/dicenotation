using d20Tek.DiceNotation.DieRoller;

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
    [DataRow(20)]
    [DataRow(4)]
    [DataRow(6)]
    [DataRow(8)]
    [DataRow(10)]
    [DataRow(12)]
    [DataRow(100)]
    [DataRow(7)]
    [DataRow(1)]
    public void CryptoDieRoller_RollDiceTest(int sides)
    {
        // arrange

        // act
        var result = _die.Roll(sides);

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, sides, result);
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
        var rolls = Enumerable.Range(0, 100).Select(_ => _die.Roll(3, -2)).ToList();

        // assert
        rolls.ForEach(r => AssertHelpers.IsWithinRangeInclusive(-1, 1, r));
    }

    [TestMethod]
    public void CryptoDieRoller_RollThousandD6Test()
    {
        // arrange

        // act
        var rolls = Enumerable.Range(0, 100).Select(_ => _die.Roll(6)).ToList();

        // assert
        rolls.ForEach(r => AssertHelpers.IsWithinRangeInclusive(1, 6, r));
    }
}
