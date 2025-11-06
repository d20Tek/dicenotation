namespace d20Tek.DiceNotation.Parser.Parselets;

internal interface IInfixParselet
{
    int Precedence { get; }

    Expression Parse(IParser parser, Expression left, Token op);
}
