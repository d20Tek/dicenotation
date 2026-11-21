# d20Tek.DiceNotation

d20Tek.DiceNotation is a .NET library for parsing, evaluating, and rolling dice notation. It targets
.NET 9 and .NET 10, and it is trimming- and Native AOT-compatible, so you can use it in web, desktop,
console, and game clients alike.

Dice notation (also known as dice algebra, common dice notation, or RPG dice notation) is a simple,
algebra-like way to describe combinations of dice in role-playing games, such as `2d6+12` or
`4d6k3`. The library supports standard and percentile dice, Fudge/FATE dice, keep highest/lowest,
drop lowest, exploding and penetrating dice, grouping, and math operators. See the
[dice notation specification](docs/DiceNotationSpecCurrent.md) and the
[dice notation examples](docs/DiceNotationExamples.md) for the full syntax.

## Installation

The library is published on NuGet as `d20tek-dicenotation`.

.NET CLI:

```
dotnet add package d20tek-dicenotation
```

Package Manager Console:

```
PM > Install-Package d20tek-dicenotation
```

### Visual Dice Roller for Blazor

If you are building a Blazor application and want to animate your rolls, the companion package
renders physics-driven dice on an HTML canvas:

```
dotnet add package d20tek-dicenotation-blazor
```

The dice library still decides every outcome and the component only presents it, so an animation
never changes a result. See the [Blazor introduction](docs/introduction-blazor.md),
[Blazor getting started](docs/getting-started-blazor.md), and
[Blazor API reference](docs/api-reference-blazor.md) for details.

## Usage

### Dice notation strings

Parse and roll a notation string in one call:

```csharp
IDice dice = new Dice();
DiceResult result = dice.Roll("4d6k3+2");

Console.WriteLine($"{result.DiceExpression} = {result.Value} [{result.RollsDisplayText}]");
```

Invalid notation does not throw. Check `result.HasError` and `result.Error` instead.

### Building expressions in code

You can also build an expression with a fluent API:

```csharp
IDice dice = new Dice();

// equivalent of the dice notation: 4d6k3 + d8 + 5
var expression = DiceExpression.Create()
    .AddDice(6, 4, choose: 3)
    .AddDice(8)
    .AddConstant(5);

DiceResult result = dice.Roll(expression);
```

### Batch rolls

Roll the same notation or expression many times, for simulations and statistics:

```csharp
IReadOnlyList<DiceResult> results = dice.RollMany("1d20+5", 1000);
```

### Roll breakdowns

Each die records whether it was kept, dropped, exploded, or rerolled. Use `KeptResults`,
`DroppedResults`, `ExplodedResults`, and `RerolledResults` on `DiceResult`, or `VerboseDisplayText`
for an annotated description of the roll.

### Dependency injection

Register `IDice`, its configuration, the die roller, and an optional roll tracker with
`AddDiceNotation()`:

```csharp
services.AddDiceNotation(options => options
    .UseCryptoDieRoller()
    .UseRollTracker());
```

See the [dependency injection and factory reference](docs/api-reference-di-factory.md) for all options.

### Die rollers

Rolls use `RandomDieRoller` by default. You can pass a different roller to any `Roll` call, or
configure one through dependency injection:

- `RandomDieRoller` uses `System.Random`. It accepts a seed for reproducible rolls.
- `CryptoDieRoller` uses the cryptographic random number generator.
- `MathNetDieRoller` uses MathNet.Numerics random sources. It also accepts a seed.
- `ConstantDieRoller` always returns the same value, which is useful in unit tests.

Seeded rollers implement `IReplayableDieRoller` and expose a `ReplayToken` that
`DieRollerFactory.CreateFromReplayToken` uses to replay the same sequence of rolls. See the
[reproducible rolling guide](docs/guide-reproducible-rolling.md). You can also implement `IDieRoller`
to plug in your own random number generator.

### Roll tracking

`DieRollTracker` and `AggregateRollTracker` record the frequency of each die value, and can save and
load that data as JSON.

## Documentation

- [Introduction](docs/introduction.md)
- [Getting started](docs/getting-started.md)
- [API reference](docs/api-reference.md)
- [Release notes](ReleaseNotes.md)

## Samples

- [DiceCli](samples/DiceCli): a command-line dice roller.
- [BlazorDice](samples/BlazorDice): a Blazor WebAssembly app that shows the visual dice roller.

## Building and testing

You need the .NET 10 SDK. From the repository root:

```
dotnet build
dotnet test
```

The test projects use MSTest on Microsoft.Testing.Platform. To collect code coverage for a test
project, run:

```
dotnet test --project tests/d20Tek.DiceNotation.UnitTests -- --coverage --coverage-output-format cobertura
```

Performance benchmarks live in [benchmarks/d20Tek.DiceNotation.Benchmarks](benchmarks/d20Tek.DiceNotation.Benchmarks).

## Feedback

If you use this library and have feedback, bugs, or suggestions, please file them in the
[Issues](https://github.com/d20Tek/dicenotation/issues) section of this repository.
