namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Creates seeded <see cref="MathNetDieRoller"/> instances for
/// <see cref="DieRollerAlgorithm.MathNetMersenneTwister"/>, so that <see cref="DieRollerFactory"/> and
/// replay through dependency injection can reproduce MathNet roll sequences.
/// </summary>
public sealed class MathNetSeededDieRollerProvider : ISeededDieRollerProvider
{
    /// <inheritdoc />
    public DieRollerAlgorithm Algorithm => DieRollerAlgorithm.MathNetMersenneTwister;

    /// <inheritdoc />
    public IReplayableDieRoller Create(int seed, IAllowRollTrackerEntry? tracker = null) =>
        new MathNetDieRoller(seed, tracker);
}
