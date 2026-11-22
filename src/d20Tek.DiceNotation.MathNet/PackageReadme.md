# d20Tek DiceNotation Math.NET Die Roller

Adds a die roller for the [d20Tek.DiceNotation](https://github.com/d20Tek/dicenotation) library that
uses [Math.NET Numerics](https://numerics.mathdotnet.com/) random sources. It lives in its own package
so that applications that don't need it don't take on the Math.NET Numerics dependency.

## Install

```shell
dotnet add package d20tek-dicenotation-mathnet
```

## Roll with a Math.NET random source

```csharp
IDice dice = new Dice();

// Mersenne Twister by default; pass any Math.NET RandomSource to the constructor to use another one.
DiceResult result = dice.Roll("4d6k3", new MathNetDieRoller());
```

A seeded roller produces a reproducible sequence and exposes a `ReplayToken`:

```csharp
var roller = new MathNetDieRoller(seed: 12345);
ReplayToken token = roller.ReplayToken!;
```

## Dependency injection

```csharp
services.AddDiceNotation(options => options.UseMathNetDieRoller());

// Or a seeded, replayable roller.
services.AddDiceNotation(options => options.UseSeededMathNetDieRoller(12345));
```

## Replaying Math.NET tokens

Replaying a `MathNetMersenneTwister` token needs this package's provider:

```csharp
// With dependency injection.
services.AddDiceNotation(options => options
    .AddMathNetSeededDieRollerProvider()
    .UseReplayDieRoller(token));

// Without dependency injection.
var replayed = DieRollerFactory.CreateFromReplayToken(token, null, [new MathNetSeededDieRollerProvider()]);
```
