//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation.DieRoller;

public abstract class RandomDieRollerBase : IDieRoller
{
    private readonly IAllowRollTrackerEntry? tracker;

    public RandomDieRollerBase(IAllowRollTrackerEntry? tracker = null) => this.tracker = tracker;

    public int Roll(int sides, int? factor = null)
    {
        // roll the actual random value
        int result = GetNextRandom(sides);
        result += (factor is not null) ? factor.Value : 0;

        // if the user provided a roll tracker, then use it
        tracker?.AddDieRoll(sides, result, GetType());

        return result;
    }

    protected abstract int GetNextRandom(int sides);
}
