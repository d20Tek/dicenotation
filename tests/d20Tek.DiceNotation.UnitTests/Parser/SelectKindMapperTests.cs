using d20Tek.DiceNotation.Parser;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class SelectKindMapperTests
{
    [TestMethod]
    public void FromTokenKind_WithSelectKind_ReturnsSelectKind()
    {
        // arrange

        // act
        var result = SelectKindMapper.FromTokenKind(TokenKind.Keep);

        // assert
        Assert.AreEqual(SelectKind.KeepHigh, result);
    }

    [TestMethod]
    public void FromTokenKind_WithInvalidMapping_ThrowsException()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ParseException>([ExcludeFromCodeCoverage]() =>
            SelectKindMapper.FromTokenKind(TokenKind.Percent));
    }
}
