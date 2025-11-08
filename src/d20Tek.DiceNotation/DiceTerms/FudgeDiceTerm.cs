namespace d20Tek.DiceNotation.DiceTerms;

internal class FudgeDiceTerm : DiceTerm
{
    private const string FudgeFormatResultType = "{0}.dF";
    private const string FudgeFormatDiceTermText = "{0}f{2}";
    private const int FudgeNumberSides = 3;
    private const int FudgeFactor = -2;

    public FudgeDiceTerm(int numberDice, int? choose = null) : base(numberDice, FudgeNumberSides, 1, choose)
    {
        FormatResultType = FudgeFormatResultType;
        FormatDiceTermText = FudgeFormatDiceTermText;
    }

    protected override int RollTerm(IDieRoller dieRoller, int sides) => dieRoller.Roll(sides, FudgeFactor);
}
