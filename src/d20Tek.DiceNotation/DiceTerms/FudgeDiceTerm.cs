namespace d20Tek.DiceNotation.DiceTerms;

internal class FudgeDiceTerm : DiceTerm
{
    public FudgeDiceTerm(int numberDice, int? choose = null) : base(numberDice, Constants.FudgeNumberSides, 1, choose)
    {
        FormatResultType = Constants.FudgeFormatResultType;
        FormatDiceTermText = Constants.FudgeFormatDiceTermText;
    }

    protected override int RollTerm(IDieRoller dieRoller, int sides) => dieRoller.Roll(sides, Constants.FudgeFactor);
}
