using d20Tek.DiceNotation.Parser;
using d20Tek.DiceNotation.Parser.Parselets;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class ParseletTableTests
{
    [TestMethod]
    [DataRow((int)TokenKind.Number, "20", 20, typeof(NumberPrefix), DisplayName = "Number prefix")]
    [DataRow((int)TokenKind.GroupStart, "(", null, typeof(GroupPrefix), DisplayName = "Group start prefix")]
    [DataRow((int)TokenKind.Plus, "+", null, typeof(UnaryPrefix), DisplayName = "Unary prefix")]
    [DataRow((int)TokenKind.Minus, "-", null, typeof(UnaryPrefix), DisplayName = "Unary prefix")]
    [DataRow((int)TokenKind.Dice, "d", null, typeof(DicePrefix), DisplayName = "Dice prefix")]
    [DataRow((int)TokenKind.FudgeDice, "f", null, typeof(FudgeDicePrefix), DisplayName = "Fudge dice prefix")]
    public void GetPrefix_WithValidPrefixTokenKind_ReturnsParselet(int tokenKind, string lexeme, int? value, Type prefixType)
    {
        // arrange
        var parselets = new ParseletTable();
        var token = new Token((TokenKind)tokenKind, lexeme, value, new());

        // act
        var result = parselets.GetPrefix(token);

        // assert
        Assert.IsInstanceOfType(result, prefixType);
    }

    [TestMethod]
    public void GetPrefix_WithNonPrefixTokenKind_ThrowsException()
    {
        // arrange
        var parselets = new ParseletTable();
        var token = new Token(TokenKind.GroupEnd, ")", null, new(0, 1, 1));

        // act - assert
        Assert.ThrowsExactly<ParseException>([ExcludeFromCodeCoverage]() => parselets.GetPrefix(token));
    }

    [TestMethod]
    [DataRow((int)TokenKind.Plus, "+", typeof(BinaryInfix))]
    [DataRow((int)TokenKind.Minus, "-", typeof(BinaryInfix))]
    [DataRow((int)TokenKind.Star, "*", typeof(BinaryInfix))]
    [DataRow((int)TokenKind.Times, "x", typeof(BinaryInfix))]
    [DataRow((int)TokenKind.Divide, "/", typeof(BinaryInfix))]
    [DataRow((int)TokenKind.Dice, "d", typeof(DiceInfix))]
    [DataRow((int)TokenKind.FudgeDice, "f", typeof(FudgeDiceInfix))]
    public void GetInfix_WithValidInfixTokenKind_ReturnsParselet(int tokenKind, string lexeme, Type infixType)
    {
        // arrange
        var parselets = new ParseletTable();
        var token = new Token((TokenKind)tokenKind, lexeme, null, new());

        // act
        var result = parselets.GetInfix(token);

        // assert
        Assert.IsInstanceOfType(result, infixType);
    }

    [TestMethod]
    public void GetInfix_WithNonPrefixTokenKind_ThrowsException()
    {
        // arrange
        var parselets = new ParseletTable();
        var token = new Token(TokenKind.Number, "6", 6, new(0, 1, 1));

        // act - assert
        Assert.ThrowsExactly<ParseException>([ExcludeFromCodeCoverage] () => parselets.GetInfix(token));
    }

    [TestMethod]
    public void GetInfixPrecedence_WithValidInfixTokenKind_ReturnsPrecedence()
    {
        // arrange
        var parselets = new ParseletTable();
        var token = new Token(TokenKind.Dice, "d", null, new(1, 1, 2));

        // act
        var result = parselets.GetInfixPrecedence(token);

        // assert
        Assert.AreEqual(40, result);
    }

    [TestMethod]
    public void GetInfixPrecedence_WithNonInfixTokenKind_ReturnsZero()
    {
        // arrange
        var parselets = new ParseletTable();
        var token = new Token(TokenKind.Number, "4", 4, new(0, 1, 1));

        // act
        var result = parselets.GetInfixPrecedence(token);

        // assert
        Assert.AreEqual(0, result);
    }
}
