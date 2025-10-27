using d20Tek.DiceNotation.DiceTerms;

namespace d20Tek.DiceNotation.UnitTests.DiceTerms;

[TestClass]
[ExcludeFromCodeCoverage]
public class FudgeDiceTermErrorTests
{
    [TestMethod]
    public void FudgeDiceTerm_ConstructorInvalidNumDiceTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(-5));
    }

    [TestMethod]
    public void FudgeDiceTerm_ConstructorInvalidChooseTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(3, choose: 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(3, choose: -4));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(3, choose: 4));
    }
}
