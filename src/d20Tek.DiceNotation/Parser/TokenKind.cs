namespace d20Tek.DiceNotation.Parser;


internal enum TokenKind
{
    StartOfInput,
    Number,
    Plus,
    Minus,
    Star,
    Times,
    Divide,
    GroupStart,
    GroupEnd,
    Dice,
    FudgeDice,
    Percent,
    Exploding,
    Keep,
    Drop,
    KeepLowest,
    EndOfInput
}
