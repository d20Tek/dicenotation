namespace d20Tek.DiceNotation.Parser;

internal static class Constants
{
    public const string PercentileNotation = "d%";
    public const string D100EquivalentNotation = "d100";
    public const string DefaultOperator = "x";
    public const string GroupStartOperator = "(";
    public const string GroupEndOperator = ")";
    public const string DefaultNumDice = "1";
    public const string DiceOperator = "d";
    public const string FudgeDiceOperator = "f";

    public static readonly HashSet<string> Operators = ["d", "f", "k", "p", "l", "!", "/", "x", "*", "-", "+"];
    public static Token DefaultDiceNumberToken = new(TokenType.Number, "1");
    public static Token EndToken = new(TokenType.EndOfInput, "");
}
