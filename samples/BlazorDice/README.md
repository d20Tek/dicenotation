# Sample: BlazorDice

A minimal Blazor WebAssembly application that demonstrates the `VisualDiceRoller` component from
the `d20Tek.DiceNotation.Blazor` package.

## Running the sample

```shell
dotnet run --project samples/BlazorDice/BlazorDice.csproj
```

Then open the URL printed in the console.

## What it shows

The home page is the dice roller. It demonstrates the pattern a consuming application follows: the
application parses the notation and rolls through `IDice`, then hands the finished `DiceResult` to
the component to animate. The component never decides an outcome.

The page exercises these scenarios:

- Rolling arbitrary notation entered in the text box.
- Example notations covering keep-highest ability scores, a single d20 with a modifier, mixed die
  types, a d2 coin flip, percentile dice, and an intentionally invalid notation.
- Switching between the built-in `Default`, `DarkFantasy`, and `Parchment` themes.
- Toggling reduced motion.
- Replaying the current result with `ReplayAsync`.
- Reporting both roll errors and animation failures.
- Switching between two output tabs: the animated dice, and a roll history list showing the most
  recent rolls with the newest at the top.

## Output tabs

The output area has a "Dice" tab holding the `VisualDiceRoller` and a "Roll history" tab listing
previous results. The tabs only change when the user clicks them, so rolling while the history is
open keeps the history in view and simply adds the new result to the top of the list.

The roller stays mounted while the history tab is active rather than being removed from the markup.
The canvas sizes itself from its bounding box, so unmounting it would discard the settled scene and
remounting it would measure a width of zero. Hiding it with a CSS class and calling `ResizeAsync`
when the tab becomes visible again restores the backing buffer correctly.

The history list is built directly from the `DiceResult` the library returns, not from the
animation, so entries appear as soon as the dice are rolled. While the dice tab is hidden, no
animation runs at all: the sample holds onto the most recent result instead. When the user switches
back to the dice tab, only that latest result is animated, and any earlier rolls made while the tab
was hidden are skipped.

## Key wiring

`Program.cs` registers everything with a single call:

```csharp
builder.Services.AddDiceNotationBlazor();
```

`wwwroot/index.html` references the component stylesheet:

```html
<link rel="stylesheet" href="_content/d20Tek.DiceNotation.Blazor/css/dice-roller.css" />
```

## Further reading

- [Blazor introduction](../../docs/introduction-blazor.md)
- [Blazor getting started](../../docs/getting-started-blazor.md)
- [Blazor API reference](../../docs/api-reference-blazor.md)
