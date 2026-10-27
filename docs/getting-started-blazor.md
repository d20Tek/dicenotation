# Getting Started: Visual Dice Roller (Blazor)

This guide walks through installing the `d20Tek.DiceNotation.Blazor` package, registering its
services, and animating a first roll. For a conceptual overview see the
[introduction](introduction-blazor.md), and for the complete type listing see the
[API reference](api-reference-blazor.md).

## Prerequisites

- A Blazor WebAssembly or Blazor Server application targeting .NET 9 or .NET 10.

## 1. Install the package

```shell
dotnet add package d20tek-dicenotation-blazor
```

The core `d20Tek.DiceNotation` library comes along as a dependency, so there is no need to install
it separately.

## 2. Register the services

Add the following to `Program.cs`:

```csharp
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddDiceNotationBlazor();
```

This single call registers both the dice notation services and the component services. An
application that already calls `AddDiceNotation` does not need to change it, but the combined call
is enough on its own.

## 3. Import the namespaces

Add these entries to `_Imports.razor`:

```razor
@using d20Tek.DiceNotation
@using d20Tek.DiceNotation.Results
@using d20Tek.DiceNotation.Blazor.Components
@using d20Tek.DiceNotation.Blazor.Models
```

## 4. Roll and animate

The application rolls the dice and the component animates the finished result:

```razor
@page "/roll"
@inject IDice Dice

<button @onclick="Roll" disabled="@_isRolling">Roll</button>

<VisualDiceRoller Result="_result" OnRollCompleted="OnCompleted" OnRollFailed="OnFailed" />

@if (_summary is not null)
{
	<p><strong>@_summary.Expression</strong> = @_summary.Total</p>
}

@code {
	private DiceResult? _result;
	private DiceRollSummary? _summary;
	private bool _isRolling;

	private void Roll()
	{
		var result = Dice.Roll("4d6k3+2");
		if (result.HasError)
		{
			return;
		}

		_isRolling = true;
		_result = result;
	}

	private void OnCompleted(DiceRollSummary summary)
	{
		_summary = summary;
		_isRolling = false;
	}

	private void OnFailed(Exception exception) => _isRolling = false;
}
```

Assigning a new `DiceResult` to `Result` starts the animation. When the dice settle,
`OnRollCompleted` provides the expression, the authoritative total, and every individual die value.

The component renders only the canvas, so the result text above is the application's own markup.
This keeps styling under the application's control.

## 5. Handle invalid notation

`Dice.Roll` returns a result rather than throwing when the notation cannot be parsed. Check
`HasError` before assigning the result, and show `result.Error` to the user:

```csharp
var result = Dice.Roll(notation);
if (result.HasError)
{
	_error = result.Error;
	return;
}

_result = result;
```

## 6. Configure the tray and theme

Pass a `DiceRollerOptions` instance to change the tray size, theme, or motion behavior:

```razor
<VisualDiceRoller Result="_result"
				  Options="@(DiceRollerOptions.Default with
							 {
								 Theme = DiceTheme.DarkFantasy,
								 ReducedMotion = _reducedMotion,
							 })" />
```

Three themes ship with the package: `DiceTheme.Default`, `DiceTheme.DarkFantasy`, and
`DiceTheme.Parchment`. A custom theme is just a `DiceTheme` record with the colors an application
prefers.

Options can also be configured once at startup, which becomes the default for every component that
does not supply its own:

```csharp
builder.Services.AddDiceNotationBlazor(options =>
{
	options.Theme = DiceTheme.Parchment;
});
```

## 7. Replay a roll

Because the animation restarts only when `Result` is set to a different instance, replaying the
exact result already on screen requires an explicit call:

```razor
<VisualDiceRoller @ref="_roller" Result="_result" />

<button @onclick="() => _roller!.ReplayAsync()">Replay</button>

@code {
	private VisualDiceRoller? _roller;
}
```

## 8. Support reduced motion

Set `ReducedMotion` on the options to disable face churn, implied height, and motion smear. This is
an explicit option rather than an automatic media-query check, because an application usually
already knows its accessibility preferences and can apply them consistently across its UI.

The canvas is given a `role` of `img` and an accessible label generated from the settled result, so
the outcome is available to assistive technology without reading the canvas.

## Next steps

- Browse the [API reference](api-reference-blazor.md) for every parameter, option, and theme
  property.
- Read the [reproducible rolling guide](guide-reproducible-rolling.md) to combine seeded rolls with
  the visual roller.
