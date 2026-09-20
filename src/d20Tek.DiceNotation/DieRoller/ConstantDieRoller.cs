namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// A die roller that always returns a fixed, constant value. Useful for testing.
/// </summary>
/// <param name="rollValue">The constant value returned by every roll.</param>
public class ConstantDieRoller(int rollValue = ConstantDieRoller.DefaultRollValue) : IDieRoller
{
    private const int DefaultRollValue = 1;
    private readonly int constantRollValue = rollValue;

    /// <inheritdoc />
    public int Roll(int sides, int? factor = null) => constantRollValue;
}
