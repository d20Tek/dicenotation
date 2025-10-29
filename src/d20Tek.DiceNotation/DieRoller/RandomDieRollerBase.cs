namespace d20Tek.DiceNotation.DieRoller;

public abstract class RandomDieRollerBase(IAllowRollTrackerEntry? tracker = null) : IDieRoller
{
    private readonly IAllowRollTrackerEntry? _tracker = tracker;

    public int Roll(int sides, int? factor = null)
    {
        // roll the actual random value
        int result = GetNextRandom(sides);
        result += (factor is not null) ? factor.Value : 0;

        // if the user provided a roll tracker, then use it
        _tracker?.AddDieRoll(sides, result, GetType());

        return result;
    }

    protected abstract int GetNextRandom(int sides);
}
