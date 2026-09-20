namespace d20Tek.DiceNotation;

/// <summary>
/// Defines a mechanism for rolling a single die and producing a numeric result.
/// </summary>
public interface IDieRoller
{
    /// <summary>
    /// Rolls a single die with the specified number of sides.
    /// </summary>
    /// <param name="sides">The number of sides on the die.</param>
    /// <param name="factor">
    /// An optional factor applied to the roll, or <see langword="null"/> when no factor is used.
    /// </param>
    /// <returns>The value produced by the die roll.</returns>
    int Roll(int sides, int? factor = null);
}
