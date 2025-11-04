namespace d20Tek.DiceNotation.Parser;


internal readonly record struct Position(int Index, int Line, int Column)
{
    public override string ToString() => $"L:{Line},C:{Column}";
}
