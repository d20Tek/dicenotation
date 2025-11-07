namespace d20Tek.DiceNotation.Parser.Parselets;

internal sealed class ArgParser()
{
    public Expression Parse(IParser parser) =>
        parser.Match(TokenKind.Number) ? ParseNumberExpression(parser) :
            parser.Match(TokenKind.GroupStart) ? ParseGroupExpression(parser) :
            throw parser.Error("Expected argument: Number or parenthesized expression.");

    private static GroupExpression ParseGroupExpression(IParser parser)
    {
        var groupStart = parser.Advance();
        var inner = parser.Parse(Precedence.None);
        parser.Consume(TokenKind.GroupEnd);

        return new(inner, groupStart.Pos);
    }

    private static NumberExpression ParseNumberExpression(IParser parser)
    {
        var token = parser.Advance();
        return new(token.IntValue!.Value, token.Pos);
    }

    public static bool IsArg(Expression expression) => expression is NumberExpression || expression is GroupExpression;
}
