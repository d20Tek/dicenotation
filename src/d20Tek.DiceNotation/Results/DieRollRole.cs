namespace d20Tek.DiceNotation.Results;

/// <summary>
/// Describes the role or roles a single die roll played within a dice term result. The values are
/// combinable as flags, so a die can hold more than one role (for example, a die that exploded and
/// was then dropped by a keep/drop selection).
/// </summary>
[Flags]
public enum DieRollRole
{
    /// <summary>
    /// The die has no special role assigned.
    /// </summary>
    None = 0,

    /// <summary>
    /// The die was kept and contributes to the overall calculation.
    /// </summary>
    Kept = 1,

    /// <summary>
    /// The die was dropped by a keep-highest or drop-lowest selection and does not contribute to the
    /// overall calculation.
    /// </summary>
    Dropped = 2,

    /// <summary>
    /// The die was produced by an exploding roll that met or exceeded the exploding threshold.
    /// </summary>
    Exploded = 4,

    /// <summary>
    /// The die was rerolled because it met a reroll condition.
    /// </summary>
    Rerolled = 8,
}
