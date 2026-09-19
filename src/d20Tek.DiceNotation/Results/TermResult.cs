namespace d20Tek.DiceNotation.Results;

/// <summary>
/// Represents the result of evaluating a single term within a dice expression.
/// </summary>
public class TermResult
{
    /// <summary>
    /// Gets or sets the scalar multiplier applied to this term's value.
    /// </summary>
    public double Scalar { get; set; }

    /// <summary>
    /// Gets or sets the value produced by the term.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Gets or sets the type identifier of the term that produced this result.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this result contributes to the overall calculation.
    /// </summary>
    public bool AppliesToResultCalculation { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="TermResult"/> class.
    /// </summary>
    /// <param name="scalar">The scalar multiplier applied to the value.</param>
    /// <param name="value">The value produced by the term.</param>
    /// <param name="type">The type identifier of the term.</param>
    public TermResult(double scalar, int value, string type) => (Scalar, Value, Type) = (scalar, value, type);

    /// <summary>
    /// Initializes a new instance of the <see cref="TermResult"/> class.
    /// </summary>
    public TermResult() { }
}
