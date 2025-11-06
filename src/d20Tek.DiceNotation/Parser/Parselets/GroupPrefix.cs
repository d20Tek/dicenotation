namespace d20Tek.DiceNotation.Parser.Parselets;

internal sealed class GroupPrefix : IPrefixParselet
{
    public Expression Parse(IParser parser, Token token)
    {
        var inner = parser.Parse(Precedence.None);
        parser.Consume(TokenKind.GroupEnd);
        return new GroupExpression(inner, token.Pos);
    }
}