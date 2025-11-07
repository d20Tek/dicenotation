namespace d20Tek.DiceNotation.Parser.Parselets;

internal sealed class DiceInfix(ModifierParser mod, ArgParser args, int precedence) :
    DiceParseletBase(mod, args), IInfixParselet
{
    public int Precedence => precedence;

    public Expression Parse(IParser parser, Expression left, Token diceToken)
    {
        ParseException.ThrowIfFalse(
            ArgParser.IsArg(left),
            "Dice count must be a Number or parenthesized expression.",
            diceToken.Pos);

        return ParseInternal(parser, left, diceToken);
    }
}
