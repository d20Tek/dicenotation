using System.Text.Json.Serialization;

namespace d20Tek.DiceNotation.Results;

public class DiceResult
{
    private const int _errorValue = -404;
    private static readonly TermResultListConverter Converter = new();

    public string DiceExpression { get; set; } = string.Empty;

    public string DieRollerUsed { get; set; } = string.Empty;

    public IReadOnlyList<TermResult> Results { get; set; } = [];

    public int Value { get; set; }

    public string? Error { get; set; }

    public bool HasError => Error is not null;

    [JsonIgnore]
    public string RollsDisplayText => (Results is null)
            ? string.Empty
            : $"{Converter.Convert(Results.ToList(), typeof(string), string.Empty, Constants.DefaultLocale)}";

    public DiceResult(string expression, List<TermResult> results, string rollerUsed, IDiceConfiguration config)
        : this(expression, results.Sum(CalculateResult), results, rollerUsed, config)
    {}

    public DiceResult(string expression, int value, List<TermResult> results, string roller, IDiceConfiguration config)
    {
        DiceExpression = expression;
        DieRollerUsed = roller;
        Results = [.. results];

        bool boundedResult = !expression.Contains(Constants.FudgeDiceIdentifier) && config.HasBoundedResult;
        Value = boundedResult ? Math.Max(value, config.BoundedResultMinimum) : value;
    }


    public DiceResult(string error, string expression)
    {
        DiceExpression = expression;
        Error = error;
        Value = _errorValue;
    }

    public DiceResult() { }

    private static int CalculateResult(TermResult r) =>
        (int)Math.Round(r.AppliesToResultCalculation ? r.Value * r.Scalar : 0);
}
