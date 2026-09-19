using MathNet.Numerics.Random;

namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// A die roller that uses a Math.NET Numerics random source to generate values.
/// </summary>
/// <param name="source">The Math.NET random source used to generate values.</param>
/// <param name="tracker">An optional tracker used to record roll entries.</param>
public class MathNetDieRoller(RandomSource source, IAllowRollTrackerEntry? tracker = null) :
    RandomDieRollerBase(tracker)
{
    private readonly RandomSource _randomSource = source;

    /// <summary>
    /// Initializes a new instance of the <see cref="MathNetDieRoller"/> class using a Mersenne Twister source.
    /// </summary>
    /// <param name="tracker">An optional tracker used to record roll entries.</param>
    public MathNetDieRoller(IAllowRollTrackerEntry? tracker = null)
        : this(new MersenneTwister(), tracker) { }

    /// <inheritdoc />
    protected override int GetNextRandom(int sides)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(sides, 2);
        return _randomSource.Next(0, sides) + 1;
    }
}
