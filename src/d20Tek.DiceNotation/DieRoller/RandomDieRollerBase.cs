namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Provides a base implementation for random die rollers, handling roll factoring and tracking.
/// </summary>
/// <param name="tracker">An optional tracker used to record roll entries.</param>
public abstract class RandomDieRollerBase(IAllowRollTrackerEntry? tracker = null) : IDieRoller
{
    private readonly IAllowRollTrackerEntry? _tracker = tracker;

    /// <inheritdoc />
    public int Roll(int sides, int? factor = null)
    {
        // roll the actual random value
        int result = GetNextRandom(sides);
        result += (factor is not null) ? factor.Value : 0;

        // if the user provided a roll tracker, then use it
        _tracker?.AddDieRoll(sides, result, GetType());

        return result;
    }

    /// <summary>
    /// Generates the next random value for a die with the specified number of sides.
    /// </summary>
    /// <param name="sides">The number of sides on the die.</param>
    /// <returns>A random value in the valid range for the die.</returns>
    protected abstract int GetNextRandom(int sides);
}
