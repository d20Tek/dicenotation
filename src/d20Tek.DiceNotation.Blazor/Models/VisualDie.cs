namespace d20Tek.DiceNotation.Blazor.Models;

using d20Tek.DiceNotation.Results;

/// <summary>
/// Represents a single die that has been prepared for animation. The assigned
/// <see cref="Value"/> is determined before the animation begins and is never changed
/// by the simulation.
/// </summary>
/// <param name="Id">The stable identifier used to correlate the die across snapshots.</param>
/// <param name="DieType">The die type label, such as <c>d6</c>.</param>
/// <param name="Value">The authoritative result assigned to this die.</param>
/// <param name="Sides">
/// The number of faces on this die. The renderer uses this to churn through plausible
/// faces while the die is tumbling, so it must describe the die itself rather than being
/// inferred from <paramref name="Value"/>.
/// </param>
/// <param name="Roles">
/// The role or roles this die played within its term, such as being kept, dropped,
/// exploded, or rerolled. Every die from the roll is animated, so this is what lets a
/// caller distinguish a counted die from one the notation discarded.
/// </param>
/// <param name="MinValue">
/// The lowest face value on this die. Standard dice start at one, but fudge dice run from
/// minus one to plus one, so the face range cannot be assumed from <paramref name="Sides"/>.
/// </param>
public sealed record VisualDie(
    string Id,
    string DieType,
    int Value,
    int Sides = 6,
    DieRollRole Roles = DieRollRole.Kept,
    int MinValue = 1);
