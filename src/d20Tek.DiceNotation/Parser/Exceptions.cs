namespace d20Tek.DiceNotation.Parser;

internal sealed class ParseException(string message, Position pos) : Exception($"{message} @ {pos}")
{
    public Position Position { get; } = pos;
}

internal sealed class EvalException(string message) : Exception(message)
{
}
