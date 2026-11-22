using d20Tek.DiceNotation.DependencyInjection;
using d20Tek.DiceNotation.DieRoller;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Adds Math.NET based die roller options to <see cref="DiceNotationOptionsBuilder"/>. These live in the
/// same namespace as <c>AddDiceNotation</c>, so code that configures the builder needs no extra using.
/// </summary>
public static class MathNetOptionsBuilderExtensions
{
    /// <summary>
    /// Configures the default die roller to use the Math.NET based <see cref="MathNetDieRoller"/>.
    /// </summary>
    /// <param name="builder">The builder to configure.</param>
    /// <returns>The same <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    public static DiceNotationOptionsBuilder UseMathNetDieRoller(this DiceNotationOptionsBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.UseDieRoller(sp => new MathNetDieRoller(sp.GetService<IAllowRollTrackerEntry>()));
    }

    /// <summary>
    /// Configures the default die roller to use a seeded <see cref="MathNetDieRoller"/> that produces a
    /// reproducible, replayable sequence of rolls.
    /// </summary>
    /// <param name="builder">The builder to configure.</param>
    /// <param name="seed">The seed used to initialize the Math.NET generator.</param>
    /// <returns>The same <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    public static DiceNotationOptionsBuilder UseSeededMathNetDieRoller(this DiceNotationOptionsBuilder builder, int seed)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.UseDieRoller(sp => new MathNetDieRoller(seed, sp.GetService<IAllowRollTrackerEntry>()));
    }

    /// <summary>
    /// Registers a <see cref="MathNetSeededDieRollerProvider"/>, so that
    /// <see cref="DiceNotationOptionsBuilder.UseReplayDieRoller(ReplayToken)"/> can replay
    /// <see cref="DieRollerAlgorithm.MathNetMersenneTwister"/> tokens.
    /// </summary>
    /// <param name="builder">The builder to configure.</param>
    /// <returns>The same <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    public static DiceNotationOptionsBuilder AddMathNetSeededDieRollerProvider(this DiceNotationOptionsBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddSeededDieRollerProvider(new MathNetSeededDieRollerProvider());
    }
}
