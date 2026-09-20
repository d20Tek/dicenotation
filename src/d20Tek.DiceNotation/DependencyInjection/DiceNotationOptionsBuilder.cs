using d20Tek.DiceNotation.DieRoller;
using Microsoft.Extensions.DependencyInjection;

namespace d20Tek.DiceNotation.DependencyInjection;

/// <summary>
/// Provides a fluent builder for configuring the services registered by
/// <see cref="DiceServiceCollectionExtensions.AddDiceNotation(IServiceCollection, Action{DiceNotationOptionsBuilder}?, ServiceLifetime)"/>.
/// </summary>
public sealed class DiceNotationOptionsBuilder
{
    private int _defaultDieSides = Constants.Config.DefaultDieSides;
    private int _boundedMinimum = Constants.Config.DefaultBoundedMin;
    private bool _hasBoundedResult = true;
    private Func<IServiceProvider, IDieRoller> _dieRollerFactory =
        sp => new RandomDieRoller(sp.GetService<IAllowRollTrackerEntry>());
    private Action<IServiceCollection, ServiceLifetime>? _trackerRegistrar;

    /// <summary>
    /// Sets the default number of die sides used when a notation omits an explicit side count.
    /// </summary>
    /// <param name="dieSides">The default number of die sides.</param>
    /// <returns>The current <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    public DiceNotationOptionsBuilder WithDefaultDieSides(int dieSides)
    {
        _defaultDieSides = dieSides;
        return this;
    }

    /// <summary>
    /// Configures whether roll results are bounded by a minimum value and sets that minimum.
    /// </summary>
    /// <param name="hasBoundedResult">
    /// <see langword="true"/> to bound results by a minimum value; otherwise, <see langword="false"/>.
    /// </param>
    /// <param name="boundedMinimum">The minimum value that a bounded result can produce.</param>
    /// <returns>The current <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    public DiceNotationOptionsBuilder WithBoundedResult(bool hasBoundedResult, int boundedMinimum = Constants.Config.DefaultBoundedMin)
    {
        _hasBoundedResult = hasBoundedResult;
        _boundedMinimum = boundedMinimum;
        return this;
    }

    /// <summary>
    /// Configures the default die roller to use the standard <see cref="RandomDieRoller"/>.
    /// </summary>
    /// <returns>The current <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    public DiceNotationOptionsBuilder UseRandomDieRoller() =>
        UseDieRoller(sp => new RandomDieRoller(sp.GetService<IAllowRollTrackerEntry>()));

    /// <summary>
    /// Configures the default die roller to use the cryptographically secure <see cref="CryptoDieRoller"/>.
    /// </summary>
    /// <returns>The current <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    public DiceNotationOptionsBuilder UseCryptoDieRoller() =>
        UseDieRoller(sp => new CryptoDieRoller(sp.GetService<IAllowRollTrackerEntry>()));

    /// <summary>
    /// Configures the default die roller to use the Math.NET based <see cref="MathNetDieRoller"/>.
    /// </summary>
    /// <returns>The current <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    public DiceNotationOptionsBuilder UseMathNetDieRoller() =>
        UseDieRoller(sp => new MathNetDieRoller(sp.GetService<IAllowRollTrackerEntry>()));

    /// <summary>
    /// Configures the default die roller to use a seeded <see cref="RandomDieRoller"/> that produces a
    /// reproducible, replayable sequence of rolls.
    /// </summary>
    /// <param name="seed">The seed used to initialize the random generator.</param>
    /// <returns>The current <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    public DiceNotationOptionsBuilder UseSeededDieRoller(int seed) =>
        UseDieRoller(sp => new RandomDieRoller(seed, sp.GetService<IAllowRollTrackerEntry>()));

    /// <summary>
    /// Configures the default die roller to use a seeded <see cref="MathNetDieRoller"/> that produces a
    /// reproducible, replayable sequence of rolls.
    /// </summary>
    /// <param name="seed">The seed used to initialize the Math.NET generator.</param>
    /// <returns>The current <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    public DiceNotationOptionsBuilder UseSeededMathNetDieRoller(int seed) =>
        UseDieRoller(sp => new MathNetDieRoller(seed, sp.GetService<IAllowRollTrackerEntry>()));

