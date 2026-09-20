# API Reference: Dependency Injection and Replayable Rolling

This reference documents the public types related to dependency injection registration and
seeded/replayable die rolling for the d20Tek.DiceNotation library. For the core library types, see
the main [API Reference](api-reference.md). The descriptions are derived from the library's XML
documentation comments.

For a task-focused walkthrough of seeded and replayable rolling, see the
[Reproducible Rolling guide](guide-reproducible-rolling.md).

## Table of Contents

- [Namespace: d20Tek.DiceNotation.DieRoller (Replayable Rolling)](#namespace-d20tekdicenotationdieroller-replayable-rolling)
  - [IReplayableDieRoller](#ireplayabledieroller)
  - [DieRollerAlgorithm](#dierolleralgorithm)
  - [ReplayToken](#replaytoken)
  - [DieRollerFactory](#dierollerfactory)
- [Namespace: d20Tek.DiceNotation.DependencyInjection](#namespace-d20tekdicenotationdependencyinjection)
  - [DiceServiceCollectionExtensions](#diceservicecollectionextensions)
  - [DiceNotationOptionsBuilder](#dicenotationoptionsbuilder)

---

## Namespace: d20Tek.DiceNotation.DieRoller (Replayable Rolling)

These types provide reproducible (seeded) rolling as a capability across the seedable die rollers.
The `RandomDieRoller` and `MathNetDieRoller` implementations (documented in the main
[API Reference](api-reference.md)) implement `IReplayableDieRoller` when constructed with a seed.

### IReplayableDieRoller

Defines a die roller that produces a reproducible sequence of rolls and can expose a `ReplayToken`
describing how that sequence can be recreated. Inherits `IDieRoller`.

**Properties**

| Member | Description |
| --- | --- |
| `ReplayToken? ReplayToken { get; }` | Gets the replay token capturing the algorithm and seed used to produce this roller's sequence, or `null` when the roller was created from an external generator whose seed is unknown. |

### DieRollerAlgorithm

Identifies the random number generation algorithm used by a replayable die roller. The algorithm is
captured as part of a `ReplayToken` so that a reproducible roll sequence can be reconstructed with the
same generator.

**Values**

| Member | Description |
| --- | --- |
| `Random` | The standard `System.Random` pseudo-random generator. |
| `MathNetMersenneTwister` | The Math.NET Numerics Mersenne Twister random source. |

### ReplayToken

Captures the information required to reproduce a seeded roll sequence: the generation algorithm and
the seed value. A replay token can be persisted and later supplied to
`DieRollerFactory.CreateFromReplayToken` to recreate an equivalent die roller. Reproducibility is only
guaranteed for the same algorithm on a compatible platform and library version. Cryptographic rollers
are not replayable, because their generators are not seedable.

**Properties**

| Member | Description |
| --- | --- |
| `DieRollerAlgorithm Algorithm { get; init; }` | Gets the random generation algorithm used to produce the sequence. |
| `int Seed { get; init; }` | Gets the seed value that initializes the generator. |

### DieRollerFactory

Provides factory methods for creating replayable die rollers, including reconstructing a roller from a
previously captured `ReplayToken`.

**Static Methods**

`static IReplayableDieRoller CreateSeeded(int seed, DieRollerAlgorithm algorithm = DieRollerAlgorithm.Random, IAllowRollTrackerEntry? tracker = null)`

Creates a seeded, replayable die roller that produces a reproducible sequence of rolls.

- `seed` - The seed used to initialize the generator.
- `algorithm` - The generation algorithm to use. Defaults to `DieRollerAlgorithm.Random`.
- `tracker` - An optional tracker used to record roll entries.
- Returns a replayable die roller initialized with the specified seed and algorithm.
- Throws `ArgumentOutOfRangeException` when `algorithm` is not a supported value.

`static IReplayableDieRoller CreateFromReplayToken(ReplayToken token, IAllowRollTrackerEntry? tracker = null)`

Reconstructs a replayable die roller from a previously captured `ReplayToken`, producing the same
sequence of rolls as the original seeded roller.

- `token` - The replay token describing the algorithm and seed to reproduce.
- `tracker` - An optional tracker used to record roll entries.
- Returns a replayable die roller equivalent to the one that produced the token.
- Throws `ArgumentNullException` when `token` is `null`.
- Throws `ArgumentOutOfRangeException` when the token's algorithm is not supported for replay.

---

## Namespace: d20Tek.DiceNotation.DependencyInjection

This namespace provides integration with `Microsoft.Extensions.DependencyInjection`, allowing the
dice notation services to be registered cleanly in an application's service container.

### DiceServiceCollectionExtensions

Provides extension methods for registering d20Tek.DiceNotation services with an `IServiceCollection`.

**Methods**

`static IServiceCollection AddDiceNotation(this IServiceCollection services, Action<DiceNotationOptionsBuilder> configure = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)`

Registers the dice notation services, including `IDiceConfiguration`, `IDieRoller`, and `IDice`, with
the specified service collection.

- `services` - The service collection to add the services to.
- `configure` - An optional delegate used to configure dice behavior, the die roller, and roll tracking.
- `lifetime` - The service lifetime used for the registered services. Defaults to `ServiceLifetime.Scoped`.
- Returns the same service collection so that additional calls can be chained.
- Throws `ArgumentNullException` when `services` is `null`.

### DiceNotationOptionsBuilder

Provides a fluent builder for configuring the services registered by `AddDiceNotation`.

**Methods**

`DiceNotationOptionsBuilder WithDefaultDieSides(int dieSides)`

Sets the default number of die sides used when a notation omits an explicit side count.

`DiceNotationOptionsBuilder WithBoundedResult(bool hasBoundedResult, int boundedMinimum = 1)`

Configures whether roll results are bounded by a minimum value and sets that minimum.

`DiceNotationOptionsBuilder UseRandomDieRoller()`

Configures the default die roller to use the standard `RandomDieRoller`.

`DiceNotationOptionsBuilder UseCryptoDieRoller()`

Configures the default die roller to use the cryptographically secure `CryptoDieRoller`.

`DiceNotationOptionsBuilder UseMathNetDieRoller()`

Configures the default die roller to use the Math.NET based `MathNetDieRoller`.

`DiceNotationOptionsBuilder UseSeededDieRoller(int seed)`

Configures the default die roller to use a seeded `RandomDieRoller` that produces a reproducible,
replayable sequence of rolls.

`DiceNotationOptionsBuilder UseSeededMathNetDieRoller(int seed)`

Configures the default die roller to use a seeded `MathNetDieRoller` that produces a reproducible,
replayable sequence of rolls.

`DiceNotationOptionsBuilder UseReplayDieRoller(ReplayToken token)`

Configures the default die roller to reproduce a previously captured roll sequence from a
`ReplayToken`. Throws `ArgumentNullException` when `token` is `null`.

`DiceNotationOptionsBuilder UseConstantDieRoller(int rollValue = 1)`

Configures the default die roller to use a `ConstantDieRoller` that always returns the specified value.

`DiceNotationOptionsBuilder UseDieRoller(Func<IServiceProvider, IDieRoller> dieRollerFactory)`

Configures the default die roller using a custom factory. Throws `ArgumentNullException` when
`dieRollerFactory` is `null`.

`DiceNotationOptionsBuilder UseRollTracker()`

Registers a `DieRollTracker` as the roll tracker, exposed through `IDieRollTracker` and
`IAllowRollTrackerEntry`.

`DiceNotationOptionsBuilder UseAggregateRollTracker()`

Registers an `AggregateRollTracker` as the roll tracker, exposed through `IAggregateRollTracker` and
`IAllowRollTrackerEntry`.

`DiceNotationOptionsBuilder UseRollTracker<TTracker, TService>()`

Registers a custom roll tracker implementation, exposed through the specified tracker service type as
well as `IAllowRollTrackerEntry`.
