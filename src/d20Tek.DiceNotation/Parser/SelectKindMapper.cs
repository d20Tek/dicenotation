namespace d20Tek.DiceNotation.Parser;

internal sealed class SelectKindMapper
{
    public static SelectKind FromTokenKind(TokenKind kind, Position? pos = null) =>
        kind switch
        {
            TokenKind.Keep => SelectKind.KeepHigh,
            TokenKind.Drop => SelectKind.DropLow,
            TokenKind.KeepLowest => SelectKind.KeepLow,
            _ => throw new ParseException($"Invalid selection modifier for {kind}.", pos ?? new())
        };
}
