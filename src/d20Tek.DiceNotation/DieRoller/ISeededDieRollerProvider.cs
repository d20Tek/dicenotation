namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Creates seeded, replayable die rollers for one <see cref="DieRollerAlgorithm"/>, so that die rollers
/// defined outside this library can take part in seeded rolling and replay through
/// <see cref="DieRollerFactory"/>.
/// </summary>
public interface ISeededDieRollerProvider
{
    /// <summary>
    /// Gets the algorithm whose die rollers this provider creates.
    /// </summary>
    DieRollerAlgorithm Algorithm { get; }

    /// <summary>
    /// Creates a die roller for <see cref="Algorithm"/> that produces a reproducible sequence of rolls.
    /// </summary>
    /// <param name="seed">The seed used to initialize the generator.</param>
    /// <param name="tracker">An optional tracker used to record roll entries.</param>
    /// <returns>A replayable die roller initialized with the specified seed.</returns>
    IReplayableDieRoller Create(int seed, IAllowRollTrackerEntry? tracker = null);
}
