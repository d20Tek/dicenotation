namespace d20Tek.DiceNotation.Parser;

internal static class Constants
{
    public const int PercentValue = 100;
    public const int DefaultScalar = 1;
    public const int DefaultNumDice = 1;
    public const int DefaultDiceSides = 6;
    public const char NewLine = '\n';
    public const string WhiteSpaceGroup = "WS";

    public static string Position(int i, int l, int c) => $"I:{i},L:{l},C:{c}";

    public static class Errors
    {
        public const string UnmatchedToken = "Unexpected input (no token matched).";
        public const string UnexpectedArgument = "Expected argument: Number or parenthesized expression.";
        public const string DiceNumber = "Dice count must be a Number or parenthesized expression.";
        public const string FudgDiceNumber = "Fudge count must be a Number or parenthesized expression.";
        public const string DivideByZero = "Division by zero is not allowed.";

        public static string UnexpectedCharacter(string ch) => $"Unexpected character '{ch}'.";

        public static string UnexpectedTokenKind(TokenKind expected, TokenKind actual) =>
            $"Expected token of kind {expected}, but actual kind - {actual}.";

        public static string UnexpectedSelectKind(TokenKind kind) => $"Invalid selection modifier for {kind}.";

        public static string MissingPrefixParser(TokenKind kind) => $"Unexpected token {kind} for prefix parsers.";

        public static string MissingInfixParser(TokenKind kind) => $"Unexpected token {kind} in infix parsers.";

        public static string UnknownExpression(Expression expr) => $"Unknown expression type: {expr.GetType().Name}.";

        public static string UnknownBinaryOperator(BinaryOperator op) => $"Unknown binary operator: {op}.";

        public static string UnknownException(string msg) => $"Unexpected error: {msg}.";

        public static string ParseException(string msg, Position pos) => $"Parse error: {msg} @({pos})";

        public static string EvalException(string msg) => $"Evaluation error: {msg}";
    }
}
