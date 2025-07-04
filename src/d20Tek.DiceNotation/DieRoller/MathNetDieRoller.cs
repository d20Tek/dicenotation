//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using MathNet.Numerics.Random;

namespace d20Tek.DiceNotation.DieRoller;

public class MathNetDieRoller : RandomDieRollerBase
{
    private static RandomSource randomSource = new MersenneTwister();

    public MathNetDieRoller(IAllowRollTrackerEntry? tracker = null)
        : this(new MersenneTwister(), tracker) { }

    public MathNetDieRoller(RandomSource source, IAllowRollTrackerEntry? tracker = null)
        : base(tracker) => randomSource = source;

    protected override int GetNextRandom(int sides)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(sides, 2);
        return randomSource.Next(0, sides) + 1;
    }
}
