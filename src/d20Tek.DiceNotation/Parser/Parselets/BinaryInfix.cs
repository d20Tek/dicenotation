namespace d20Tek.DiceNotation.Parser.Parselets;

internal sealed class BinaryInfix(BinaryOperator op, int precedence) : IInfixParselet
{
    public int Precedence => precedence;

    public Expression Parse(IParser parser, Expression left, Token token)
        => new BinaryExpression(left, op, parser.Parse(precedence), token.Pos);
}
