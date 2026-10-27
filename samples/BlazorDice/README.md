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
