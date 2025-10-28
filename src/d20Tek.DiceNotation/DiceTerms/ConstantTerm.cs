//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.DiceTerms;

public class ConstantTerm(int constant) : IExpressionTerm
{
    private readonly int _constant = constant;

    public IReadOnlyList<TermResult> CalculateResults(IDieRoller dieRoller) =>
        [ new() { Scalar = 1, Value = _constant, Type = GetType().Name, } ];

    public override string ToString() => _constant.ToString();
}
