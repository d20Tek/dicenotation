namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Identifies the random number generation algorithm used by a replayable die roller. The algorithm
/// is captured as part of a <see cref="ReplayToken"/> so that a reproducible roll sequence can be
/// reconstructed with the same generator.
/// </summary>
public enum DieRollerAlgorithm
{
    /// <summary>
    /// The standard <see cref="System.Random"/> pseudo-random generator.
    /// </summary>
    Random = 0,

    /// <summary>
    /// The Math.NET Numerics Mersenne Twister random source.
    /// </summary>
    MathNetMersenneTwister = 1,
}

/// <summary>
/// Captures the information required to reproduce a seeded roll sequence: the generation algorithm and
/// the seed value. A replay token can be persisted and later supplied to
/// <see cref="DieRollerFactory.CreateFromReplayToken(ReplayToken, IAllowRollTrackerEntry?)"/> to
/// recreate an equivalent die roller.
/// </summary>
/// <param name="Algorithm">The random generation algorithm used to produce the sequence.</param>
/// <param name="Seed">The seed value that initializes the generator.</param>
/// <remarks>
/// Reproducibility is only guaranteed for the same algorithm on a compatible platform and library
/// version. Cryptographic rollers are not replayable, because their generators are not seedable.
/// </remarks>
public sealed record ReplayToken(DieRollerAlgorithm Algorithm, int Seed);
