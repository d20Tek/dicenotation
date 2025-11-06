using d20Tek.DiceNotation.Parser;
using Parse = d20Tek.DiceNotation.Parser.Parser;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class ParserTestsErrors
{
    [ExcludeFromCodeCoverage]
    private static IEnumerable<object[]> ErrorDiceCases =>
    [
        ["1d20g4"],
        ["2d4x"],
        ["2d4/"],
        ["2d4k"],
        ["2d4l"],
        ["2+l2d4"],
        ["2drk4/9"],
        ["7y+2d4k4"],
        ["7!y+2d4"],
        ["2d6%3"],
        ["2d6f"],
        ["6fd"],
        [""],
        ["2k3"],
        ["1!"],
        ["(2+1d20+2+3))x3-10+(3)"],
        ["(2+1d20+(2+3)x3-10+(3)"],
        ["(2+1d20+(2+3))x3-10+()"]
    ];

    [TestMethod]
    [DynamicData(nameof(ErrorDiceCases))]
    public void DiceParser_WithErrors_ThrowsParseExpection(string notation)
    {
        // arrange
        var lexer = new Lexer(notation);
        var parser = new Parse(lexer);

        // act - assert
        Assert.ThrowsExactly<ParseException>(parser.ParseExpression);
    }

    [TestMethod]
    public void Led_WithUnexpectedTokenKind_ThrowsParseException()
    {
        // arrange
        var lexer = new Lexer("1!");
        var parser = new Parse(lexer);

        // act - assert
        Assert.ThrowsExactly<ParseException>([ExcludeFromCodeCoverage]() =>
            parser.Led(new Token(TokenKind.Exploding, "!", null, new(1)), new NumberExpression(1, new(0))));
    }
}
