using MathNet.Numerics.Random;

namespace d20Tek.DiceNotation.DieRoller;

public class MathNetDieRoller(RandomSource source, IAllowRollTrackerEntry? tracker = null) : RandomDieRollerBase(tracker)
{
    private readonly RandomSource _randomSource = source;

    public MathNetDieRoller(IAllowRollTrackerEntry? tracker = null)
        : this(new MersenneTwister(), tracker) { }

    protected override int GetNextRandom(int sides)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(sides, 2);
        return _randomSource.Next(0, sides) + 1;
    }
}
