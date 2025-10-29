namespace d20Tek.DiceNotation.DieRoller;

public class RandomDieRoller(Random random, IAllowRollTrackerEntry? tracker) : RandomDieRollerBase(tracker)
{
    private static readonly Random DefaultRandomGenerator = new();
    private readonly Random _random = random;

    public RandomDieRoller(IAllowRollTrackerEntry? tracker = null)
        : this(DefaultRandomGenerator, tracker) { }

    protected override int GetNextRandom(int sides)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(sides, 0);
        return _random.Next(sides) + 1;
    }
}
