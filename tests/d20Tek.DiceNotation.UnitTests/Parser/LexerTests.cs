using d20Tek.DiceNotation.Parser;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class LexerTests
{
    [TestMethod]
    public void GetNextToken_WithDiceTerm()
    {
        // arrange
        var lexer = new Lexer("1d20+3");

        // act
        var tokens = TokenizeNotation(lexer);

        // assert
        Assert.IsNotNull(tokens);
        Assert.HasCount(6, tokens);
        Assert.AreEqual(new Token(TokenKind.Number, "1", 1, new(0, 1, 1)), tokens[0]);
        Assert.AreEqual(new Token(TokenKind.Dice, "d", null, new(1, 1, 2)), tokens[1]);
        Assert.AreEqual(new Token(TokenKind.Number, "20", 20, new(2, 1, 3)), tokens[2]);
        Assert.AreEqual(new Token(TokenKind.Plus, "+", null, new(4, 1, 5)), tokens[3]);
        Assert.AreEqual(new Token(TokenKind.Number, "3", 3, new(5, 1, 6)), tokens[4]);
        Assert.AreEqual(TokenKind.EndOfInput, tokens[5].Kind);
    }

    [TestMethod]
    public void GetNextToken_WithChooseDiceTerm()
    {
        // arrange
        var lexer = new Lexer("4d6k3");

        // act
        var tokens = TokenizeNotation(lexer);

        // assert
        Assert.IsNotNull(tokens);
        Assert.HasCount(6, tokens);
        Assert.AreEqual(new Token(TokenKind.Number, "4", 4, new(0, 1, 1)), tokens[0]);
        Assert.AreEqual(new Token(TokenKind.Dice, "d", null, new(1, 1, 2)), tokens[1]);
        Assert.AreEqual(new Token(TokenKind.Number, "6", 6, new(2, 1, 3)), tokens[2]);
        Assert.AreEqual(new Token(TokenKind.Keep, "k", null, new(3, 1, 4)), tokens[3]);
        Assert.AreEqual(new Token(TokenKind.Number, "3", 3, new(4, 1, 5)), tokens[4]);
        Assert.AreEqual(TokenKind.EndOfInput, tokens[5].Kind);
    }

    [TestMethod]
    public void GetNextToken_WithTermGrouping()
    {
        // arrange
        var lexer = new Lexer("((2+1d20)+(2+3))");

        // act
        var tokens = TokenizeNotation(lexer);

        // assert
        Assert.IsNotNull(tokens);
        Assert.HasCount(16, tokens);
        Assert.AreEqual(new Token(TokenKind.GroupStart, "(", null, new(0, 1, 1)), tokens[0]);
        Assert.AreEqual(new Token(TokenKind.GroupStart, "(", null, new(1, 1, 2)), tokens[1]);
        Assert.AreEqual(new Token(TokenKind.Number, "2", 2, new(2, 1, 3)), tokens[2]);
        Assert.AreEqual(new Token(TokenKind.Plus, "+", null, new(3, 1, 4)), tokens[3]);
        Assert.AreEqual(new Token(TokenKind.Number, "1", 1, new(4, 1, 5)), tokens[4]);
        Assert.AreEqual(new Token(TokenKind.Dice, "d", null, new(5, 1, 6)), tokens[5]);
        Assert.AreEqual(new Token(TokenKind.Number, "20", 20, new(6, 1, 7)), tokens[6]);
        Assert.AreEqual(new Token(TokenKind.GroupEnd, ")", null, new(8, 1, 9)), tokens[7]);
        Assert.AreEqual(new Token(TokenKind.Plus, "+", null, new(9, 1, 10)), tokens[8]);
        Assert.AreEqual(new Token(TokenKind.GroupStart, "(", null, new(10, 1, 11)), tokens[9]);
        Assert.AreEqual(new Token(TokenKind.Number, "2", 2, new(11, 1, 12)), tokens[10]);
        Assert.AreEqual(new Token(TokenKind.Plus, "+", null, new(12, 1, 13)), tokens[11]);
        Assert.AreEqual(new Token(TokenKind.Number, "3", 3, new(13, 1, 14)), tokens[12]);
        Assert.AreEqual(new Token(TokenKind.GroupEnd, ")", null, new(14, 1, 15)), tokens[13]);
        Assert.AreEqual(new Token(TokenKind.GroupEnd, ")", null, new(15, 1, 16)), tokens[14]);
        Assert.AreEqual(TokenKind.EndOfInput, tokens[15].Kind);
    }

    [TestMethod]
    public void GetNextToken_WithInvalidOperator()
    {
        // arrange
        var lexer = new Lexer("4d6g3");

        // act - assert
        var exception = Assert.ThrowsExactly<ParseException>([ExcludeFromCodeCoverage]() => TokenizeNotation(lexer));
        Assert.AreEqual(new Position(3, 1, 4).ToString(), exception.Position);
    }

    [ExcludeFromCodeCoverage]
    private static IEnumerable<object[]> ImplicitOneCases =>
    [
        ["d20", new string[] { "d", "20", string.Empty }],
        ["(d6)", new string[] { "(", "d", "6", ")", string.Empty }],
        ["+d6", new string[] { "+", "d", "6", string.Empty }],
        ["xd6", new string[] { "x", "d","6", string.Empty }],
        ["d%", new string[] { "d", "%", string.Empty }],
        ["f6", new string[] { "f", "6", string.Empty }],
        ["3f!", new string[] { "3", "f", "!", string.Empty }],
        ["-d6", new string[] { "-", "d", "6", string.Empty }],
        ["((d6)+d8)", new string[] { "(", "(", "d", "6", ")", "+", "d", "8", ")", string.Empty }],
        ["4 * 2", new string[] { "4", "*", "2", string.Empty }],
        ["4 / 2", new string[] { "4", "/", "2", string.Empty }],
        ["4d6k3", new string[] { "4", "d", "6", "k", "3", string.Empty}],
        ["4d6p1", new string[] { "4", "d", "6", "p", "1", string.Empty}],
        ["6d6l2", new string[] { "6", "d", "6", "l", "2", string.Empty}],
        ["  ( (    2d6 )  +  d8   - 1  ) ", new string[] { "(", "(", "2", "d", "6", ")", "+", "d", "8", "-", "1", ")", string.Empty }],
    ];

    [TestMethod]
    [DynamicData(nameof(ImplicitOneCases))]
    public void GetNextToken_ShouldInsertImplicitOne(string notation, object[] expected)
    {
        // arrange
        var lexer = new Lexer(notation);

        // act
        var tokens = TokenizeNotation(lexer);

        // assert
        CollectionAssert.AreEqual(expected, tokens.Select(x => x.Lexeme).ToArray());
    }


    [TestMethod]
    public void GetNextToken_MultipleLines()
    {
        // arrange
        var lexer = new Lexer("1d20\n+3");

        // act
        var tokens = TokenizeNotation(lexer);

        // assert
        Assert.IsNotNull(tokens);
        Assert.HasCount(6, tokens);
        Assert.AreEqual(new Token(TokenKind.Number, "1", 1, new(0, 1, 1)), tokens[0]);
        Assert.AreEqual(new Token(TokenKind.Dice, "d", null, new(1, 1, 2)), tokens[1]);
        Assert.AreEqual(new Token(TokenKind.Number, "20", 20, new(2, 1, 3)), tokens[2]);
        Assert.AreEqual(new Token(TokenKind.Plus, "+", null, new(5, 2, 1)), tokens[3]);
        Assert.AreEqual(new Token(TokenKind.Number, "3", 3, new(6, 2, 2)), tokens[4]);
        Assert.AreEqual(TokenKind.EndOfInput, tokens[5].Kind);
    }

    [TestMethod]
    public void GetNextToken_EndWithNewLine()
    {
        // arrange
        var lexer = new Lexer("1d20+\n  ");

        // act
        var tokens = TokenizeNotation(lexer);

        // assert
        Assert.IsNotNull(tokens);
        Assert.HasCount(5, tokens);
        Assert.AreEqual(new Token(TokenKind.Number, "1", 1, new(0, 1, 1)), tokens[0]);
        Assert.AreEqual(new Token(TokenKind.Dice, "d", null, new(1, 1, 2)), tokens[1]);
        Assert.AreEqual(new Token(TokenKind.Number, "20", 20, new(2, 1, 3)), tokens[2]);
        Assert.AreEqual(new Token(TokenKind.Plus, "+", null, new(4, 1, 5)), tokens[3]);
        Assert.AreEqual(TokenKind.EndOfInput, tokens[4].Kind);
    }

    private static List<Token> TokenizeNotation(Lexer lexer)
    {
        var tokens = new List<Token>();
        var currToken = new Token(TokenKind.StartOfInput, string.Empty, null, new());

        while (currToken.Kind != TokenKind.EndOfInput)
        {
            currToken = lexer.GetNextToken();
            tokens.Add(currToken);
        }

        return tokens;
    }
}
