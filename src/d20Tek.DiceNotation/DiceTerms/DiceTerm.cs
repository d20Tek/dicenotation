//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.DiceTerms;

public class DiceTerm : IExpressionTerm
{
    private const string DiceFormatResultType = "{0}.d{1}";
    private const string DiceFormatDiceTermText = "{0}d{1}{2}";
    private const string FormatDiceMultiplyTermText = "{0}d{1}{2}x{3}";
    private const string FormatDiceDivideTermText = "{0}d{1}{2}/{3}";
    private const int MaxRerollsAllowed = 1000;

    private readonly int _numberDice;
    private readonly int _sides;
    private readonly double _scalar;
    private readonly int? _choose;
    private readonly int? _exploding;

    protected string FormatResultType { get; set; } = DiceFormatResultType;

    protected string FormatDiceTermText { get; set; } = DiceFormatDiceTermText;

    public DiceTerm(
        int numberDice,
        int sides,
        double scalar = 1,
        int? choose = null,
        int? exploding = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(numberDice, 0, nameof(numberDice));
        ArgumentOutOfRangeException.ThrowIfLessThan(sides, 1, nameof(sides));
        ArgumentOutOfRangeException.ThrowIfEqual(scalar, 0, nameof(scalar));

        if (choose is not null)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(choose.Value, numberDice, nameof(choose));
            ArgumentOutOfRangeException.ThrowIfLessThan(choose.Value, -numberDice, nameof(choose));
            ArgumentOutOfRangeException.ThrowIfEqual(choose.Value, 0, nameof(choose));
        }

        if (exploding != null)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(exploding.Value, -numberDice, nameof(exploding));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(exploding.Value, sides, nameof(exploding));
        }

        _numberDice = numberDice;
        _sides = sides;
        _scalar = scalar;
        _choose = choose;
        _exploding = exploding;
    }

    public IReadOnlyList<TermResult> CalculateResults(IDieRoller dieRoller)
    {
        // ensure we have a die roller.
        ArgumentNullException.ThrowIfNull(dieRoller);

        List<TermResult> results = [];
        string termType = string.Format(FormatResultType, GetType().Name, _sides);
        int rerolls = 0;

        // go through the number of dice and roll each one, saving them as term results.
        for (int i = 0; i < _numberDice + rerolls; i++)
        {
            int value = RollTerm(dieRoller, _sides);
            if (_exploding != null && value >= _exploding)
            {
                if (rerolls > MaxRerollsAllowed)
                {
                    throw new OverflowException(
                        "Rolling dice past the maximum allowed number of rerolls.");
                }

                rerolls++;
            }

            results.Add(new TermResult
            {
                Scalar = _scalar,
                Value = value,
                Type = termType,
            });
        }

        // order by their value (high to low) and only take the amount specified in choose.
        int tempChoose = _choose ?? results.Count;
        var ordered = tempChoose > 0 ?
                        results.OrderByDescending(d => d.Value).ToList() :
                        results.OrderBy(d => d.Value).ToList();

        for (int i = Math.Abs(tempChoose); i < ordered.Count; i++)
        {
            ordered[i].AppliesToResultCalculation = false;
        }

        return results;
    }

    public override string ToString()
    {
        string variableText = _choose == null || _choose == _numberDice ?
            string.Empty :
            "k" + _choose;

        variableText += _exploding == null ? string.Empty : "!" + _exploding;
        string result;

        if (_scalar == 1)
        {
            result = string.Format(FormatDiceTermText, _numberDice, _sides, variableText);
        }
        else if (_scalar == -1)
        {
            result = string.Format(FormatDiceTermText, -_numberDice, _sides, variableText);
        }
        else if (_scalar > 1)
        {
            result = string.Format(
                FormatDiceMultiplyTermText,
                _numberDice,
                _sides,
                variableText,
                _scalar);
        }
        else
        {
            result = string.Format(
                FormatDiceDivideTermText,
                _numberDice,
                _sides,
                variableText,
                (int)(1 / _scalar));
        }

        return result;
    }

    protected virtual int RollTerm(IDieRoller dieRoller, int sides)
    {
        return dieRoller.Roll(sides);
    }
}
