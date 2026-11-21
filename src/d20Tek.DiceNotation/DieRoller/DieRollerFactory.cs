namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Provides factory methods for creating replayable die rollers, including reconstructing a roller
/// from a previously captured <see cref="ReplayToken"/>.
/// </summary>
public static class DieRollerFactory
{
    private const string UnsupportedAlgorithmMessage =
        "Unsupported die roller algorithm for seeded rolling. Register an ISeededDieRollerProvider for it.";

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
        CreateSeeded(seed, algorithm, tracker, []);

    /// <summary>
    /// Creates a seeded, replayable die roller, using the supplied providers for algorithms that are not
    /// built into this library.
    /// </summary>
    /// <param name="seed">The seed used to initialize the generator.</param>
    /// <param name="algorithm">The generation algorithm to use.</param>
    /// <param name="tracker">An optional tracker used to record roll entries.</param>
    /// <param name="providers">
    /// The providers to search when <paramref name="algorithm"/> is not built in. When several providers
    /// handle the same algorithm, the last one is used.
    /// </param>
    /// <returns>A replayable die roller initialized with the specified seed and algorithm.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="providers"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="algorithm"/> is neither built in nor handled by any of the providers.
    /// </exception>
    public static IReplayableDieRoller CreateSeeded(
        int seed,
        DieRollerAlgorithm algorithm,
        IAllowRollTrackerEntry? tracker,
        IEnumerable<ISeededDieRollerProvider> providers)
    {
        ArgumentNullException.ThrowIfNull(providers);

        return CreateBuiltIn(seed, algorithm, tracker)
            ?? FindProvider(algorithm, providers)?.Create(seed, tracker)
            ?? throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, UnsupportedAlgorithmMessage);
    }

    /// <summary>
    /// Reconstructs a replayable die roller from a previously captured <see cref="ReplayToken"/>,
    /// producing the same sequence of rolls as the original seeded roller.
    /// </summary>
    /// <param name="token">The replay token describing the algorithm and seed to reproduce.</param>
    /// <param name="tracker">An optional tracker used to record roll entries.</param>
    /// <returns>A replayable die roller equivalent to the one that produced the token.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="token"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the token's algorithm is not supported for replay.</exception>
    public static IReplayableDieRoller CreateFromReplayToken(ReplayToken token, IAllowRollTrackerEntry? tracker = null) =>
        CreateFromReplayToken(token, tracker, []);

    /// <summary>
    /// Reconstructs a replayable die roller from a previously captured <see cref="ReplayToken"/>, using the
    /// supplied providers for algorithms that are not built into this library.
    /// </summary>
    /// <param name="token">The replay token describing the algorithm and seed to reproduce.</param>
    /// <param name="tracker">An optional tracker used to record roll entries.</param>
    /// <param name="providers">
    /// The providers to search when the token's algorithm is not built in. When several providers handle
    /// the same algorithm, the last one is used.
    /// </param>
    /// <returns>A replayable die roller equivalent to the one that produced the token.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="token"/> or <paramref name="providers"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the token's algorithm is neither built in nor handled by any of the providers.
    /// </exception>
    public static IReplayableDieRoller CreateFromReplayToken(
        ReplayToken token,
        IAllowRollTrackerEntry? tracker,
        IEnumerable<ISeededDieRollerProvider> providers)
    {
        ArgumentNullException.ThrowIfNull(token);

        return CreateSeeded(token.Seed, token.Algorithm, tracker, providers);
    }

    /// <summary>Creates a roller for an algorithm built into this library, or returns null for any other.</summary>
    private static IReplayableDieRoller? CreateBuiltIn(
        int seed,
        DieRollerAlgorithm algorithm,
        IAllowRollTrackerEntry? tracker) =>
        algorithm switch
        {
            DieRollerAlgorithm.Random => new RandomDieRoller(seed, tracker),
            DieRollerAlgorithm.MathNetMersenneTwister => new MathNetDieRoller(seed, tracker),
            _ => null,
        };

    /// <summary>Finds the last provider for the algorithm, matching how dependency injection resolves overrides.</summary>
    private static ISeededDieRollerProvider? FindProvider(
        DieRollerAlgorithm algorithm,
        IEnumerable<ISeededDieRollerProvider> providers) =>
        providers.LastOrDefault(provider => provider.Algorithm == algorithm);
}
