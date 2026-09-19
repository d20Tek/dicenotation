# Getting Started

This guide walks you through installing d20Tek.DiceNotation and using its core features to roll
dice in your .NET application.

## Prerequisites

- A .NET project targeting .NET 9 or later.
- A package source configured for NuGet (the default nuget.org source is sufficient).

## Installation

Install the NuGet package using whichever workflow you prefer.

Package Manager Console:

```
PM > Install-Package d20tek-dicenotation
```

.NET CLI:

```
dotnet add package d20tek-dicenotation
```

You can also install it through the Visual Studio NuGet Package Manager UI by searching for
"d20tek-dicenotation".

## Your First Roll

The primary entry point is the `IDice` interface and its default implementation, `Dice`. Create a
`Dice` instance and call `Roll` with a dice notation string.

```csharp
using d20Tek.DiceNotation;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

IDice dice = new Dice();
DiceResult result = dice.Roll("d20+4", new RandomDieRoller());

Console.WriteLine($"Roll result = {result.Value}");
```

If you omit the die roller argument, the roll uses the default die roller configured on the
instance:

```csharp
IDice dice = new Dice();
DiceResult result = dice.Roll("d20+4");
```

## Two Ways to Build Expressions

The library supports two equivalent approaches for describing what to roll. Both produce the same
kind of `DiceResult`.

### 1. Dice Notation Strings

Parse a string that follows the dice notation language. This is the most compact approach and is
ideal when expressions come from configuration, user input, or game data.

```csharp
IDice dice = new Dice();
DiceResult result = dice.Roll("4d6k3 + d8 + 5", new RandomDieRoller());
Console.WriteLine($"Roll result = {result.Value}");
```

### 2. The Fluent Programmatic API

Build the expression in code by chaining operations with `DiceExpression`. This approach is
strongly typed and convenient when you assemble expressions dynamically.

```csharp
IDice dice = new Dice();

// Equivalent of dice expression: 4d6k3 + d8 + 5
DiceExpression expression = DiceExpression.Create()
										  .AddDice(6, 4, choose: 3)
										  .AddDice(8)
										  .AddConstant(5);

DiceResult result = dice.Roll(expression, new RandomDieRoller());
Console.WriteLine($"Roll result = {result.Value}");
```

## Working With the Result

Every roll returns a `DiceResult` that contains more than just the total. The most commonly used
members are shown below.

```csharp
DiceResult result = dice.Roll("3d6+2", new RandomDieRoller());

Console.WriteLine(result.Value);            // The final calculated total.
Console.WriteLine(result.DiceExpression);   // The normalized expression that was rolled.
Console.WriteLine(result.DieRollerUsed);    // The die roller type that produced the roll.
Console.WriteLine(result.RollsDisplayText); // A friendly string of the individual dice rolls.

foreach (TermResult term in result.Results)
{
	Console.WriteLine($"{term.Type}: {term.Value} (scalar {term.Scalar})");
}
```

### Handling Errors

When an expression cannot be parsed or evaluated, the result reports the failure rather than
throwing during normal use. Check `HasError` before relying on the value.

```csharp
DiceResult result = dice.Roll("not-a-valid-expression");

if (result.HasError)
{
	Console.WriteLine($"Could not roll: {result.Error}");
}
else
{
	Console.WriteLine($"Roll result = {result.Value}");
}
```

## Choosing a Die Roller

All roll operations use an `IDieRoller` to generate random values. The library provides several
implementations for different needs.

| Die roller | Use it when |
| --- | --- |
| `RandomDieRoller` | You want general-purpose randomness backed by `System.Random`. |
| `CryptoDieRoller` | You need cryptographically strong random numbers. |
| `MathNetDieRoller` | You want an advanced random source from Math.NET Numerics. |
| `ConstantDieRoller` | You need deterministic, repeatable values (ideal for unit tests). |

```csharp
// Deterministic rolls for a test: every die returns 4.
IDice dice = new Dice();
DiceResult result = dice.Roll("3d6", new ConstantDieRoller(4));
// result.Value == 12
```

If none of these suit your needs, implement the `IDieRoller` interface to provide your own random
number generation strategy.

## Configuring Default Behavior

You can customize default dice behavior by supplying an `IDiceConfiguration` when constructing
`Dice`. The configuration controls the default number of die sides, whether results are bounded by a
minimum value, and which die roller is used when no explicit roller is passed.

```csharp
var config = new DiceConfiguration();
config.SetDefaultDieSides(6);
config.SetHasBoundedResult(true);
config.SetBoundedMinimumResult(1);
config.SetDefaultDieRoller(new CryptoDieRoller());

IDice dice = new Dice(config);

// Uses the configured default die roller and default die sides.
DiceResult result = dice.Roll("3d");
```

## Tracking Roll Statistics

To collect statistics about the dice you roll, attach a tracker to a die roller. The tracker records
every roll and can produce frequency distributions or serialize the data to JSON.

```csharp
using d20Tek.DiceNotation.DieRoller;

IDieRollTracker tracker = new DieRollTracker();
IDieRoller roller = new RandomDieRoller(tracker);
IDice dice = new Dice();

for (int i = 0; i < 100; i++)
{
	dice.Roll("d20", roller);
}

// Aggregate frequency data across all recorded rolls.
IList<AggregateDieTrackingData> frequencies = await tracker.GetFrequencyDataViewAsync();
foreach (AggregateDieTrackingData entry in frequencies)
{
	Console.WriteLine($"d{entry.DieSides} => {entry.Result}: {entry.Count} times ({entry.Percentage}%)");
}

// Persist and reload the tracked data.
string json = await tracker.ToJsonAsync();
await tracker.LoadFromJsonAsync(json);
```

For a lightweight, synchronous alternative that only keeps aggregate counts, use
`AggregateRollTracker` with its `GetFrequencyDataView`, `ToJson`, and `LoadFromJson` methods.

## Next Steps

- Review the full [API Reference](api-reference.md) for every public type and member.
- Read the [Introduction](introduction.md) for an overview of why the library exists and what it
  offers.
