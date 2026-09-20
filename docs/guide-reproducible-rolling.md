# Guide: Reproducible Rolling

This guide explains how to produce reproducible, replayable dice rolls with d20Tek.DiceNotation. It
covers why reproducibility matters, how the seeded and replay APIs work, and complete examples you
can adapt in your own application.

For the reference documentation of the types used here, see the
[Dependency Injection and Replayable Rolling reference](api-reference-di-factory.md).

## Why Reproducible Rolling Matters

By default, dice rolls are random and cannot be repeated. That is exactly what you want in most
gameplay, but there are several situations where you need to reproduce the same sequence of rolls on
demand:

- **Bug reproduction.** A player reports an unexpected result, such as a combat that resolved in a
  surprising way. If they can share the seed or a replay token, you can reproduce the exact same
  sequence of rolls locally and debug the behavior instead of guessing.
- **Deterministic tests.** Unit tests and integration tests that involve randomness are hard to
  assert against. Seeding the roller makes the outcomes stable, so a test can verify precise values
  without becoming flaky.
- **Repeatable simulations.** Balance passes, Monte Carlo analysis, and probability studies often
  need to be re-run with identical inputs. A fixed seed guarantees that every run starts from the
  same point, so you can compare results meaningfully.
- **Save and replay.** A game can persist a replay token alongside a save file so that a specific
  encounter, or an entire session, can be reconstructed and reviewed later.

The key idea is that a seed plus an algorithm fully determines the sequence of rolls. Capture those
two values and you can recreate the sequence anywhere the same algorithm is available.

## What Is Replayable

Reproducible rolling is a capability of the seedable die rollers:

| Die roller | Replayable | Notes |
| --- | --- | --- |
| `RandomDieRoller` | Yes | Uses `System.Random`. Replayable when constructed from a seed. |
| `MathNetDieRoller` | Yes | Uses the Math.NET Numerics Mersenne Twister source. Replayable when constructed from a seed. |
| `CryptoDieRoller` | No | Uses a cryptographically secure generator that is not seedable, so it cannot be reproduced. |
| `ConstantDieRoller` | Not applicable | Always returns a fixed value and is intended for testing. |

A replayable roller implements `IReplayableDieRoller`, which exposes a nullable `ReplayToken`. The
token is populated when the roller was created from a seed, and it is `null` when the roller was
created from an external generator whose seed is unknown.

> **Note:** Reproducibility is only guaranteed for the same algorithm on a compatible platform and
> library version. Do not rely on a token produced by one algorithm reproducing the same sequence
> under a different algorithm, runtime, or library version.

## Seeded Rolling

The simplest way to get reproducible results is to construct a roller from a seed. Two rollers that
share the same seed and algorithm produce identical sequences of rolls.

```csharp
using d20Tek.DiceNotation;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

IDice dice = new Dice();

// Two rollers created with the same seed produce the same sequence.
var first = new RandomDieRoller(seed: 12345);
var second = new RandomDieRoller(seed: 12345);

DiceResult firstResult = dice.Roll("4d6", first);
DiceResult secondResult = dice.Roll("4d6", second);

Console.WriteLine(firstResult.Value == secondResult.Value); // True
```

You can use the Math.NET roller in exactly the same way when you prefer its generator:

```csharp
var roller = new MathNetDieRoller(seed: 12345);
DiceResult result = dice.Roll("4d6", roller);
```

## Capturing and Replaying a Sequence

When you need to reproduce a sequence later, or on another machine, capture the roller's
`ReplayToken` and persist it. The token is a small, serializable record that holds only the
algorithm and the seed.

```csharp
using d20Tek.DiceNotation;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

IDice dice = new Dice();

// Roll with a seeded, replayable roller.
IReplayableDieRoller roller = DieRollerFactory.CreateSeeded(
	seed: 20240107,
	algorithm: DieRollerAlgorithm.Random);

DiceResult original = dice.Roll("2d20+5", roller);

// Capture the token so the sequence can be reproduced later.
ReplayToken? token = roller.ReplayToken;
```

The token exposes just two values, so it is easy to store in a database, a save file, or a log entry:

```csharp
if (token is not null)
{
	DieRollerAlgorithm algorithm = token.Algorithm; // for example, Random
	int seed = token.Seed;                          // for example, 20240107
}
```

To reproduce the sequence, rebuild a roller from the token with
`DieRollerFactory.CreateFromReplayToken`, then roll the same expression again:

```csharp
// Later, or on another machine, reconstruct the roller from the token.
IReplayableDieRoller replay = DieRollerFactory.CreateFromReplayToken(token!);

DiceResult reproduced = dice.Roll("2d20+5", replay);

Console.WriteLine(original.Value == reproduced.Value); // True
```

## Using Reproducible Rolling with Dependency Injection

If you register services with `AddDiceNotation`, you can configure the resolved `IDieRoller` to be
seeded or to replay a token. This is useful for deterministic tests or for wiring a known sequence
into a running application.

```csharp
using d20Tek.DiceNotation.DependencyInjection;
using d20Tek.DiceNotation.DieRoller;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Register a seeded RandomDieRoller.
services.AddDiceNotation(options => options.UseSeededDieRoller(12345));

// Or register a seeded MathNetDieRoller.
services.AddDiceNotation(options => options.UseSeededMathNetDieRoller(12345));

// Or replay a previously captured token.
var token = new ReplayToken(DieRollerAlgorithm.Random, 12345);
services.AddDiceNotation(options => options.UseReplayDieRoller(token));
```

Once registered, resolve `IDice` as usual and every roll draws from the seeded or replayed sequence.

## Choosing an Algorithm

Both replayable algorithms behave the same way from the caller's perspective; the difference is the
underlying generator.

- Use `DieRollerAlgorithm.Random` for the standard `System.Random` generator. This is the default
  and is a good fit for most gameplay and testing.
- Use `DieRollerAlgorithm.MathNetMersenneTwister` when you want the statistical properties of the
  Math.NET Mersenne Twister generator, for example in simulations or probability analysis.

Whichever algorithm you pick, record it in the token so that the sequence is reconstructed with the
same generator.

## Summary

- A seed plus an algorithm fully determines a sequence of rolls, which makes reproduction possible.
- Seeded `RandomDieRoller` and `MathNetDieRoller` instances are replayable; the cryptographic roller
  is not.
- Capture `IReplayableDieRoller.ReplayToken` and persist its `Algorithm` and `Seed` to reproduce a
  sequence later.
- Rebuild a roller with `DieRollerFactory.CreateFromReplayToken`, or configure one through
  `AddDiceNotation`, to replay the captured sequence.

## Next Steps

- Review the [Getting Started](getting-started.md) guide for the core rolling workflow.
- See the [Dependency Injection and Replayable Rolling reference](api-reference-di-factory.md) for
  the full API surface of the types used in this guide.
