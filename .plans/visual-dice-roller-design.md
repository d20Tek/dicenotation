# Visual Dice Roller for Blazor — Design Document

**Status:** Proposed
**Target package:** `DiceNotation.Blazor`
**Related package:** `DiceNotation`
**Primary target:** Blazor WebAssembly
**Rendering:** HTML Canvas with minimal JavaScript interop
**Physics:** Aether.Physics2D

---

## 1. Executive Summary

Build a reusable Blazor component that visually rolls tabletop RPG dice in a 2D dice tray. A user enters dice notation such as `4d6+2`; the existing `DiceNotation` library parses the notation and produces the authoritative individual die results and total. The component then animates the dice bouncing and settling in a tray.

The central design principle is to **separate the result from the animation**:

* **DiceNotation** determines the result exactly once.
* **The physics simulation** controls only the dice' positions, rotations, collisions, and settling.
* **The Canvas renderer** displays the current scene.
* **The Blazor component** coordinates the experience and exposes results to the consuming application.
* **Accessible HTML** presents the final results independently of the animation.

This provides a physical feel without allowing physics timing or visual randomness to change the actual dice result.

The first release should focus on a reliable, attractive 2D dice tray with readable results. It should not attempt to be a complete game engine or a photorealistic 3D dice simulator.

## 2. Goals

The component should:

* Provide a reusable visual dice-rolling experience for Blazor applications.
* Support tabletop dice notation through the existing `DiceNotation` package.
* Animate multiple dice with collisions, friction, and settling.
* Keep the physics simulation in C#.
* Use JavaScript primarily for Canvas rendering and browser integration.
* Work well in Blazor WebAssembly without server round trips during animation.
* Expose component parameters and callbacks suitable for application integration.
* Keep the simulation, result generation, and rendering layers independently testable.
* Make final results accessible and available to application code.
* Provide a foundation for future themes and visual enhancements without overcomplicating the initial release.

## 3. Non-goals

The initial version will not:

* Render true 3D dice or use WebGL/Three.js.
* Reimplement dice-notation parsing or result generation.
* Provide a complete tabletop RPG rules engine.
* Include networking or shared multiplayer rolls.
* Attempt to simulate perfectly realistic physical dice.
* Require consumers to write JavaScript to use the component.
* Promise identical physical trajectories across browsers or platforms.
* Include a full dice editor, roll-history system, or campaign-management features.

Future versions could add custom themes, die-face artwork, roll history, sound effects, and richer shapes without changing the fundamental separation between results and animation.

## 4. User Experience

A typical interaction works as follows:

1. The user enters `4d6+2`.
2. The component validates the notation.
3. `DiceNotation` produces four individual d6 results and the modifier and total.
4. The component creates four visual dice, assigning each one its already-determined result.
5. The physics simulation launches the dice into the tray.
6. The Canvas renderer draws each die at its current position and rotation.
7. The simulation settles, and the component displays the final individual results and total.
8. The component invokes an `OnRollCompleted` callback with the result.

**The displayed value on each die must remain fixed throughout the animation.** A die may rotate or bounce, but its assigned result must not be rerolled when it collides or settles.

### 4.1 Suggested visual style

* A bounded, top-down tray with rounded corners.
* Dice represented by simple square or polygonal shapes in the first version.
* Clear face values, strong contrast, and subtle shadows.
* A restrained palette with configurable themes.
* A visible indication that a roll is in progress.
* A compact result summary outside the Canvas.
* A responsive layout that works in both desktop and mobile browsers.

The first version should prioritize readable results and a convincing rolling motion over elaborate artwork.

## 5. Technology Decisions

### 5.1 Existing DiceNotation package

Keep the existing `DiceNotation` package responsible for notation parsing and random result generation.

The visual component should consume its output through a small adapter or service rather than duplicate its logic.

The exact adapter implementation must be based on the current public API of `DiceNotation`. The models and signatures in this document are proposed contracts, not claims about the existing API.

### 5.2 Aether.Physics2D

