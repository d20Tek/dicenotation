namespace d20Tek.DiceNotation.Parser;

/// <summary>
/// The exception thrown when dice notation cannot be parsed.
/// </summary>
public sealed class ParseException : Exception
{
    private readonly Position _pos;

    /// <summary>
    /// Gets the position within the notation where the parse error occurred.
    /// </summary>
    public string Position => _pos.ToString();

    internal ParseException(string message, Position pos)
        : base(Constants.Errors.ParseException(message, pos)) =>
        _pos = pos;

    internal static void ThrowIfFalse(bool condition, string message, Position pos)
    {
        if (condition is false) throw new ParseException(message, pos);
    }
}

/// <summary>
/// The exception thrown when a parsed dice expression cannot be evaluated.
/// </summary>
/// <param name="message">The message that describes the evaluation error.</param>
public sealed class EvalException(string message) : Exception(Constants.Errors.EvalException(message))
{
}
