using d20Tek.DiceNotation.Parser;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class TokenTests
{
    [TestMethod]
    public void Token_WithChanges_ReturnsUpdatedToken()
    {
        // arrange
        var token = new Token(TokenType.StartOfInput, "");

        // act
        var next = token with { Type = TokenType.Number, Value = "10" };

        // assert
        Assert.AreNotEqual(token, next);
        Assert.AreEqual(TokenType.Number, next.Type);
        Assert.AreEqual("10", next.Value);
    }
}
