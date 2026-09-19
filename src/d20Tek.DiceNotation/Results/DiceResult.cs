using System.Text.Json.Serialization;

namespace d20Tek.DiceNotation.Results;

/// <summary>
/// Represents the outcome of rolling a dice expression, including the total value and individual term results.
/// </summary>
public class DiceResult
{
    private const int _errorValue = -404;
    private static readonly TermResultListConverter Converter = new();

    /// <summary>
    /// Gets or sets the dice expression that produced this result.
    /// </summary>
    public string DiceExpression { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the die roller type that was used.
    /// </summary>
    public string DieRollerUsed { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the individual term results that make up this dice result.
    /// </summary>
    public IReadOnlyList<TermResult> Results { get; set; } = [];

    /// <summary>
    /// Gets or sets the total calculated value of the roll.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Gets or sets the error message describing why the roll failed, if any.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets a value indicating whether this result represents an error.
    /// </summary>
    public bool HasError => Error is not null;

    /// <summary>
    /// Gets the display text describing the individual dice rolls.
    /// </summary>
    [JsonIgnore]
    public string RollsDisplayText => (Results is null)
            ? string.Empty
            : $"{Converter.Convert(Results.ToList(), typeof(string), string.Empty, Constants.DefaultLocale)}";

    /// <summary>
    /// Initializes a new instance of the <see cref="DiceResult"/> class, calculating the value from the results.
    /// </summary>
    /// <param name="expression">The dice expression that produced this result.</param>
    /// <param name="results">The individual term results.</param>
    /// <param name="rollerUsed">The name of the die roller type used.</param>
    /// <param name="config">The configuration that controls default dice behavior.</param>
    public DiceResult(string expression, List<TermResult> results, string rollerUsed, IDiceConfiguration config)
        : this(expression, results.Sum(CalculateResult), results, rollerUsed, config)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="DiceResult"/> class with a precalculated value.
    /// </summary>
    /// <param name="expression">The dice expression that produced this result.</param>
    /// <param name="value">The total calculated value of the roll.</param>
    /// <param name="results">The individual term results.</param>
    /// <param name="roller">The name of the die roller type used.</param>
    /// <param name="config">The configuration that controls default dice behavior.</param>
    public DiceResult(string expression, int value, List<TermResult> results, string roller, IDiceConfiguration config)
    {
        DiceExpression = expression;
        DieRollerUsed = roller;
        Results = [.. results];

        bool boundedResult = !expression.Contains(Constants.FudgeDiceIdentifier) && config.HasBoundedResult;
        Value = boundedResult ? Math.Max(value, config.BoundedResultMinimum) : value;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="DiceResult"/> class that represents an error.
    /// </summary>
    /// <param name="error">The error message describing the failure.</param>
    /// <param name="expression">The dice expression that produced the error.</param>
    public DiceResult(string error, string expression)
    {
        DiceExpression = expression;
        Error = error;
        Value = _errorValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiceResult"/> class.
    /// </summary>
    public DiceResult() { }

    private static int CalculateResult(TermResult r) =>
        (int)Math.Round(r.AppliesToResultCalculation ? r.Value * r.Scalar : 0);
}
