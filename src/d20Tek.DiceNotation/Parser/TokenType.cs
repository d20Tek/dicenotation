namespace d20Tek.DiceNotation.Parser;

internal enum TokenType
{
    StartOfInput,
    Number,
    Identifier,
    Operator,
    GroupStart,
    GroupEnd,
    EndOfInput
}
