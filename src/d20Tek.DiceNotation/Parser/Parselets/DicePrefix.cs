namespace d20Tek.DiceNotation.Parser.Parselets;

internal sealed class DicePrefix(ModifierParser mod, ArgParser args) : IPrefixParselet
{
    public Expression Parse(IParser parser, Token token)
    {
        bool percent;
        Expression? sidesArg = null;

        if (parser.Match(TokenKind.Percent))
        {
            parser.Advance();
            percent = true;
        }
        else
        {
            sidesArg = args.Parse(parser);
            percent = false;
        }

        var m = mod.Parse(parser, args);
        return new DiceExpression(null, percent, sidesArg, m, token.Pos);
    }
}
