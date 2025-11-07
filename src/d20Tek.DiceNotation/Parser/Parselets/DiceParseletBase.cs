namespace d20Tek.DiceNotation.Parser.Parselets;

internal abstract class DiceParseletBase(ModifierParser mod, ArgParser args)
{
    public virtual Expression ParseInternal(IParser parser, Expression? left, Token token)
    {
        (bool percent, Expression? sidesArg) = ParsePercentOrArguments(parser, args);
        return new DiceExpression(left, percent, sidesArg, mod.Parse(parser, args), token.Pos);
    }

    protected static (bool, Expression?) ParsePercentOrArguments(IParser parser, ArgParser args) =>
        parser.Match(TokenKind.Percent) ? ConsumePercentage(parser) : (false, args.Parse(parser));

    protected static (bool, Expression?) ConsumePercentage(IParser parser)
    {
        var t = parser.Advance();
        return (true, new NumberExpression(100, t.Pos));
    }
}
