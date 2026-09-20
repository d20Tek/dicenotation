namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Provides factory methods for creating replayable die rollers, including reconstructing a roller
/// from a previously captured <see cref="ReplayToken"/>.
/// </summary>
public static class DieRollerFactory
{
    /// <summary>
    /// Creates a seeded, replayable die roller that produces a reproducible sequence of rolls.
    /// </summary>
    /// <param name="seed">The seed used to initialize the generator.</param>
    /// <param name="algorithm">The generation algorithm to use. Defaults to <see cref="DieRollerAlgorithm.Random"/>.</param>
    /// <param name="tracker">An optional tracker used to record roll entries.</param>
    /// <returns>A replayable die roller initialized with the specified seed and algorithm.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="algorithm"/> is not a supported value.</exception>
    public static IReplayableDieRoller CreateSeeded(
        int seed,
        DieRollerAlgorithm algorithm = DieRollerAlgorithm.Random,
        IAllowRollTrackerEntry? tracker = null) =>
        algorithm switch
        {
            DieRollerAlgorithm.Random => new RandomDieRoller(seed, tracker),
            DieRollerAlgorithm.MathNetMersenneTwister => new MathNetDieRoller(seed, tracker),
            _ => throw new ArgumentOutOfRangeException(
                nameof(algorithm), algorithm, "Unsupported die roller algorithm for seeded rolling."),
        };

    /// <summary>
    /// Reconstructs a replayable die roller from a previously captured <see cref="ReplayToken"/>,
    /// producing the same sequence of rolls as the original seeded roller.
    /// </summary>
    /// <param name="token">The replay token describing the algorithm and seed to reproduce.</param>
    /// <param name="tracker">An optional tracker used to record roll entries.</param>
    /// <returns>A replayable die roller equivalent to the one that produced the token.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="token"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the token's algorithm is not supported for replay.</exception>
    public static IReplayableDieRoller CreateFromReplayToken(ReplayToken token, IAllowRollTrackerEntry? tracker = null)
    {
        ArgumentNullException.ThrowIfNull(token);
        return CreateSeeded(token.Seed, token.Algorithm, tracker);
    }
}
