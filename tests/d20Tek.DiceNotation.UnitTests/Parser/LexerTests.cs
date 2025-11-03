using d20Tek.DiceNotation.Parser;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class LexerTests
{
    private static Token _endToken = new(TokenType.EndOfInput, string.Empty);
    private readonly Lexer _lexer = new();

    [TestMethod]
    public void Tokenize_WithDiceTerm()
    {
        // arrange

        // act
        var tokens = _lexer.Tokenize("1d20+3");

        // assert
        Assert.IsNotNull(tokens);
        Assert.HasCount(6, tokens);
        Assert.AreEqual(new Token(TokenType.Number, "1"), tokens.ElementAt(0));
        Assert.AreEqual(new Token(TokenType.Operator, "d"), tokens.ElementAt(1));
        Assert.AreEqual(new Token(TokenType.Number, "20"), tokens.ElementAt(2));
        Assert.AreEqual(new Token(TokenType.Operator, "+"), tokens.ElementAt(3));
        Assert.AreEqual(new Token(TokenType.Number, "3"), tokens.ElementAt(4));
    }

    [TestMethod]
    public void Tokenize_WithChooseDiceTerm()
    {
        // arrange

        // act
        var tokens = _lexer.Tokenize("4d6k3");

        // assert
        Assert.IsNotNull(tokens);
        Assert.HasCount(6, tokens);
        Assert.AreEqual(new Token(TokenType.Number, "4"), tokens.ElementAt(0));
        Assert.AreEqual(new Token(TokenType.Operator, "d"), tokens.ElementAt(1));
        Assert.AreEqual(new Token(TokenType.Number, "6"), tokens.ElementAt(2));
        Assert.AreEqual(new Token(TokenType.Operator, "k"), tokens.ElementAt(3));
        Assert.AreEqual(new Token(TokenType.Number, "3"), tokens.ElementAt(4));
    }

    [TestMethod]
    public void Tokenize_WithTermGrouping()
    {
        // arrange

        // act
        var tokens = _lexer.Tokenize("((2+1d20)+(2+3))");

        // assert
        Assert.IsNotNull(tokens);
        Assert.HasCount(16, tokens);
        Assert.AreEqual(new Token(TokenType.GroupStart, "("), tokens.ElementAt(0));
        Assert.AreEqual(new Token(TokenType.GroupStart, "("), tokens.ElementAt(1));
        Assert.AreEqual(new Token(TokenType.Number, "2"), tokens.ElementAt(2));
        Assert.AreEqual(new Token(TokenType.Operator, "+"), tokens.ElementAt(3));
        Assert.AreEqual(new Token(TokenType.Number, "1"), tokens.ElementAt(4));
        Assert.AreEqual(new Token(TokenType.Operator, "d"), tokens.ElementAt(5));
        Assert.AreEqual(new Token(TokenType.Number, "20"), tokens.ElementAt(6));
        Assert.AreEqual(new Token(TokenType.GroupEnd, ")"), tokens.ElementAt(7));
        Assert.AreEqual(new Token(TokenType.Operator, "+"), tokens.ElementAt(8));
        Assert.AreEqual(new Token(TokenType.GroupStart, "("), tokens.ElementAt(9));
        Assert.AreEqual(new Token(TokenType.Number, "2"), tokens.ElementAt(10));
        Assert.AreEqual(new Token(TokenType.Operator, "+"), tokens.ElementAt(11));
        Assert.AreEqual(new Token(TokenType.Number, "3"), tokens.ElementAt(12));
        Assert.AreEqual(new Token(TokenType.GroupEnd, ")"), tokens.ElementAt(13));
        Assert.AreEqual(new Token(TokenType.GroupEnd, ")"), tokens.ElementAt(14));
    }

    [TestMethod]
    public void Tokenize_WithInvalidOperator()
    {
        // arrange

        // act
        var tokens = _lexer.Tokenize("4d6g3");

        // assert
        Assert.IsNotNull(tokens);
        Assert.HasCount(6, tokens);
        Assert.AreEqual(new Token(TokenType.Number, "4"), tokens.ElementAt(0));
        Assert.AreEqual(new Token(TokenType.Operator, "d"), tokens.ElementAt(1));
        Assert.AreEqual(new Token(TokenType.Number, "6"), tokens.ElementAt(2));
        Assert.AreEqual(new Token(TokenType.Identifier, "g"), tokens.ElementAt(3));
        Assert.AreEqual(new Token(TokenType.Number, "3"), tokens.ElementAt(4));
    }

    private static IEnumerable<object[]> ImplicitOneCases =>
    [
        ["d20", new Token[] { new(TokenType.Number, "1"), new(TokenType.Operator, "d"), new(TokenType.Number, "20"), _endToken }],
        ["(d6)", new Token[] { new(TokenType.GroupStart, "("), new(TokenType.Number, "1"), new(TokenType.Operator, "d"), new(TokenType.Number, "6"), new(TokenType.GroupEnd, ")"), _endToken }],
        ["+d6", new Token[] { new(TokenType.Operator, "+"), new(TokenType.Number, "1"), new(TokenType.Operator, "d"), new(TokenType.Number, "6"), _endToken }],
        ["xd6", new Token[] { new(TokenType.Identifier, "xd"), new(TokenType.Number, "6"), _endToken }],
        ["d%", new Token[] { new(TokenType.Number, "1"), new(TokenType.Operator, "d"), new(TokenType.Number, "100"), _endToken }],
        ["f6", new Token[] { new(TokenType.Number, "1"), new(TokenType.Operator, "f"), new(TokenType.Number, "6"), _endToken }],
        ["3f", new Token[] { new(TokenType.Number, "3"), new(TokenType.Operator, "f"), _endToken }],
        ["-d6", new Token[] { new(TokenType.Operator, "-"), new(TokenType.Number, "1"), new(TokenType.Operator, "d"), new(TokenType.Number, "6"), _endToken }],
        ["((d6)+d8)", new Token[] { new(TokenType.GroupStart, "("), new(TokenType.GroupStart, "("), new(TokenType.Number, "1"), new(TokenType.Operator, "d"), new(TokenType.Number, "6"), new(TokenType.GroupEnd, ")"), new(TokenType.Operator, "+"), new(TokenType.Number, "1"), new(TokenType.Operator, "d"), new(TokenType.Number, "8"), new(TokenType.GroupEnd, ")"), _endToken }]
    ];

    [TestMethod]
    [DynamicData(nameof(ImplicitOneCases))]
    public void Tokenize_ShouldInsertImplicitOne(string notation, object[] expected)
    {
        // arrange

        // act
        var tokens = _lexer.Tokenize(notation);

        // assert
        CollectionAssert.AreEqual(expected, tokens.ToArray());
    }
}
