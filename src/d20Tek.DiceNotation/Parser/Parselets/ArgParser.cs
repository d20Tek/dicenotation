namespace d20Tek.DiceNotation.Parser.Parselets;

internal sealed class ArgParser()
{
    public Expression Parse(IParser parser)
    {
        if (parser.Match(TokenKind.Number))
        {
            var token = parser.Advance();
            return new NumberExpression(token.IntValue!.Value, token.Pos);
        }

        if (parser.Match(TokenKind.GroupStart))
        {
            var groupStart = parser.Advance();
            var inner = parser.Parse(Precedence.None);
            parser.Consume(TokenKind.GroupEnd);
            return new GroupExpression(inner, groupStart.Pos);
        }

        throw parser.Error("Expected argument: Number or parenthesized expression.");
    }

    public static bool IsArg(Expression expression) => expression is NumberExpression || expression is GroupExpression;
}
