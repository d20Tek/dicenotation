namespace d20Tek.DiceNotation.Parser.Parselets;

internal sealed class FudgeDicePrefix(ModifierParser mod, ArgParser args) : IPrefixParselet
{
    public Expression Parse(IParser parser, Token fudgeToken) => 
        new FudgeExpression(null, mod.Parse(parser, args), fudgeToken.Pos);
}