Use [Aether.Physics2D](https://nkast.github.io/Aether.Physics2D/) for rigid-body simulation.

The library provides 2D physics capabilities such as:

* Dynamic and static bodies.
* Collision detection and resolution.
* Friction and restitution.
* Convex polygon shapes.
* Angular movement.
* Sleeping and settling behavior.

Before committing to a target framework, verify that the chosen package version and its dependencies work in the intended Blazor WebAssembly runtime. A minimal browser/WASM compatibility test should happen early.

### 5.3 HTML Canvas

Use Canvas to draw the tray and dice.

Canvas is well suited to frequently changing positions and rotations without producing a large Blazor component tree or triggering a Blazor render for every physics step.

JavaScript should remain a thin rendering and browser-integration layer. It should not own dice-notation parsing, random result generation, or a second physics simulation.

### 5.4 Blazor WebAssembly

Prefer Blazor WebAssembly for the initial implementation.

The simulation can run locally, avoiding server round trips during animation. A server-hosted Blazor application may still consume the component if the package supports that hosting model, but server-side physics should not be required.

### 5.5 Responsibility summary

| Technology       | Responsibility                                                    |
| ---------------- | ----------------------------------------------------------------- |
| `DiceNotation`   | Parse notation and determine individual results and totals        |
| C# simulation    | Manage bodies, collisions, positions, rotations, and settling     |
| Aether.Physics2D | Provide the rigid-body physics engine                             |
| Blazor           | Component lifecycle, configuration, events, and accessible output |
| JavaScript       | Canvas rendering, browser frame scheduling, and interop           |
| CSS              | Component layout and non-Canvas presentation                      |

## 6. Architecture

The design has four primary layers:

1. **Dice result layer** — uses `DiceNotation` to produce authoritative outcomes.
2. **Simulation layer** — advances the physics world in C#.
3. **Rendering layer** — draws scene snapshots through JavaScript and Canvas.
4. **Blazor component layer** — coordinates input, lifecycle, configuration, callbacks, and accessible output.

### 6.1 Component architecture

```text
Application
    |
    v
VisualDiceRoller.razor
    |
    +--> IDiceRollService ------> DiceNotation
    |
    +--> IDiceSimulation -------> Aether.Physics2D
    |           |
    |           +--> DiceSceneState snapshot
    |
    +--> CanvasRenderer --------> JavaScript Canvas renderer
    |
    +--> Accessible HTML result
    |
    +--> OnRollCompleted callback
```

### 6.2 Architectural rules

* The result service is the only authority for dice values.
* Physics objects carry a stable ID and a fixed result value.
* Physics must never generate or modify results.
* The simulation layer must not depend on Blazor component types.
* The Canvas renderer receives render data, not domain decisions.
* JavaScript interop calls should be batched at frame or snapshot boundaries.
* Do not invoke JavaScript once per die or once per physics substep.
* A single scheduler should own the frame loop to avoid competing timers.
* Component instances must not share mutable physics state accidentally.

## 7. Proposed Solution Structure

The following structure is a starting point. Adjust names and files to match the repository's existing conventions.

```text
src/
  DiceNotation.Blazor/
    Components/
      VisualDiceRoller.razor
      VisualDiceRoller.razor.cs
    Models/
      DieResult.cs
      DiceRollResult.cs
      VisualDie.cs
      DieState.cs
      DiceSceneState.cs
      DiceTheme.cs
      DiceRollerOptions.cs
      DiceTraySize.cs
    Services/
      IDiceRollService.cs
      DiceRollService.cs
      IDiceSimulation.cs
      AetherDiceSimulation.cs
    Interop/
      CanvasRenderer.cs
    wwwroot/
      js/
        dice-roller.js
      css/
        dice-roller.css
    DependencyInjection/
      ServiceCollectionExtensions.cs

tests/
  DiceNotation.Blazor.Tests/

samples/
  DiceNotation.Blazor.Demo/
```

### 7.1 Package responsibilities

**`DiceNotation.Blazor`**

Contains the reusable component, simulation integration, Canvas interop, themes, and configuration.

**`DiceNotation.Blazor.Tests`**

Contains unit tests for result mapping, simulation behavior, lifecycle, and component integration.

**`DiceNotation.Blazor.Demo`**

Provides a working sample application that demonstrates installation, configuration, and usage.

Keep the initial package small. Avoid introducing extension points that are not needed by the first consumer scenarios.

## 8. Core Models

These are illustrative models. Refine them after checking the current `DiceNotation` API and deciding how the library represents modifiers and die types.

```csharp
public sealed record DieResult(
    string DieType,
    int Value);

public sealed record DiceRollResult(
    IReadOnlyList<DieResult> Dice,
    int Modifier,
    int Total);

public sealed record VisualDie(
    string Id,
    string DieType,
    int Result);

public sealed record DieState(
    string Id,
    int Value,
    float X,
    float Y,
    float Rotation,
    float Width,
    float Height);

public sealed record DiceSceneState(
    IReadOnlyList<DieState> Dice,
    bool IsSettled);

public sealed record DiceTraySize(
    float Width,
    float Height);
```

### 8.1 Model responsibilities

| Model            | Responsibility                                                   |
| ---------------- | ---------------------------------------------------------------- |
| `DieResult`      | One authoritative result from the notation engine                |
| `DiceRollResult` | Final result returned to users and application code              |
| `VisualDie`      | Die prepared for animation, including stable ID and fixed result |
| `DieState`       | Renderable state of one die at a particular moment               |
| `DiceSceneState` | Snapshot of the entire visual scene                              |
| `DiceTraySize`   | Logical dimensions used by the simulation                        |

Simulation coordinates should use logical units rather than physical device pixels.

Consider whether `DieType` should be a string, enum, or type abstraction based on the existing notation library. Avoid introducing a second dice-type system unnecessarily.

### 8.2 Result integrity

The result and simulation state must be separate.

For example, a visual die could carry the following information:

```text
Id: die-3
DieType: d6
Result: 5
Position: (142, 87)
Rotation: 1.24 radians
```

The physics simulation may update position and rotation many times, but the result remains `5`.

The renderer may display the result as a label on the die. This avoids needing to model a true 3D die with multiple faces and a physically accurate orientation in the first version.

## 9. Service Contracts

The following interfaces describe the intended boundaries. Their final signatures may change to fit the existing library's conventions.

### 9.1 Dice roll service

```csharp
public interface IDiceRollService
{
    DiceRollResult Roll(string notation);
}
```

Responsibilities:

* Parse and evaluate the requested notation through `DiceNotation`.
* Map the library's output to the component-facing result model.
* Surface invalid notation and evaluation errors in a form the component can display.
* Never depend on the visual simulation.

### 9.2 Simulation service

```csharp
public interface IDiceSimulation : IDisposable
{
    void StartRoll(
        IReadOnlyList<VisualDie> dice,
        DiceTraySize traySize);

    void Step(float elapsedSeconds);

    DiceSceneState GetSnapshot();

    bool IsSettled { get; }
}
```

Responsibilities:

* Own the active physics world.
* Create and dispose bodies for the active roll.
* Advance physics using a bounded timestep.
* Expose snapshots of die positions and rotations.
* Detect when the scene has settled.
* Keep the result assigned to each visual die unchanged.

A separate simulation instance per component instance is the simplest initial ownership model. If profiling later shows that this is expensive, optimize based on measurements rather than sharing mutable physics state prematurely.

## 10. Physics Simulation

### 10.1 Scene setup

For each roll:

1. Create or reset the physics world.
2. Add static bodies for the tray boundaries.
3. Create one dynamic body for each die.
4. Give each die a stable ID and its precomputed result.
5. Place dice within the tray without excessive overlap.
6. Apply randomized initial linear and angular velocities.
7. Step the simulation until it settles or reaches the maximum animation duration.

Start with square dice represented by simple convex polygons. This is enough to validate the overall experience. Add more accurate silhouettes or different polygonal shapes only when the core simulation feels good.

### 10.2 Tray boundaries

Create static bodies representing the four walls of the tray.

The tray should be sized in logical simulation units. Its dimensions may correspond to the rendered Canvas dimensions, but the simulation should not depend directly on device-pixel resolution.

Ensure that:

* Dice begin inside the tray.
* Walls are thick enough to avoid tunneling at expected velocities.
* High die counts do not make the tray unusable.
* Initial overlaps do not produce extreme velocities.
* Dice do not escape the tray under normal simulation conditions.

### 10.3 Material tuning

Tune friction, restitution (bounciness), damping, density, and angular damping to create a convincing tabletop feel.

The goal is not strict physical realism. The animation should feel responsive, avoid excessive bouncing, and settle in a reasonable amount of time.

### 10.4 Fixed timestep

Use a fixed physics timestep, initially around `1/60` second, with an accumulator.

The browser frame loop can vary, so each render frame may advance zero, one, or several physics steps.

Conceptually:

```text
accumulator += elapsedTime

while accumulator >= fixedStep
      and stepsThisFrame < maxCatchUpSteps:

    simulation.Step(fixedStep)
    accumulator -= fixedStep

render latest snapshot
```

Cap catch-up steps to prevent a long pause from causing a large burst of physics work.

If the page has been backgrounded, clamp or reset elapsed time instead of attempting to simulate the entire time away.

This is a conceptual example; implement it with the actual physics API and ensure only one loop controls progression.

### 10.5 Settling detection

Use physics-engine sleeping where appropriate, supplemented with explicit checks for low linear and angular velocity over a short interval.

Consider the scene settled when all dice remain below configured velocity thresholds for a sustained period.

Also define a maximum animation duration. If the physics simulation fails to settle, stop safely and present the already-determined results rather than leaving the component in a permanent rolling state.

### 10.6 Randomized motion

Initial positions and velocities may be randomized to make successive rolls feel different.

This randomness must affect only the animation.

The random source used to generate physical motion must be independent of the result-generation process. Changing the animation's random seed must never change a die's assigned result.

## 11. Simulation and Rendering Loop

Use one authoritative scheduler.

A simple initial implementation can use a component-owned animation loop or a JavaScript `requestAnimationFrame` callback that notifies C# at a controlled rate. Choose one approach and avoid running both an independent .NET timer and an independent JavaScript loop.

### 11.1 Recommended initial approach

1. JavaScript uses `requestAnimationFrame` as the browser frame scheduler.
2. Frame timing is passed to C# in a controlled way.
3. C# advances the physics simulation with the fixed-step accumulator.
4. C# produces a scene snapshot at most once per displayed frame.
5. A batched interop call sends the snapshot to the Canvas renderer.
6. JavaScript draws the tray and dice.

### 11.2 Performance considerations

Benchmark interop overhead early.

If round-tripping to C# on every animation frame is too costly, revise the scheduling boundary while preserving the principle that C# owns the simulation and JavaScript owns rendering.

Do not move the physics into JavaScript merely to work around an unmeasured concern.

The design should avoid:

* One interop call per die.
* One interop call per physics substep.
* Multiple independent frame schedulers.
* Full Blazor component rerenders for every simulation update.
* Unbounded catch-up work after a long browser pause.

## 12. Canvas Renderer

The JavaScript renderer should be deliberately small.

It should:

* Initialize the Canvas and drawing context.
* Resize the backing buffer when the element size changes.
* Account for `devicePixelRatio` while keeping simulation coordinates in logical units.
* Draw the tray, boundaries, dice silhouettes, values, and optional shadows.
* Apply each die's position and rotation around its center.
* Draw a complete scene from a snapshot.
* Dispose event handlers and animation resources when the component is removed.

Avoid embedding notation parsing, random result generation, physics rules, or business logic in JavaScript.

### 12.1 Rendering considerations

* Use logical CSS pixels for scene coordinates and scale the backing buffer for high-DPI screens.
* Keep values legible at the smallest supported component size.
* Avoid expensive per-frame DOM changes.
* Reuse drawing resources where practical.
* Support resizing while a roll is in progress.
* Decide whether resize rescales the current scene or restarts the animation.
* If a resize restarts the animation, do not reroll the dice.

### 12.2 Renderer contract

The renderer should receive a snapshot containing enough information to draw the scene:

* Tray dimensions.
* Die identifiers.
* Die positions.
* Die rotations.
* Die dimensions.
* Fixed die values.
* Theme information or resolved style data.

Prefer a compact, serializable snapshot over passing complex physics-engine objects through interop.

## 13. Blazor Component API

A first-pass API might look like this:

```razor
<VisualDiceRoller
    Notation="@_notation"
    Theme="DiceTheme.DarkFantasy"
    ShowResults="true"
    ShowTotal="true"
    OnRollCompleted="HandleRollCompleted" />
```

### 13.1 Potential parameters

| Parameter         | Purpose                                    |
| ----------------- | ------------------------------------------ |
| `Notation`        | Dice expression to roll                    |
| `Theme`           | Built-in or custom visual theme            |
| `ShowResults`     | Show individual results in accessible HTML |
| `ShowTotal`       | Show the computed total                    |
| `Disabled`        | Prevent starting a roll                    |
| `MaxDice`         | Limit the number of visual dice            |
| `Options`         | Optional advanced configuration            |
| `OnRollCompleted` | Callback when a roll completes             |
| `OnRollStarted`   | Optional callback when a roll starts       |
| `OnRollFailed`    | Optional callback or error notification    |

Avoid exposing every physics parameter as an individual component parameter. A configuration object is easier to evolve.

Keep the public API compact until usage demonstrates which customization points are valuable.

### 13.2 Component responsibilities

`VisualDiceRoller` should:

* Validate component parameters and enforce configured limits.
* Coordinate the roll service, simulation, and renderer.
* Prevent or define behavior for repeated roll requests.
* Track the rolling, settled, and error states.
* Publish the final result to application code.
* Provide accessible HTML output.
* Dispose interop references and simulation resources.
* Avoid rerendering the entire component at physics frequency.

### 13.3 Roll lifecycle

A simple lifecycle can be represented as:

```text
Idle
  |
  v
Validating
  |
  v
Rolling
  |
  v
Settling
  |
  v
Completed
```

Errors during validation should return the component to a usable state without starting the simulation.

If animation fails after a result has been calculated, preserve and display the result. Rendering should never determine whether a roll is valid or change the result.

## 14. Dependency Injection and Configuration

Provide a small registration extension, for example:

```csharp
services.AddDiceNotationBlazor();
```

The exact name and registration lifetime should follow the repository's conventions.

Register stateless services as singleton where appropriate. The simulation contains mutable state, so avoid registering one shared mutable simulation as a singleton unless it is explicitly designed to manage multiple independent scenes safely.

A scoped or component-created simulation may be more appropriate depending on the chosen lifecycle.

### 14.1 Potential options

* Default tray dimensions or aspect ratio.
* Maximum number of dice.
* Fixed physics timestep.
* Maximum catch-up steps per frame.
* Maximum roll duration.
* Settling velocity thresholds.
* Default theme.
* Whether to show individual results and totals.
* Default animation intensity.

Validate configuration and provide safe defaults. Do not expose low-level settings unless they are useful to consumers.

## 15. Error Handling and Edge Cases

Handle these cases explicitly:

* Invalid or empty notation.
* Notation that produces no visual dice.
* A modifier-only expression, if supported by the underlying library.
* More dice than the configured maximum.
* A new roll requested while another is active.
* Component disposal while a roll is running.
* Browser tab backgrounding or long frame pauses.
* Canvas resize during a roll.
* JavaScript initialization or interop failure.
* Physics simulation that never settles.
* Repeated component instances on one page.
* Unsupported or malformed custom theme values.

Error handling should not silently alter a roll.

If a roll expression is invalid, show a clear error and do not start a simulation. If animation fails after a result has been calculated, preserve and display the result.

### 15.1 Concurrent roll policy

Choose and document one behavior for requests made while a roll is active:

* Ignore the new request.
* Cancel the current animation and start the new one.
* Queue the new request.

For the first version, ignoring or replacing the active animation is simpler than maintaining a queue. Whichever policy is chosen, it must not cause duplicate completion callbacks or leak physics resources.

## 16. Accessibility

The Canvas animation must not be the only way to understand the outcome.

* Render individual results and the total as normal HTML text.
* Announce completion through an appropriate live region without announcing every animation frame.
* Support keyboard operation for starting a roll.
* Provide visible focus states.
* Respect `prefers-reduced-motion`; for reduced motion, show the result with little or no animation.
* Ensure text and controls have sufficient contrast.
* Provide an accessible name for the component and its input/button controls.
* Avoid relying on color alone to distinguish dice.
* Keep result information available after the animation completes.

Accessibility is part of the component's public contract, not a later cosmetic enhancement.

## 17. Testing Strategy

### 17.1 Unit tests: roll service

Test that:

* Valid notation maps to the expected result model.
* Individual die results and totals are preserved.
* Invalid notation is reported correctly.
* The adapter does not invent or alter results.
* The service does not depend on the simulation.

### 17.2 Unit tests: simulation

Test that:

* One physics body is created for each visual die.
* Each die retains its assigned ID and value.
* Snapshots contain finite coordinates, dimensions, and rotations.
* The simulation eventually settles under normal conditions.
* Dice remain within tray bounds within a suitable tolerance.
* Disposal is safe and idempotent where appropriate.
* A maximum-duration fallback works.
* Large elapsed-time values do not trigger unbounded catch-up work.

Do not assert exact stochastic final positions or collision sequences. Those are implementation details and can vary by platform or small changes to the physics configuration.

### 17.3 Component and integration tests

Test that:

* A roll starts and completes.
* `OnRollCompleted` receives the same result produced by the roll service.
* Invalid input displays an error without starting the animation.
* Repeated clicks follow the documented policy.
* Multiple component instances do not share mutable simulation state.
* Disposal during a roll does not produce unhandled interop errors.
* Reduced-motion mode is respected.

### 17.4 Browser tests

Use a small real-browser smoke test for:

* Canvas initialization.
* Correct resizing.
* High-DPI rendering.
* JavaScript interop.
* Animation completion.
* Component disposal.
* Browser backgrounding and resuming.

A physics unit test alone cannot validate browser runtime compatibility.

## 18. Performance and Resource Management

* Keep the default number of dice modest and define a configurable maximum.
* Use a fixed physics step and cap catch-up work.
* Send one batched snapshot per frame, not one call per die.
* Avoid rerendering the entire Blazor component on every physics step.
* Use Canvas for high-frequency visual updates.
* Dispose `IJSObjectReference` instances, event handlers, and simulation resources.
* Ensure a second roll cleans up or safely resets the first simulation.
* Test on a representative browser and device before optimizing further.
* Avoid unnecessary allocations when creating per-frame snapshots if profiling shows they matter.

Favor a clear ownership model over premature pooling or shared-world optimizations.

## 19. Security and Robustness

Although this is a client-side visual component, it still needs defensive limits:

* Limit the maximum number of dice and the complexity of accepted notation according to the underlying library's capabilities.
* Treat notation and theme input as untrusted.
* Do not build raw HTML from user-provided values.
* Avoid unbounded loops in the simulation.
* Handle failed JavaScript interop and component disposal cleanly.
* Do not let rendering failures change or recalculate the result.
* Validate configuration values before using them in physics or rendering.

## 20. Implementation Milestones

### Milestone 1 — Physics spike

Create a tiny standalone C# prototype using Aether.Physics2D:

* A rectangular tray with static walls.
* Several square dynamic bodies.
* Randomized initial positions and velocities.
* Friction, restitution, and angular movement.
* A fixed timestep.
* A settling condition and maximum duration.

**Exit criteria:** Dice collide, remain inside the tray, and settle reliably in a reasonable time. Verify that the package works in the intended browser/WASM target before investing in the component.

### Milestone 2 — Scene contract

Define the minimal scene models and simulation interface. Add tests that exercise the physics world without Blazor or Canvas.

**Exit criteria:** The simulation can start, step, report snapshots, and settle without any UI dependencies.

### Milestone 3 — Static Canvas rendering

Create the Canvas JavaScript module and draw a static tray and a few dice from a supplied snapshot.

**Exit criteria:** Correct resizing, high-DPI rendering, legible values, and reliable initialization and disposal.

### Milestone 4 — Live simulation rendering

Connect the C# simulation to Canvas snapshots using a single frame scheduler and batched interop.

**Exit criteria:** Smooth animation with bounded work per frame and no unbounded interop traffic.

### Milestone 5 — DiceNotation integration

Adapt the existing notation library's actual output to the component result model. Keep results fixed while dice animate.

**Exit criteria:** Each visual die displays the correct precomputed result, and the final total matches the library.

### Milestone 6 — Lifecycle and error resilience

Handle repeated rolls, invalid notation, resize, background tabs, disposal, maximum dice, and animation timeout.

**Exit criteria:** No permanent rolling state or unhandled disposal errors in normal edge cases.

### Milestone 7 — Reusable package

Finalize component parameters, DI registration, options, themes, and public XML documentation.

**Exit criteria:** A consumer can install the package and add the component with minimal setup.

### Milestone 8 — Demo, tests, and release preparation

Create a sample app and package documentation. Include setup instructions, supported targets, examples, accessibility notes, and known limitations.

**Exit criteria:** The demo exercises the main scenarios and the package is ready for a first release.

## 21. Risks and Mitigations

| Risk                                                                                                  | Mitigation                                                                                 |
| ----------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------ |
| Aether.Physics2D is incompatible with the selected WASM target or adds unexpected runtime constraints | Verify with a browser/WASM spike before building the component                             |
| Frequent C# ↔ JS calls cause frame drops                                                              | Batch snapshots and benchmark early; use one scheduler                                     |
| Dice fail to settle consistently                                                                      | Tune material properties, use velocity thresholds, and enforce a maximum duration          |
| Visual values become confused with physical face orientation                                          | Treat each result as fixed metadata and render that value explicitly                       |
| Public API becomes too broad too early                                                                | Start with a small parameter set and add options based on real use                         |
| Canvas-only output is inaccessible                                                                    | Provide accessible HTML results and reduced-motion support                                 |
| Multiple components interfere with one another                                                        | Give each component its own simulation state and lifecycle                                 |
| Browser backgrounding causes long simulation pauses                                                   | Clamp elapsed time and cap catch-up steps                                                  |
| Component disposal races with JS callbacks                                                            | Make shutdown explicit, cancel scheduling, and handle expected interop-disposal exceptions |

## 22. Open Decisions

Resolve these during implementation rather than over-designing them up front.

1. **Notation integration:** Which exact public types and methods does the current `DiceNotation` package expose for individual die outcomes, modifiers, and totals?
2. **Initial die set:** Should the first release support only d4/d6/d8/d10/d12/d20/d100, or every die type accepted by `DiceNotation`?
3. **Result visualization:** Should the value always be printed on the top face, or should the first version focus on readable result labels near each die?
4. **Roll policy:** Should a new roll be ignored, queued, or cancel the active animation?
5. **Frame scheduling:** Does a C#-driven loop with batched snapshots perform adequately in the target browser, or should the browser own more of the scheduling boundary?
6. **Customization:** Which themes and sizing controls are essential for the first release?
7. **Package target frameworks:** Which .NET and Blazor targets are supported by the package and selected physics dependency?
8. **Component input model:** Should the component accept notation only, or also expose an overload/API for callers that already have a calculated result?

## 23. Recommended First Implementation

Start with **Milestone 1: the physics spike** before building the Blazor component.

Keep the prototype intentionally small:

* A tray.
* Four to six square bodies.
* Basic collision materials.
* A fixed timestep.
* A settling condition.
* A maximum simulation duration.

Log or draw positions only as needed to inspect behavior. Confirm that the chosen Aether.Physics2D package runs in the target environment.

Once the simulation feels reliable, define the scene contract and build the Canvas renderer. This reduces the risk of spending time on UI and package design before the core animation is viable.

The first meaningful end-to-end demonstration should be deliberately modest: roll a few preassigned values, animate them in the tray, and confirm that the final values never change.

## 24. References

* [Aether.Physics2D documentation](https://nkast.github.io/Aether.Physics2D/)
* [Aether.Physics2D source repository](https://github.com/tainicom/Aether.Physics2D)
* [Aether.Physics2D on NuGet](https://www.nuget.org/packages/Aether.Physics2D/)

Check the package's current target frameworks, version, and dependency requirements when creating the prototype; those details may change over time.

## 25. Implementation Plan

The following steps implement this design within the existing `d20Tek.DiceNotation` solution, using the
result-rendering component API described in section 25.1.

| #  | Step |
| -: | :--- |
| 1  | Spike Aether.Physics2D under Blazor WebAssembly to verify it loads and steps a world in the browser on net9.0/net10.0. |
| 2  | Record the API deviation from the design document, replacing the roll-owning component with the result-rendering component and its rationale. |
| 3  | Create the `src\d20Tek.DiceNotation.Blazor` Razor class library project with multi-targeting and packaging properties mirroring the core package, and add it to the solution. |
| 4  | Add the core model records under `Models` (VisualDie, DieState, DiceSceneState, DiceTraySize). |
| 5  | Add `DiceRollerOptions` and `DiceTheme` configuration types with validated safe defaults. |
| 6  | Implement `DiceResultDisplayBase` with the shared display parameters and the RollId/Result change-detection rule. |
| 7  | Implement the internal `VisualDieMapper` translating `DiceResult` into visual dice, honoring die roles and enforcing MaxDice. |
| 8  | Define the `IDiceSimulation` contract per design section 9.2. |
| 9  | Implement `AetherDiceSimulation` with tray walls, per-die bodies, fixed-timestep accumulator with capped catch-up, settle detection, and max-duration fallback. |
| 10 | Write the `wwwroot/js/dice-roller.js` ES module owning canvas init, resize/devicePixelRatio handling, requestAnimationFrame scheduling, and snapshot drawing. |
| 11 | Add `wwwroot/css/dice-roller.css` for component layout and accessible non-canvas presentation. |
| 12 | Implement `Interop\CanvasRenderer.cs` wrapping the JS module with batched snapshot calls and async disposal. |
| 13 | Implement `VisualDiceRoller.razor` markup with canvas, accessible results live region, and focus support. |
| 14 | Implement `VisualDiceRoller.razor.cs` coordinating the animation lifecycle, error-result handling, reduced-motion handling, OnAnimationCompleted, and disposal. |
| 15 | Implement a `DiceTextDisplay` component on the shared base to validate renderer substitutability. |
| 16 | Add an optional `AddDiceNotationBlazor()` extension for default options and theme registration, or omit it if it carries no weight. |
| 17 | Create the `tests\d20Tek.DiceNotation.Blazor.UnitTests` MSTest + bUnit project with a `Fakes` folder, and add it to the solution. |
| 18 | Write exhaustive unit tests for `VisualDieMapper` covering roles, exploding dice, MaxDice, empty and errored results. |
| 19 | Write unit tests for the display base change-detection rule, including identical consecutive results and re-passed instances. |
| 20 | Write unit tests for `AetherDiceSimulation` covering body creation, ID/value preservation, finite snapshots, settling, bounds, disposal idempotency, and catch-up clamping. |
| 21 | Write bUnit component tests for the animation lifecycle, callbacks, errored results, rapid RollId changes, multi-instance isolation, reduced motion, and disposal mid-animation. |
| 22 | Create the `samples\BlazorDice` Blazor WebAssembly standalone app, add it to the solution, and register `AddDiceNotation()`. |
| 23 | Build a demo page where the app owns `IDice`, performs rolls, and feeds results to the component, demonstrating themes, roll tracking, and multiple instances. |
| 24 | Run the full build and test suite, and fix any failures. |
| 25 | Add api-reference documentation for the new package under `docs\` and update `CHANGELOG.md` and `ReleaseNotes.md`. |

Step 1 is a gate. If Aether.Physics2D does not run under the WebAssembly runtime, steps 8 and 9 pivot to a
minimal custom simulation behind the same `IDiceSimulation` contract, and the remaining steps are unaffected.

### 25.1 API deviation from this document

Sections 4, 9.1, and 13 describe a component that owns the roll: it accepts a `Notation` parameter, calls a
`IDiceRollService` adapter, and raises `OnRollCompleted`. The implementation instead takes the already
calculated result as a parameter:

| Design document | Implementation |
| :-------------- | :------------- |
| `Notation` parameter | `Result` (`DiceResult`) and `RollId` (`Guid`) parameters |
| `IDiceRollService` / `DiceRollService` | Removed from the public API; mapping becomes the internal `VisualDieMapper` |
| `DieResult` / `DiceRollResult` models | Not introduced; the component consumes `DiceResult` from the core library |
| `OnRollCompleted` | `OnAnimationCompleted` |

The rationale is that the component becomes structurally incapable of producing or altering a result, which
enforces the separation principle in section 1 more strongly than convention alone. It also makes the
component substitutable: a visual roller, a text display, and a future 3D renderer are all implementations of
"render a `DiceResult`", sharing the `DiceResultDisplayBase` contract. Finally, it composes with existing
library features that a roll-owning component cannot express well, including `RollMany`, roll tracking,
seeded rollers and replay tokens, and reuse of a pre-built `DiceExpression`.

The tradeoff is that consumers must resolve `IDice` and perform the roll themselves. A thin composing
component that owns an `IDice` and renders a `VisualDiceRoller` can be added later if drop-in convenience
proves valuable.

This also answers open question 8 in section 22: the component accepts a calculated result only.
