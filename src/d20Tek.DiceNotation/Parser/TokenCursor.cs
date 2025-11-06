namespace d20Tek.DiceNotation.Parser;

internal sealed class TokenCursor(Lexer lexer)
{
    private readonly Lexer _lex = lexer;

    public Token Current { get; private set; } = lexer.GetNextToken();

    public bool Match(TokenKind k) => Current.Kind == k;

    public Token Advance()
    {
        var t = Current;
        Current = _lex.GetNextToken();
        return t;
    }

    public void Consume(TokenKind expected)
    {
        Current.EnsureExpectedKind(expected);
        Advance();
    }
}
