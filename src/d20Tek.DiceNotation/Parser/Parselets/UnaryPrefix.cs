namespace d20Tek.DiceNotation.Parser.Parselets;

internal sealed class UnaryPrefix(UnaryOperator op) : IPrefixParselet
{
    public Expression Parse(IParser parser, Token token)
        => new UnaryExpression(op, parser.Parse(Precedence.Unary), token.Pos);
}
