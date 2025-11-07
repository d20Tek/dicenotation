namespace d20Tek.DiceNotation.Parser;


internal readonly record struct Position(int Index, int Line, int Column)
{
    public Position(int index) : this(index, 1, index + 1) { }

    public override string ToString() => Constants.Position(Index, Line, Column);
}
