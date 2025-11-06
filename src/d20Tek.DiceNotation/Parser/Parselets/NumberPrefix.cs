namespace d20Tek.DiceNotation.Parser.Parselets;

internal sealed class NumberPrefix : IPrefixParselet
{
    public Expression Parse(IParser parser, Token token) =>
        new NumberExpression(token.IntValue!.Value, token.Pos);
}
