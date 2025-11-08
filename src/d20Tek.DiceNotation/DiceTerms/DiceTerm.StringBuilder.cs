namespace d20Tek.DiceNotation.DiceTerms;

internal partial class DiceTerm
{
    public override string ToString()
    {
        var text = BuildVariableDisplayText();
        return _scalar switch
        {
            1 => string.Format(FormatDiceTermText, _numberDice, _sides, text),
            -1 => string.Format(FormatDiceTermText, -_numberDice, _sides, text),
            > 1 => string.Format(DiceTermHelper.FormatDiceMultiplyTermText, _numberDice, _sides, text, _scalar),
            _ => string.Format(DiceTermHelper.FormatDiceDivideTermText, _numberDice, _sides, text, (int)(1 / _scalar))
        };
    }

    private string BuildVariableDisplayText()
    {
        var chooseText = _choose is null || _choose == _numberDice ? string.Empty : "k" + _choose;
        var explodingText = _exploding is null ? string.Empty : "!" + _exploding;
        return chooseText + explodingText;
    }
}
