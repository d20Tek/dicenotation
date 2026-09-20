namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Defines a die roller that produces a reproducible sequence of rolls and can expose a
/// <see cref="ReplayToken"/> describing how that sequence can be recreated.
/// </summary>
public interface IReplayableDieRoller : IDieRoller
{
    /// <summary>
    /// Gets the replay token that captures the algorithm and seed used to produce this roller's
    /// sequence, so that an equivalent roller can be reconstructed later. Returns
    /// <see langword="null"/> when the roller was created from an external generator whose seed is
    /// not known and therefore cannot be reproduced.
    /// </summary>
    ReplayToken? ReplayToken { get; }
}
