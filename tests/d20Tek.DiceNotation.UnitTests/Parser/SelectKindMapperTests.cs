using d20Tek.DiceNotation.Parser;
using d20Tek.DiceNotation.Parser.Parselets;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class SelectKindMapperTests
{
    [TestMethod]
    public void FromTokenKind_WithSelectKind_ReturnsSelectKind()
    {
        // arrange
        var token = new Token(TokenKind.Keep, "k", null, new(1, 1, 2));

        // act
        var result = SelectKindMapper.FromTokenKind(token);

        // assert
        Assert.AreEqual(SelectKind.KeepHigh, result);
    }

    [TestMethod]
    public void FromTokenKind_WithInvalidMapping_ThrowsException()
    {
        // arrange
        var token = new Token(TokenKind.Percent, "%", null, new());

        // act - assert
        Assert.ThrowsExactly<ParseException>([ExcludeFromCodeCoverage]() =>
            SelectKindMapper.FromTokenKind(token));
    }
}
