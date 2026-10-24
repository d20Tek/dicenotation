namespace d20Tek.DiceNotation.Blazor.Models;

/// <summary>
/// Represents a single die that has been prepared for animation. The assigned
/// <see cref="Value"/> is determined before the animation begins and is never changed
/// by the simulation.
/// </summary>
/// <param name="Id">The stable identifier used to correlate the die across snapshots.</param>
/// <param name="DieType">The die type label, such as <c>d6</c>.</param>
/// <param name="Value">The authoritative result assigned to this die.</param>
public sealed record VisualDie(string Id, string DieType, int Value);
