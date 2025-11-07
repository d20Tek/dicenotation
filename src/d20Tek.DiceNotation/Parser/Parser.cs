using d20Tek.DiceNotation.Parser.Parselets;

namespace d20Tek.DiceNotation.Parser;

internal sealed class Parser(TokenCursor cursor) : IParser
{
    private static readonly ParseletTable _parselets = new();
    private readonly TokenCursor _cursor = cursor;

    public Token Current => _cursor.Current;

    public Parser(Lexer lexer) : this(new TokenCursor(lexer)) { }

    public Expression ParseExpression()
    {
        var expr = Parse(Precedence.None);
        Current.EnsureExpectedKind(TokenKind.EndOfInput);
        return expr;
    }

    public Expression Parse(int rightPrecedence)
    {
        var token = Advance();
        var prefixParselet = _parselets.GetPrefix(token);
        var left = prefixParselet.Parse(this, token);

        while (rightPrecedence < _parselets.GetInfixPrecedence(_cursor.Current))
        {
            var opToken = Advance();
            var infix = _parselets.GetInfix(opToken);
            left = infix.Parse(this, left, opToken);
        }
        return left;
    }

    public bool Match(TokenKind kind) => _cursor.Match(kind);

    public Token Advance() => _cursor.Advance();

    public void Consume(TokenKind kind) => _cursor.Consume(kind);

    public ParseException Error(string message) => new(message, _cursor.Current.Pos);
}
