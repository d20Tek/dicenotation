using d20Tek.DiceNotation.Parser.Parselets;

namespace d20Tek.DiceNotation.Parser;

internal sealed class PrattParser(TokenCursor cursor) : IParser
{
    private static readonly ModifierParser _modParser = new();
    private static readonly ArgParser _argParser = new();
    private static readonly Dictionary<TokenKind, IPrefixParselet> _prefixParselets = new()
    {
        { TokenKind.Number, new NumberPrefix() },
        { TokenKind.GroupStart, new GroupPrefix() },
        { TokenKind.Plus, new UnaryPrefix(UnaryOperator.Positive) },
        { TokenKind.Minus, new UnaryPrefix(UnaryOperator.Negative) },
        { TokenKind.Dice, new DicePrefix(_modParser, _argParser) },
        { TokenKind.FudgeDice, new FudgeDicePrefix(_modParser, _argParser) }
    };

    private static readonly Dictionary<TokenKind, IInfixParselet> _infixParselets = new()
    {
        { TokenKind.Plus, new BinaryInfix(BinaryOperator.Add, Precedence.Get(TokenKind.Plus)) },
        { TokenKind.Minus, new BinaryInfix(BinaryOperator.Subtract, Precedence.Get(TokenKind.Minus)) },
        { TokenKind.Star, new BinaryInfix(BinaryOperator.Multiply, Precedence.Get(TokenKind.Star)) },
        { TokenKind.Times, new BinaryInfix(BinaryOperator.Multiply, Precedence.Get(TokenKind.Times)) },
        { TokenKind.Divide, new BinaryInfix(BinaryOperator.Divide, Precedence.Get(TokenKind.Divide)) },
        { TokenKind.Dice, new DiceInfix(_modParser, _argParser, Precedence.Get(TokenKind.Dice)) },
        { TokenKind.FudgeDice, new FudgeDiceInfix(_modParser, _argParser, Precedence.Get(TokenKind.FudgeDice)) },
    };

    private readonly TokenCursor _cursor = cursor;

    public Token Current => _cursor.Current;

    public Expression ParseExpression()
    {
        var expr = Parse(Precedence.None);
        Current.EnsureExpectedKind(TokenKind.EndOfInput);
        return expr;
    }

    public Expression Parse(int rightPrec)
    {
        var token = Advance();
        var prefixParselet = GetPrefixParselet(token.Kind);
        var left = prefixParselet.Parse(this, token);

        while (rightPrec < GetInfixPrecedence())
        {
            var opToken = Advance();
            var infix = GetInfixParselet(opToken.Kind);
            left = infix.Parse(this, left, opToken);
        }
        return left;
    }

    private IPrefixParselet GetPrefixParselet(TokenKind kind) =>
        _prefixParselets.TryGetValue(kind, out var prefix) 
            ? prefix
            : throw Error($"Unexpected token {kind} for prefix parsers.");

    private IInfixParselet GetInfixParselet(TokenKind kind) =>
        _infixParselets.TryGetValue(kind, out var infix)
            ? infix
            : throw Error($"Unexpected token {kind} in infix parsers.");

    private int GetInfixPrecedence() => 
        _infixParselets.TryGetValue(_cursor.Current.Kind, out var parselet) ? parselet.Precedence : 0;

    public bool Match(TokenKind k) => _cursor.Match(k);

    public Token Advance() => _cursor.Advance();

    public void Consume(TokenKind k) => _cursor.Consume(k);

    public ParseException Error(string message) => new(message, _cursor.Current.Pos);
}
