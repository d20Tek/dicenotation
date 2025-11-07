namespace d20Tek.DiceNotation.Parser.Parselets;

internal sealed class DicePrefix(ModifierParser mod, ArgParser args) :
    DiceParseletBase(mod, args), IPrefixParselet
{
    public Expression Parse(IParser parser, Token token) => ParseInternal(parser, null, token);
}
