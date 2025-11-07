namespace d20Tek.DiceNotation.Parser;

internal static class Constants
{
    public const char NewLine = '\n';
    public const string WhiteSpaceGroup = "WS";

    public static class Errors
    {
        public const string UnmatchedToken = "Unexpected input (no token matched).";

        public static string UnexpectedCharacter(string ch) => $"Unexpected character '{ch}'.";
    }
}
