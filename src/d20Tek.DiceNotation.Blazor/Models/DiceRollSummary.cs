namespace d20Tek.DiceNotation.Blazor.Models;

/// <summary>
/// Represents a completed roll prepared for display alongside the animation. The
/// <see cref="Total"/> is the value reported by the notation library and is never
/// recomputed from <see cref="Dice"/>, because notation features such as dropped dice,
/// scalars, and bounded results mean the visible faces do not always sum to the result.
/// </summary>
/// <param name="Dice">Every die in the roll, in the order the notation produced them.</param>
/// <param name="Modifier">The combined value of the roll's constant terms.</param>
/// <param name="Total">The authoritative total reported by the notation library.</param>
/// <param name="Expression">The notation that produced this roll.</param>
public sealed record DiceRollSummary(
    IReadOnlyList<VisualDie> Dice,
    int Modifier,
    int Total,
    string Expression)
{
    /// <summary>Gets an empty summary used before any roll has been made.</summary>
    public static DiceRollSummary Empty { get; } = new([], 0, 0, string.Empty);
}
