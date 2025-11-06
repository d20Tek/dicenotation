namespace d20Tek.DiceNotation.Parser.Parselets;

internal sealed class DiceInfix(ModifierParser mod, ArgParser args, int precedence) : IInfixParselet
{
    public int Precedence => precedence;

    public Expression Parse(IParser parser, Expression left, Token diceToken)
    {
        ParseException.ThrowIfFalse(
            ArgParser.IsArg(left),
            "Dice count must be a Number or parenthesized expression.",
            diceToken.Pos);

        // sides: '%' | arg
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

        return new DiceExpression(left, percent, sidesArg, mod.Parse(parser, args), diceToken.Pos);
    }
}
