# d20Tek DiceNotation Visual Dice Roller for Blazor

A Blazor component that animates dice rolls produced by the
[d20Tek.DiceNotation](https://github.com/d20Tek/dicenotation) library. The dice library decides the
outcome and the component presents it, so an animation never changes a result.

## Install

```shell
dotnet add package d20tek-dicenotation-blazor
```

## Register the services

```csharp
builder.Services.AddDiceNotationBlazor();
```

This registers the dice library and the component services together, so a single call is enough for
most applications.

## Use the component

```razor
@inject IDice Dice

<VisualDiceRoller Result="_result" OnRollCompleted="OnCompleted" />

<button @onclick="Roll">Roll</button>

@code {
	private DiceResult? _result;

	private void Roll() => _result = Dice.Roll("4d6k3+2");

	private void OnCompleted(DiceRollSummary summary) => Console.WriteLine(summary.Total);
}
```

Assign a finished `DiceResult` to `Result` and the component animates it. When the dice settle,
`OnRollCompleted` reports a `DiceRollSummary` containing the expression, the total, and the
individual die values.

## Features

- Physics-driven dice that tumble and settle in a tray, rendered on an HTML canvas.
- Shape-aware dice: d2 renders as a round coin, d4 as a triangle, d6 as a square, d8 as a diamond,
  d10 and d100 as kites, d12 as a pentagon, and d20 as a hexagon.
- Built-in `DiceTheme` presets (`Default`, `DarkFantasy`, `Parchment`) plus fully custom themes.
- `DiceRollerOptions` for tray size, theme, visual settings, simulation settings, and reduced motion.
- `ReplayAsync` to re-animate the current result without rolling again.
- An accessible label describing the settled result, and a reduced-motion mode.

## Supported targets

- .NET 9 and .NET 10.
- Blazor WebAssembly and Blazor Server.

## Documentation

- [Introduction](https://github.com/d20Tek/dicenotation/blob/main/docs/introduction-blazor.md)
- [Getting started](https://github.com/d20Tek/dicenotation/blob/main/docs/getting-started-blazor.md)
- [API reference](https://github.com/d20Tek/dicenotation/blob/main/docs/api-reference-blazor.md)
