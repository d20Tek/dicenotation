namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// A die roller that uses the standard <see cref="System.Random"/> generator to produce values.
/// </summary>
/// <param name="random">The random generator used to produce values.</param>
/// <param name="tracker">An optional tracker used to record roll entries.</param>
public class RandomDieRoller(Random random, IAllowRollTrackerEntry? tracker) : RandomDieRollerBase(tracker)
{
    private static readonly Random DefaultRandomGenerator = new();
    private readonly Random _random = random;

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomDieRoller"/> class using a shared default generator.
    /// </summary>
    /// <param name="tracker">An optional tracker used to record roll entries.</param>
    public RandomDieRoller(IAllowRollTrackerEntry? tracker = null)
        : this(DefaultRandomGenerator, tracker) { }

    /// <inheritdoc />
    protected override int GetNextRandom(int sides)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(sides, 0);
        return _random.Next(sides) + 1;
    }
}
