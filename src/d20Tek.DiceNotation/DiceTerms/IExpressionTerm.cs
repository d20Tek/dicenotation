//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.DiceTerms;

public interface IExpressionTerm
{
    IReadOnlyList<TermResult> CalculateResults(IDieRoller dieRoller);
}
