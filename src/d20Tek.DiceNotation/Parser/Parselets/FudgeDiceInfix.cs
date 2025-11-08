namespace d20Tek.DiceNotation.Parser.Parselets;

internal sealed class FudgeDiceInfix(ModifierParser mod, ArgParser args, int precedence) : IInfixParselet
{
    public int Precedence => precedence;

    public Expression Parse(IParser parser, Expression left, Token fudgeToken)
    {
        ParseException.ThrowIfFalse(ArgParser.IsArg(left), Constants.Errors.FudgDiceNumber, fudgeToken.Pos);
        return new FudgeExpression(left, mod.Parse(parser, args), fudgeToken.Pos);
    }
}
