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

    public IReadOnlyList<TermResult> Results { get; set; } = [];

    public int Value { get; set; }

    [JsonIgnore]
    public string RollsDisplayText =>
        (Results == null) ?
            string.Empty :
            Converter.Convert(Results.ToList(), typeof(string), string.Empty, "en-us").ToString()!;

    public DiceResult(string expression, List<TermResult> results, string rollerUsed, IDiceConfiguration config)
        : this(
              expression,
              results.Sum(r => (int)Math.Round(
                  r.AppliesToResultCalculation ? r.Value * r.Scalar : 0)),
              results,
              rollerUsed,
              config)
    {
    }

    public DiceResult(string expression, int value, List<TermResult> results, string roller, IDiceConfiguration config)
    {
        DiceExpression = expression;
        DieRollerUsed = roller;
        Results = [.. results];

        bool boundedResult = !expression.Contains(FudgeDiceIdentifier) && config.HasBoundedResult;
        Value = boundedResult ? Math.Max(value, config.BoundedResultMinimum) : value;
    }

    public DiceResult()
    {
    }
}
