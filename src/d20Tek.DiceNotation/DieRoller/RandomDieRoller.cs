//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation.DieRoller;

public class RandomDieRoller : RandomDieRollerBase
{
    private static readonly Random DefaultRandomGenerator = new Random();
    private readonly Random random;

    public RandomDieRoller(IAllowRollTrackerEntry? tracker = null)
        : this(DefaultRandomGenerator, tracker)
    {
    }

    public RandomDieRoller(Random random, IAllowRollTrackerEntry? tracker)
        : base(tracker)
    {
        this.random = random;
    }

    /// <inheritdoc/>
    protected override int GetNextRandom(int sides)
    {
        return random.Next(sides) + 1;
    }
}
