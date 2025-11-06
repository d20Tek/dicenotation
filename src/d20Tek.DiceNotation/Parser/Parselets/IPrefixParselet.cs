namespace d20Tek.DiceNotation.Parser.Parselets;

internal interface IPrefixParselet
{
    Expression Parse(IParser parser, Token token);
}
