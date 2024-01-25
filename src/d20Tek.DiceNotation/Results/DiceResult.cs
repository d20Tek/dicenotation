﻿//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using System.Text.Json.Serialization;

namespace d20Tek.DiceNotation.Results;

public class DiceResult
{
    private const string FudgeDiceIdentifier = "f";
    private static readonly TermResultListConverter Converter = new();

    public string DiceExpression { get; set; } = string.Empty;

    public string DieRollerUsed { get; set; } = string.Empty;

    [JsonIgnore]
    public string RollsDisplayText
    {
        get
        {
            if (Results == null)
            {
                return string.Empty;
            }

            return Converter.Convert(
                Results.ToList(),
                typeof(string),
                "",
                "en-us") as string ?? string.Empty;
        }
    }

    public IReadOnlyList<TermResult> Results { get; set; } = new List<TermResult>();

    public int Value { get; set; }

    public DiceResult(
        string expression,
        List<TermResult> results,
        string rollerUsed,
        IDiceConfiguration config)
        : this(
              expression,
              results.Sum(r => (int)Math.Round(
                  r.AppliesToResultCalculation ? r.Value * r.Scalar : 0)),
              results,
              rollerUsed,
              config)
    {
    }

    public DiceResult(
        string expression,
        int value,
        List<TermResult> results,
        string rollerUsed,
        IDiceConfiguration config)
    {
        DiceExpression = expression;
        DieRollerUsed = rollerUsed;
        Results = results.ToList();

        bool boundedResult = !expression.Contains(FudgeDiceIdentifier) && config.HasBoundedResult;
        Value = boundedResult ? Math.Max(value, config.BoundedResultMinimum) : value;
    }

    public DiceResult()
    {
    }
}
