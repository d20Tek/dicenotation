//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation.DieRoller;

public abstract class RandomDieRollerBase : IDieRoller
{
    private readonly IAllowRollTrackerEntry? tracker;

    public RandomDieRollerBase(IAllowRollTrackerEntry? tracker = null)
    {
        this.tracker = tracker;
    }

    public int Roll(int sides, int? factor = null)
    {
        // roll the actual random value
        int result = GetNextRandom(sides);
        if (factor != null)
        {
            result += factor.Value;
        }

        // if the user provided a roll tracker, then use it
        if (tracker != null)
        {
            tracker.AddDieRoll(sides, result, GetType());
        }

        return result;
    }

    protected abstract int GetNextRandom(int sides);
}
