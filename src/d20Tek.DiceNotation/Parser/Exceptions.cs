namespace d20Tek.DiceNotation.Parser;

public sealed class ParseException : Exception
{
    private readonly Position _pos;

    public string Position => _pos.ToString();

    internal ParseException(string message, Position pos)
        : base($"{message} @({pos})") => 
        _pos = pos;

    internal static void ThrowIfFalse(bool condition, string message, Position pos)
    {
        if (condition is false) throw new ParseException(message, pos);
    }
}

public sealed class EvalException(string message) : Exception(message)
{
}
