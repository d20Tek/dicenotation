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

        // act
        var result = SelectKindMapper.FromTokenKind(TokenKind.Keep, new(1, 1, 2));

        // assert
        Assert.AreEqual(SelectKind.KeepHigh, result);
    }

    [TestMethod]
    public void FromTokenKind_WithInvalidMapping_ThrowsException()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ParseException>([ExcludeFromCodeCoverage]() =>
            SelectKindMapper.FromTokenKind(TokenKind.Percent, new()));
    }
}
