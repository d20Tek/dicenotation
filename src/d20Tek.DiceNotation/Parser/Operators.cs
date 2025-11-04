namespace d20Tek.DiceNotation.Parser;

internal enum BinaryOperator
{
    Add,
    Subtract,
    Multiply,
    Divide
}

internal enum UnaryOperator 
{
    Positive,
    Negative
}

internal enum SelectKind 
{
    KeepHigh,
    DropLow,
    KeepLow
}
