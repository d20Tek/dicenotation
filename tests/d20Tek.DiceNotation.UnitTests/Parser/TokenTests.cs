using d20Tek.DiceNotation.Parser;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class TokenTests
{
    [TestMethod]
    public void Token_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var token = new Token(TokenKind.StartOfInput, "", null, new());

        // act
        var next = token with { Kind = TokenKind.Number, Lexeme = "5", IntValue = 5, Pos = new Position(1, 0, 5) };

        // assert
        Assert.AreNotEqual(token, next);
        Assert.AreEqual(TokenKind.Number, next.Kind);
        Assert.AreEqual("5", next.Lexeme);
        Assert.AreEqual(5, next.IntValue);
        Assert.AreEqual(new(1, 0, 5), next.Pos);
    }

    [TestMethod]
    public void Token_ToString()
    {
        // arrange
        var token = new Token(TokenKind.Number, "12", 12, new(5, 1, 6));

        // act
        var result = token.ToString();

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Number '12' @ L:1,C:6", result);
    }
}
