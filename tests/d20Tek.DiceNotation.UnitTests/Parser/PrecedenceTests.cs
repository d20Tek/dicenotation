using d20Tek.DiceNotation.Parser;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class PrecedenceTests
{

    [TestMethod]
    public void Get_WithValueTokenKind_ReturnsPrecedenceValue()
    {
        // arrange

        // act
        var result = Precedence.Get(TokenKind.Dice);

        // assert
        Assert.AreEqual(40, result);
    }

    [TestMethod]
    public void Get_WithUnexpectedTokenKind_ReturnsPrecedenceZero()
    {
        // arrange

        // act
        var result = Precedence.Get(TokenKind.Keep);

        // assert
        Assert.AreEqual(0, result);
    }
}
