namespace d20Tek.DiceNotation.Parser;

internal static class Constants
{
    public const char NewLine = '\n';
    public const string WhiteSpaceGroup = "WS";

    public static string Position(int i, int l, int c) => $"I:{i},L:{l},C:{c}";

    public static class Errors
    {
        public const string UnmatchedToken = "Unexpected input (no token matched).";
        public const string UnexpectedArgument = "Expected argument: Number or parenthesized expression.";
        public const string DiceNumber = "Dice count must be a Number or parenthesized expression.";
        public const string FudgDiceNumber = "Fudge count must be a Number or parenthesized expression.";

        public static string UnexpectedCharacter(string ch) => $"Unexpected character '{ch}'.";

        public static string UnexpectedTokenKind(TokenKind expected, TokenKind actual) =>
            $"Expected token of kind {expected}, but actual kind - {actual}.";

        public static string UnexpectedSelectKind(TokenKind kind) => $"Invalid selection modifier for {kind}.";

        public static string MissingPrefixParser(TokenKind kind) => $"Unexpected token {kind} for prefix parsers.";

        public static string MissingInfixParser(TokenKind kind) => $"Unexpected token {kind} in infix parsers.";
    }
}
