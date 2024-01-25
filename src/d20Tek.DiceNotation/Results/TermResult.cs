//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation.Results;

public class TermResult
{
    public double Scalar { get; set; }

    public int Value { get; set; }

    public string Type { get; set; } = string.Empty;

    public bool AppliesToResultCalculation { get; set; } = true;
}
