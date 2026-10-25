namespace d20Tek.DiceNotation.Blazor.UnitTests.Fakes;

/// <summary>
/// Returns a predetermined sequence of die values so that rolls made through the real
/// notation engine are deterministic. The final value repeats once the sequence is spent.
/// </summary>
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
