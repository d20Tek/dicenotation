namespace d20Tek.DiceNotation.Parser;

internal static class Precedence
{
    private static readonly Dictionary<TokenKind, int> _precedenceTable = new()
    {
        { TokenKind.GroupStart, 40 },
        { TokenKind.GroupEnd, 40 },
        { TokenKind.Dice, 40 },
        { TokenKind.FudgeDice, 40 },
        { TokenKind.Star, 20 },
        { TokenKind.Times, 20 },
        { TokenKind.Divide, 20 },
        { TokenKind.Plus, 10 },
        { TokenKind.Minus, 10 },
        { TokenKind.Number, 0 },
        { TokenKind.Percent, 0 },
    };

    public const int None = 0;

    public const int Unary = 35;

    public static int Get(TokenKind kind) => _precedenceTable.TryGetValue(kind, out var result) ? result : 0;
}
