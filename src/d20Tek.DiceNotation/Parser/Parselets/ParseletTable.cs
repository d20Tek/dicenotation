namespace d20Tek.DiceNotation.Parser.Parselets;

internal class ParseletTable
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

    public IPrefixParselet GetPrefix(Token token) =>
        _prefixParselets.TryGetValue(token.Kind, out var prefix)
            ? prefix
            : throw new ParseException(Constants.Errors.MissingPrefixParser(token.Kind), token.Pos);

    public IInfixParselet GetInfix(Token token) =>
        _infixParselets.TryGetValue(token.Kind, out var infix)
            ? infix
            : throw new ParseException(Constants.Errors.MissingInfixParser(token.Kind), token.Pos);

    public int GetInfixPrecedence(Token token) =>
        _infixParselets.TryGetValue(token.Kind, out var parselet) ? parselet.Precedence : 0;
}
