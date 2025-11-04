namespace d20Tek.DiceNotation.Parser;

internal sealed class ParseException(string message, Position pos) : Exception($"{message} @ {pos}")
{
    public Position Position { get; } = pos;

    public static void ThrowIfFalse(bool condition, string message, Position pos)
    {
        if (condition is false) throw new ParseException(message, pos);
    }
}

internal sealed class EvalException(string message) : Exception(message)
{
}
