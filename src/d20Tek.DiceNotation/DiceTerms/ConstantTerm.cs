//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.DiceTerms;

public class ConstantTerm : IExpressionTerm
{
    private readonly int _constant = 0;

    public ConstantTerm(int constant)
    {
        _constant = constant;
    }

    public IReadOnlyList<TermResult> CalculateResults(IDieRoller dieRoller)
    {
        List<TermResult> results =
        [
            new TermResult
            {
                Scalar = 1,
                Value = _constant,
                Type = GetType().Name,
            },
        ];

        return results.AsReadOnly();
    }

    public override string ToString() => _constant.ToString();
}
