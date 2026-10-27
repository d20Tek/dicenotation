# API Reference: Visual Dice Roller (Blazor)

This reference documents the public types in the `d20Tek.DiceNotation.Blazor` package, which
animates completed dice rolls in a two-dimensional tray drawn on an HTML canvas. For the core
library types, see the main [API Reference](api-reference.md), and for dependency injection in the
core library, see the [Dependency Injection reference](api-reference-di-factory.md). The
descriptions are derived from the package's XML documentation comments.

The package never rolls dice. The application rolls through `IDice`, then hands the finished
`DiceResult` to the component, so the faces that come to rest always match the result the
application already calculated.

## Table of Contents

- [Getting started](#getting-started)
- [Namespace: d20Tek.DiceNotation.Blazor.Components](#namespace-d20tekdicenotationblazorcomponents)
  - [VisualDiceRoller](#visualdiceroller)
- [Namespace: d20Tek.DiceNotation.Blazor.Models](#namespace-d20tekdicenotationblazormodels)
  - [DiceRollerOptions](#dicerolleroptions)
  - [DiceTraySize](#dicetraysize)
  - [DiceTheme](#dicetheme)
  - [DiceVisualSettings](#dicevisualsettings)
  - [DiceSimulationSettings](#dicesimulationsettings)
  - [DiceRollSummary](#dicerollsummary)
  - [VisualDie](#visualdie)
- [Namespace: Microsoft.Extensions.DependencyInjection](#namespace-microsoftextensionsdependencyinjection)
  - [DiceRollerServiceCollectionExtensions](#dicerollerservicecollectionextensions)
- [Namespace: d20Tek.DiceNotation.Blazor.DependencyInjection](#namespace-d20tekdicenotationblazordependencyinjection)
  - [DiceRollerOptionsBuilder](#dicerolleroptionsbuilder)
- [Die shapes](#die-shapes)
- [Accessibility and reduced motion](#accessibility-and-reduced-motion)

---

## Getting started

Register the services once during application startup. The call also registers the core dice
notation services, so no separate `AddDiceNotation` call is required.

```csharp
builder.Services.AddDiceNotationBlazor();
```

Add the component to a page and supply a result:

```razor
@inject IDice Dice

<VisualDiceRoller Result="_result" OnRollCompleted="OnCompleted" />

<button @onclick="Roll">Roll</button>

@code {
	private DiceResult? _result;
	private DiceRollSummary? _summary;

	private void Roll() => _result = Dice.Roll("4d6k3+2");

	private void OnCompleted(DiceRollSummary summary) => _summary = summary;
}
```

The component renders only the canvas. The application renders its own result text, which keeps the
markup under the application's control and avoids fighting the component's styling.

---

## Namespace: d20Tek.DiceNotation.Blazor.Components

### VisualDiceRoller

Animates a completed dice roll in a two-dimensional tray drawn on an HTML canvas. Each instance owns
its own simulation, renderer, and canvas, so several rollers can appear on one page. The component
draws through the browser's own frame scheduler and does not re-render itself per frame.

**Parameters**

| Member | Description |
| --- | --- |
| `DiceResult? Result { get; set; }` | The completed roll to animate. Assigning a different result starts a new animation, replacing any animation already in progress. A result representing a failed roll is reported through `OnRollFailed` and is not animated. |
| `DiceRollerOptions? Options { get; set; }` | The tray, theme, presentation, and physics configuration. When this is `null`, the options registered during startup are used, falling back to the library defaults. |
| `bool Disabled { get; set; }` | A value indicating whether new animations are suppressed. Setting this does not stop an animation that is already running. |
| `string? CssClass { get; set; }` | Additional CSS classes applied to the component's container element. |
| `string? AriaLabel { get; set; }` | The accessible label describing the canvas. When this is `null`, a label is generated from the current result. |
| `EventCallback<DiceRollSummary> OnRollCompleted { get; set; }` | Invoked when the dice come to rest. The summary carries every die in the roll along with the authoritative total from the notation library. |
| `EventCallback<Exception> OnRollFailed { get; set; }` | Invoked when an animation cannot start or cannot continue. The already-determined results remain valid, so callers should keep displaying them. |

**Properties**

| Member | Description |
| --- | --- |
| `bool IsRolling { get; }` | Gets a value indicating whether dice are currently animating. |

**Methods**

| Member | Description |
| --- | --- |
| `Task ReplayAsync(CancellationToken cancellationToken = default)` | Replays the current result. This is useful when a new roll produces the same values as the previous one, because assigning an equal result does not restart the animation on its own. |
| `Task StopAsync(CancellationToken cancellationToken = default)` | Stops the current animation without raising a completion notification. |
| `Task ResizeAsync(CancellationToken cancellationToken = default)` | Recomputes the canvas backing buffer and redraws the current scene. Call this when the surrounding layout changes in a way the browser cannot detect on its own. |
| `ValueTask DisposeAsync()` | Releases the simulation, renderer, and JavaScript module owned by this instance. |

**Animation triggers**

The component animates declaratively whenever the `Result` parameter is set to a different
instance. Because each roll produces a new result instance, this covers the normal case. Rolling
the same values twice produces a different instance as well, so the animation still restarts. Use
`ReplayAsync` when you want to replay the exact result you already have without rolling again.

---

## Namespace: d20Tek.DiceNotation.Blazor.Models

### DiceRollerOptions

Provides the configuration used by the `VisualDiceRoller` component. This groups the tray, theme,
presentation, and physics settings into a single object so the component's parameter list stays
small and can evolve without breaking consumers. Nothing in this type can influence a die result.

**Properties**

| Member | Description |
| --- | --- |
| `DiceTraySize TraySize { get; init; }` | The logical dimensions of the dice tray. Defaults to `DiceTraySize.Default`. |
| `DiceTheme Theme { get; init; }` | The theme used to draw the tray and dice. Defaults to `DiceTheme.Default`. |
| `DiceVisualSettings VisualSettings { get; init; }` | The presentation values used to turn physical state into drawing instructions. |
| `DiceSimulationSettings SimulationSettings { get; init; }` | The physics values used by the dice simulation. |
| `bool ReducedMotion { get; init; }` | A value indicating whether motion effects are disabled, which suits users who prefer reduced motion. |
| `static DiceRollerOptions Default { get; }` | A default instance. |

**Methods**

| Member | Description |
| --- | --- |
| `DiceTheme ResolveTheme()` | Gets the theme with the reduced-motion preference applied, which is the theme the renderer should actually draw with. |
| `void Validate()` | Validates every contained settings object so an invalid configuration is reported before any simulation or interop work begins. |

### DiceTraySize

Represents the logical dimensions of the dice tray in simulation units. The canvas is sized by the
browser, and the renderer scales these logical units to fit, so the tray keeps its proportions at
any display size.

| Member | Description |
| --- | --- |
| `float Width { get; init; }` | The logical width of the tray. |
| `float Height { get; init; }` | The logical height of the tray. |
| `static DiceTraySize Default { get; }` | A tray measuring eight by six units. |
| `void Validate()` | Throws `ArgumentOutOfRangeException` when either dimension is not a positive, finite number. |

### DiceTheme

Describes the colors and shapes used to draw the tray and dice.

| Member | Description |
| --- | --- |
| `string Name { get; init; }` | The theme's display name. |
| `string TrayColor { get; init; }` | The tray fill color. |
| `string TrayBorderColor { get; init; }` | The tray border color. |
| `float TrayCornerRadius { get; init; }` | The tray corner radius in logical units. |
| `string DieColor { get; init; }` | The die face color. |
| `string DieBorderColor { get; init; }` | The die border color. |
| `string DieTextColor { get; init; }` | The color used to draw face values. |
| `string DieShadowColor { get; init; }` | The color of the shadow cast by a die. |
| `float DieCornerRadius { get; init; }` | The die corner radius in logical units. |
| `string DieBevelLightColor { get; init; }` | The highlight color along the lit die edges. |
| `string DieBevelDarkColor { get; init; }` | The shade color along the unlit die edges. |
| `string DieMotionColor { get; init; }` | The color used to draw motion smear. |
| `bool UseMotionEffects { get; init; }` | Whether face churn, implied height, and motion smear are drawn. |
| `static DiceTheme Default { get; }` | The default dark slate theme. |
| `static DiceTheme DarkFantasy { get; }` | A darker, higher-contrast theme. |
| `static DiceTheme Parchment { get; }` | A light, paper-toned theme. |
| `void Validate()` | Throws when a color value is missing or a radius is invalid. |

### DiceVisualSettings

Provides the presentation values used to turn physical state into drawing instructions. These
values affect appearance only.

| Member | Description |
| --- | --- |
| `float FaceLockEnergy { get; init; }` | The energy level below which a die stops churning faces and shows its final value. |
| `float LightAngle { get; init; }` | The direction of the implied light source, in radians. |
| `float MaxLift { get; init; }` | The largest implied height applied to a fast-moving die. |
| `float BaseShadowBlur { get; init; }` | The shadow blur applied to a die at rest. |
| `float EnergyShadowBlur { get; init; }` | The additional shadow blur applied at full energy. |
| `float BaseShadowOffset { get; init; }` | The shadow offset applied to a die at rest. |
| `float EnergyShadowOffset { get; init; }` | The additional shadow offset applied at full energy. |
| `float SmearEnergy { get; init; }` | The energy level above which motion smear is drawn. |
| `int SmearSteps { get; init; }` | The number of smear copies drawn behind a moving die. |
| `float SmearReach { get; init; }` | How far behind a die the smear extends, in logical units. |
| `float BevelThickness { get; init; }` | The thickness of the die bevel, in logical units. |
| `static DiceVisualSettings Default { get; }` | A default instance. |
| `void Validate()` | Throws `ArgumentOutOfRangeException` when any value is out of range. |

### DiceSimulationSettings

Provides the physics values used by the dice simulation, including body properties, stepping, and
settlement detection. These values affect motion and timing only.

| Member | Description |
| --- | --- |
| `float WallThickness { get; init; }` | The thickness of the tray walls. |
| `float DieSize { get; init; }` | The edge length of a die, in logical units. |
| `float DieDensity { get; init; }` | The density of a die body. |
| `float Friction { get; init; }` | The friction applied to die bodies. |
| `float Restitution { get; init; }` | The bounciness applied to die bodies. |
| `float LinearDamping { get; init; }` | The damping applied to linear motion. |
| `float AngularDamping { get; init; }` | The damping applied to rotation. |
| `float MaxLaunchSpeed { get; init; }` | The largest speed given to a die at launch. |
| `float MaxLaunchSpin { get; init; }` | The largest spin given to a die at launch. |
| `float FixedStep { get; init; }` | The fixed timestep used by the simulation. |
| `int MaxCatchUpSteps { get; init; }` | The largest number of steps run to catch up after a slow frame. |
| `float MaxFrameSeconds { get; init; }` | The longest frame duration honored before time is clamped. |
| `float LinearSleepThreshold { get; init; }` | The speed below which a die is considered still. |
| `float AngularSleepThreshold { get; init; }` | The spin below which a die is considered still. |
| `float SettleHoldSeconds { get; init; }` | How long dice must stay still before the roll is considered settled. |
| `float MaxDurationSeconds { get; init; }` | The longest an animation may run before it is settled forcibly. |
| `int MaxDiceCount { get; init; }` | The largest number of dice accepted in a single roll. |
| `void Validate()` | Throws `ArgumentOutOfRangeException` when any value is out of range. |

### DiceRollSummary

Represents a completed roll prepared for display alongside the animation. The `Total` is the value
reported by the notation library and is never recomputed from `Dice`, because notation features such
as dropped dice, scalars, and bounded results mean the visible faces do not always sum to the
result.

| Member | Description |
| --- | --- |
| `IReadOnlyList<VisualDie> Dice { get; init; }` | Every die in the roll, in the order the notation produced them. |
| `int Modifier { get; init; }` | The combined value of the roll's constant terms. |
| `int Total { get; init; }` | The authoritative total reported by the notation library. |
| `string Expression { get; init; }` | The notation that produced this roll. |
| `static DiceRollSummary Empty { get; }` | An empty summary used before any roll has been made. |

### VisualDie

Represents a single die that has been prepared for animation. The assigned `Value` is determined
before the animation begins and is never changed by the simulation.

| Member | Description |
| --- | --- |
| `string Id { get; init; }` | The stable identifier used to correlate the die across snapshots. |
| `string DieType { get; init; }` | The die type label, such as `d6`. |
| `int Value { get; init; }` | The authoritative result assigned to this die. |
| `int Sides { get; init; }` | The number of faces on this die. |
| `DieRollRole Roles { get; init; }` | The role or roles this die played within its term, such as being kept, dropped, exploded, or rerolled. |
| `int MinValue { get; init; }` | The lowest face value on this die. Standard dice start at one, but fudge dice run from minus one to plus one. |

---

## Namespace: Microsoft.Extensions.DependencyInjection

### DiceRollerServiceCollectionExtensions

Provides extension methods for registering the visual dice roller services with an
`IServiceCollection`.

| Member | Description |
| --- | --- |
| `IServiceCollection AddDiceNotationBlazor(this IServiceCollection services, Action<DiceRollerOptionsBuilder>? configureRoller = null, Action<DiceNotationOptionsBuilder>? configureDice = null)` | Registers everything the `VisualDiceRoller` component needs, including the core dice notation services, so a consumer only has to make one call. |

Example with configuration:

```csharp
builder.Services.AddDiceNotationBlazor(
	roller =>
	{
		roller.Theme = DiceTheme.Parchment;
		roller.TraySize = new DiceTraySize(10f, 7f);
	},
	dice => dice.UseCryptoDieRoller());
```

The configured options are validated during registration, so an invalid value is reported at
startup rather than during a component's first render.

---

## Namespace: d20Tek.DiceNotation.Blazor.DependencyInjection

### DiceRollerOptionsBuilder

Builds the default `DiceRollerOptions` used by components that do not supply their own options.
`DiceRollerOptions` is an immutable record, so this mutable builder gives registration code a simple
assignment style without exposing settable state to the components that later read the options.

| Member | Description |
| --- | --- |
| `DiceTraySize TraySize { get; set; }` | The logical dimensions of the dice tray. |
| `DiceTheme Theme { get; set; }` | The theme used to draw the tray and dice. |
| `DiceVisualSettings VisualSettings { get; set; }` | The presentation values used when drawing dice. |
| `DiceSimulationSettings SimulationSettings { get; set; }` | The physics values used by the dice simulation. |
| `bool ReducedMotion { get; set; }` | A value indicating whether motion effects are disabled. |
| `DiceRollerOptions Build()` | Creates the validated options described by this builder. |

---

## Die shapes

Each die is drawn with a silhouette matching its type rather than as a generic square, so a
handful of mixed dice can be told apart at a glance:

| Die | Silhouette |
| --- | --- |
| d2 | Round coin |
| d4 | Triangle |
| d6 | Square |
| d8 | Diamond |
| d10 and d100 | Kite |
| d12 | Pentagon |
| d20 | Hexagon |

A die type without a conventional shape falls back to a regular polygon, so an unusual die still
renders and simulates sensibly.

A d2 is a coin rather than a many-sided die. It is built as an octagon with its corners rounded as
far as each edge allows, which reads as a circle on screen while remaining a convex polygon that
the physics engine can simulate.

Shapes are resolved in C# and sent to the renderer as plain vertex data, which keeps the shape
rules in one testable place and leaves the JavaScript layer to trace what it is given. The physics
bodies are built from the same outlines, so a die never collides as one shape while being drawn as
another.

Outlines are normalized to approximately equal area rather than equal width. A hexagon inscribed in
the same square as a triangle looks considerably larger, so without this correction a d20 would
dwarf a d4 sitting beside it in the same tray.

---

## Accessibility and reduced motion

The canvas is given a `role` of `img` and an accessible label.
label is generated from the settled result, so the outcome is available to assistive technology
without reading the canvas.

Set `ReducedMotion` to disable face churn, implied height, and motion smear. This is an explicit
option rather than an automatic media-query check, because an application usually already knows its
own accessibility preferences and can apply them consistently across its UI.

A rendering or interop failure never changes a die value. Failures are surfaced through
`OnRollFailed`, and the already-determined results remain valid, so an application should keep
displaying them.