    /// <summary>
    /// Configures the default die roller to reproduce a previously captured roll sequence from a
    /// <see cref="ReplayToken"/>.
    /// </summary>
    /// <param name="token">The replay token describing the algorithm and seed to reproduce.</param>
    /// <returns>The current <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="token"/> is <see langword="null"/>.</exception>
    public DiceNotationOptionsBuilder UseReplayDieRoller(ReplayToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        return UseDieRoller(sp => DieRollerFactory.CreateFromReplayToken(token, sp.GetService<IAllowRollTrackerEntry>()));
    }

    /// <summary>
    /// Configures the default die roller to use a <see cref="ConstantDieRoller"/> that always returns the specified value.
    /// </summary>
    /// <param name="rollValue">The constant value returned by every roll.</param>
    /// <returns>The current <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    public DiceNotationOptionsBuilder UseConstantDieRoller(int rollValue = 1) =>
        UseDieRoller(_ => new ConstantDieRoller(rollValue));

    /// <summary>
    /// Configures the default die roller using a custom factory.
    /// </summary>
    /// <param name="dieRollerFactory">A factory that creates the die roller from the service provider.</param>
    /// <returns>The current <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dieRollerFactory"/> is <see langword="null"/>.</exception>
    public DiceNotationOptionsBuilder UseDieRoller(Func<IServiceProvider, IDieRoller> dieRollerFactory)
    {
        ArgumentNullException.ThrowIfNull(dieRollerFactory);
        _dieRollerFactory = dieRollerFactory;
        return this;
    }

    /// <summary>
    /// Registers a <see cref="DieRollTracker"/> as the roll tracker, exposed through
    /// <see cref="IDieRollTracker"/> and <see cref="IAllowRollTrackerEntry"/>.
    /// </summary>
    /// <returns>The current <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    public DiceNotationOptionsBuilder UseRollTracker() => UseRollTracker<DieRollTracker, IDieRollTracker>();

    /// <summary>
    /// Registers an <see cref="AggregateRollTracker"/> as the roll tracker, exposed through
    /// <see cref="IAggregateRollTracker"/> and <see cref="IAllowRollTrackerEntry"/>.
    /// </summary>
    /// <returns>The current <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    public DiceNotationOptionsBuilder UseAggregateRollTracker() =>
        UseRollTracker<AggregateRollTracker, IAggregateRollTracker>();

    /// <summary>
    /// Registers a custom roll tracker implementation, exposed through the specified tracker service type
    /// as well as <see cref="IAllowRollTrackerEntry"/>.
    /// </summary>
    /// <typeparam name="TTracker">The concrete tracker implementation type.</typeparam>
    /// <typeparam name="TService">The tracker service interface to expose.</typeparam>
    /// <returns>The current <see cref="DiceNotationOptionsBuilder"/> instance for chaining.</returns>
    public DiceNotationOptionsBuilder UseRollTracker<TTracker, TService>()
        where TTracker : class, TService, IAllowRollTrackerEntry
        where TService : class
    {
        _trackerRegistrar = (services, lifetime) =>
        {
            services.Add(new(typeof(TTracker), typeof(TTracker), lifetime));
            services.Add(new(typeof(TService), sp => sp.GetRequiredService<TTracker>(), lifetime));
            services.Add(new(
                typeof(IAllowRollTrackerEntry), sp => sp.GetRequiredService<TTracker>(), lifetime));
        };
        return this;
    }

    internal IDiceConfiguration BuildConfiguration(IServiceProvider serviceProvider) =>
        new DiceConfiguration(_defaultDieSides, _boundedMinimum, _hasBoundedResult, _dieRollerFactory(serviceProvider));

    internal IDieRoller BuildDieRoller(IServiceProvider serviceProvider) => _dieRollerFactory(serviceProvider);

    internal void RegisterTracker(IServiceCollection services, ServiceLifetime lifetime) =>
        _trackerRegistrar?.Invoke(services, lifetime);
}
