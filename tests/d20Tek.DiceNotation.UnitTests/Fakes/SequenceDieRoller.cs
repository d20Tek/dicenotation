namespace d20Tek.DiceNotation.UnitTests.Fakes;

[ExcludeFromCodeCoverage]
internal sealed class SequenceDieRoller(params int[] values) : IDieRoller
{
    private readonly int[] _values = values;
    private int _index;

    public int Roll(int sides, int? factor = null)
    {
        var value = _values[Math.Min(_index, _values.Length - 1)];
        _index++;
        return value;
    }
}
