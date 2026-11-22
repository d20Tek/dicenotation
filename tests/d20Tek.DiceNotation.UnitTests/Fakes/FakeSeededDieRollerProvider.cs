using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.Fakes;

/// <summary>
/// Creates seeded <see cref="RandomDieRoller"/> instances for an algorithm that is not built into the
/// library, and records each call so tests can see which provider was used.
/// </summary>
internal sealed class FakeSeededDieRollerProvider(DieRollerAlgorithm algorithm = FakeSeededDieRollerProvider.ExternalAlgorithm)
    : ISeededDieRollerProvider
{
    public const DieRollerAlgorithm ExternalAlgorithm = (DieRollerAlgorithm)99;

    public DieRollerAlgorithm Algorithm { get; } = algorithm;

    public int CreateCount { get; private set; }

    public IAllowRollTrackerEntry? LastTracker { get; private set; }

    public IReplayableDieRoller Create(int seed, IAllowRollTrackerEntry? tracker = null)
    {
        CreateCount++;
        LastTracker = tracker;
        return new RandomDieRoller(seed, tracker);
    }
}
