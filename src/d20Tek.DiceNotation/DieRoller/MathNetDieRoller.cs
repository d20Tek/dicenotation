using MathNet.Numerics.Random;

namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// A die roller that uses a Math.NET Numerics random source to generate values.
/// </summary>
/// <param name="source">The Math.NET random source used to generate values.</param>
/// <param name="tracker">An optional tracker used to record roll entries.</param>
public class MathNetDieRoller(RandomSource source, IAllowRollTrackerEntry? tracker = null) 
    : RandomDieRollerBase(tracker), IReplayableDieRoller
{
    private readonly RandomSource _randomSource = source;

    /// <inheritdoc />
    public ReplayToken? ReplayToken { get; private init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MathNetDieRoller"/> class using a Mersenne Twister source.
    /// </summary>
    /// <param name="tracker">An optional tracker used to record roll entries.</param>
    public MathNetDieRoller(IAllowRollTrackerEntry? tracker = null)
        : this(new MersenneTwister(), tracker) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MathNetDieRoller"/> class using a seeded Mersenne
    /// Twister source, producing a reproducible sequence of rolls that can be replayed using the
    /// <see cref="ReplayToken"/>.
    /// </summary>
    /// <param name="seed">The seed used to initialize the Mersenne Twister source.</param>
    /// <param name="tracker">An optional tracker used to record roll entries.</param>
    public MathNetDieRoller(int seed, IAllowRollTrackerEntry? tracker = null)
        : this(new MersenneTwister(seed), tracker) =>
        ReplayToken = new ReplayToken(DieRollerAlgorithm.MathNetMersenneTwister, seed);

    /// <inheritdoc />
    protected override int GetNextRandom(int sides)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(sides, 2);
        return _randomSource.Next(0, sides) + 1;
    }
}
