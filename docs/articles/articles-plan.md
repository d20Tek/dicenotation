# Visual Dice Roller Content Plan

This document captures the planned articles and social posts for the `d20Tek.DiceNotation.Blazor`
visual dice roller release. Each entry notes the format that fits the content, the source material in
the repository, and the key points to cover.

The content is split by length and audience fit. Topics that need code, diagrams, or sustained
explanation become blog articles. Topics that land in a single image, snippet, or claim become X
posts, which then link back to the related article.

## Status legend

- `Published` - written and released.
- `Drafted` - written but not yet released.
- `Planned` - not yet started.

## Blog articles

### 1. Building a Physics-Based Visual Dice Roller for Blazor

- **Status:** Drafted
- **File:** [building-a-physics-based-visual-dice-roller-for-blazor.md](building-a-physics-based-visual-dice-roller-for-blazor.md)
- **Role:** Flagship release article and the anchor every other piece links back to.
- **Covers:** The "results drive visuals" principle, the five-layer architecture, fixed-step physics
  against browser frames, face churn, shape geometry, configuration, disposal, accessibility,
  testability, and the spike-first process.
- **Source:** The whole `src/d20Tek.DiceNotation.Blazor` project.

### 2. The Result Is Already Decided: Separating Randomness from Animation

- **Status:** Planned
- **Role:** The design-principle deep dive. The single most transferable idea in the project.
- **Covers:** Why a physics engine must not decide which face lands up; auditability and
  reproducibility requirements for a game application; how `VisualDieFactory` and
  `DieVisualCalculator` choose motion that lands on a predetermined value; why the animation `Random`
  is deliberately independent of the die-roll source, and how sharing it would break seeded replay;
  why every failure path is therefore cosmetic rather than a correctness bug.
- **Source:** `VisualDiceRoller.razor.cs`, `VisualDieFactory.cs`, `DieVisualCalculator.cs`,
  `AetherDiceSimulation.cs` constructor documentation.

### 3. Wrapping Aether.Physics2D in a Testable Abstraction

- **Status:** Planned
- **Role:** Physics engineering piece for the game-development audience.
- **Covers:** The `IDiceSimulation` seam and what it deliberately hides; building a tray from four
  static walls; dice as dynamic bodies; rest detection through `LinearSleepThreshold`,
  `AngularSleepThreshold`, and `SettleHoldSeconds`; why a hold time is needed when a bouncing die
  passes through zero velocity; the `MaxDurationSeconds` escape hatch; and the hard stop applied once
  the scene settles so the final frame is clean.
- **Source:** `AetherDiceSimulation.cs`, `IDiceSimulation.cs`, `DiceSimulationSettings.cs`.

### 4. Fixed-Timestep Simulation Against a Browser Frame Loop

- **Status:** Planned
- **Role:** The most technically interesting standalone post; useful well beyond dice.
- **Covers:** Why physics wants a fixed step and browsers give a variable delta; the accumulator
  pattern; rejecting non-finite and negative deltas before they permanently corrupt a physics world;
  `MaxCatchUpSteps` and the spiral of death; discarding the backlog so a pause does not cause
  slow-motion playback; and clamping at the JavaScript boundary with `MaxFrameSeconds` for the
  backgrounded-tab case.
- **Source:** `AetherDiceSimulation.Step`, `DiceAnimationLoop.OnFrameAsync`,
  `CanvasRenderer.StartLoopAsync`, `DiceSimulationSettings.cs`.

### 5. Blazor JS Interop for 60fps Canvas Rendering Without Re-Rendering

- **Status:** Planned
- **Role:** The Blazor-specific piece with the broadest .NET reach.
- **Covers:** Importing a scoped `IJSObjectReference` module instead of polluting `window`; the
  integer handle that lets several rollers coexist on one page; batching a whole scene into one
  interop call per displayed frame rather than one per die; why interop traffic stays flat as dice
  count grows; sending resolved drawing instructions rather than physics state so the JavaScript
  holds no policy; and why the component calls `StateHasChanged` exactly once per animation.
- **Source:** `CanvasRenderer.cs`, `RenderScene.cs`, `wwwroot/js/dice-roller.js`,
  `VisualDiceRoller.RaiseCompletedAsync`.

### 6. Deterministic Unit Tests for Animation Code

- **Status:** Planned
- **Role:** Testing practice piece; pairs naturally with the test-count social post.
- **Covers:** The three seams that made testing possible (`IDiceSimulation`, `FakeJSRuntime`,
  `SequenceDieRoller`); driving a whole animation by calling `OnFrameAsync` in a loop with chosen
  deltas; asserting interop batching as a regression test rather than an architectural claim; feeding
  hostile values such as `NaN` and multi-minute pauses as one-line tests; and splitting large suites
  into focused classes by concern.
- **Source:** The `tests/d20Tek.DiceNotation.Blazor.UnitTests` project, especially the `Fakes` folder
  and the four `DiceAnimationLoop*Tests` classes.

### 7. Async Disposal and Lifecycle Correctness in Blazor Components

- **Status:** Planned
- **Role:** Hard-won correctness piece; the kind of content that gets bookmarked.
- **Covers:** `IAsyncDisposable` in a component; why disposal races with navigation and circuit
  teardown; stopping the frame scheduler before releasing the module so no queued callback runs
  against disposed state; catching a named set of interop exceptions instead of a blanket
  `catch (Exception)`; single ownership so nothing is disposed twice or leaked; `ObjectDisposedException.ThrowIf`
  guards on public entry points; and why `StopAsync` on a stopped loop is a no-op rather than an error.
- **Source:** `VisualDiceRoller.DisposeAsync`, `DiceAnimationLoop.DisposeAsync`,
  `CanvasRenderer.DisposeAsync`, `DiceAnimationLoopLifecycleTests.cs`.

### 8. Designing a Configuration API Developers Actually Like

- **Status:** Planned
- **Role:** API design piece for library authors.
- **Covers:** Grouping settings into `DiceRollerOptions` to keep the parameter list small and
  evolvable; immutable records and `with` expressions; the mutable `DiceRollerOptionsBuilder` that
  gives registration an assignment style without exposing settable state to components; fail-fast
  validation at registration rather than at first render; three-tier precedence from component
  parameter to registered default to library default; and the DI lifetime reasoning behind a singleton
  options record, a scoped factory, and per-component loops.
- **Source:** `DiceRollerOptions.cs`, `DiceTheme.cs`, `DiceVisualSettings.cs`,
  `DiceSimulationSettings.cs`, `DiceTraySize.cs`, `DiceRollerOptionsBuilder.cs`,
  `DiceRollerServiceCollectionExtensions.cs`.

### 9. Spikes Before Shipping: Validating Physics and Animation Early

- **Status:** Planned
- **Role:** Engineering process and culture piece; ages well and reaches a wider audience.
- **Covers:** What `D20Tek.DiceNotation.PhysicsSpike` and `D20Tek.DiceNotation.BlazorSpike` were built
  to answer; tuning damping and settle behavior in a console application with no browser in the loop;
  how the tuned values became the shipped `DiceSimulationSettings` defaults; documenting where
  magic numbers came from so they are not treated as arbitrary later; and why the spikes were kept in
  the repository instead of deleted.
- **Source:** `spikes/D20Tek.DiceNotation.PhysicsSpike`, `spikes/D20Tek.DiceNotation.BlazorSpike`,
  `DiceSimulationSettings.cs` remarks.

### 10. Accessible Canvas: Making a Visual Dice Roller Screen-Reader Friendly

- **Status:** Planned
- **Role:** Accessibility piece; short, practical, and widely applicable to canvas work.
- **Covers:** Why a canvas is opaque to assistive technology; generating a descriptive label from the
  settled roll summary; the single end-of-animation re-render that updates that label; reduced motion
  as an explicit option rather than an automatic media-query check, so the component agrees with its
  host application; what reduced motion actually disables; and the `InvariantCulture` aspect-ratio bug
  that would silently break layout in comma-decimal locales.
- **Source:** `VisualDiceRoller.AccessibleLabel`, `VisualDiceRoller.AspectRatio`,
  `DiceRollerOptions.ReducedMotion`, `DiceRollerOptions.ResolveTheme`.

### 11. Shape Matters: Drawing Dice That Collide the Way They Look

- **Status:** Planned
- **Role:** Short, highly visual piece with strong screenshot potential.
- **Covers:** `DieGeometry` as the single source of truth shared by the renderer and the physics
  bodies; choosing recognizable silhouettes for each die type; normalizing outlines by area rather
  than bounding box so a d20 does not dwarf a d4; per-shape corner rounding because narrow corners
  erode faster than square ones; the saturated corner radius that turns the d2 octagon into a coin
  with no special case in the renderer; and caching immutable outlines behind .NET 9's `Lock` type.
- **Source:** `DieGeometry.cs`, `DieGeometryTests.cs`.

## X posts

### Launch and demo

1. **Demo GIF.** Dice tumbling in the tray with one line: "Real 2D physics, in Blazor WebAssembly, no
   JS framework." Highest-reach post. Pin it.
2. **Three-line setup.** Screenshot of `builder.Services.AddDiceNotationBlazor();` plus
   `<VisualDiceRoller Result="_result" />` with "That's the whole setup."
3. **Release announcement.** NuGet package link when it ships, with the `PackageReadme.md` feature
   list condensed to four bullets.

### Principle and opinion

4. **Hot take.** "Your dice animation should never decide the result. Roll first, animate second."
   Links to article 2.
5. **The subtle one.** "The animation uses a different `Random` than the dice. If it did not, simply
   rendering a roll would break seeded replay." Links to article 2.
6. **Poll.** "Dice roller animation: canvas, SVG, CSS transforms, or WebGL?" Cheap engagement, then
   reply with why canvas won. Links to article 5.

### Code and technique

7. **Accumulator screenshot.** The `Step` loop with the backlog-dropping comment visible. "The bug
   everyone hits with requestAnimationFrame plus physics." Links to article 4.
8. **NaN warning.** "One non-finite delta permanently corrupts a physics world. Every position becomes
   NaN and your objects vanish. Validate at the boundary." Links to article 4.
9. **Interop batching.** "One interop call per die per frame: 720/sec with 12 dice. One batched scene
   per frame: 60/sec, flat regardless of dice count." Links to article 5.
10. **Disposal catch filter.** Screenshot of the named four-exception `when` clause. "A blanket catch
	in DisposeAsync hides real bugs forever." Links to article 7.
11. **Culture bug.** The `InvariantCulture` aspect-ratio line. "This renders `aspect-ratio: 8,0 / 6,0`
	in half of Europe and CSS silently ignores it." Links to article 10.

### Visual and before/after

12. **Settle hard-stop.** Before/after clip of the jittery final frame versus a clean rest. Links to
	article 3.
13. **Equal area versus equal width.** Two tray screenshots showing a d4 next to a d20 under each
	normalization. The difference is obvious and needs no explanation. Links to article 11.
14. **Theme showcase.** Three or four images of `Default`, `DarkFantasy`, `Parchment`, and a custom
	theme side by side. Links to article 8.
15. **Spike screenshot.** The physics spike console output with "this ugly console app saved me two
	weeks." Links to article 9.

### Testing

16. **Test-count post.** Screenshot of the green run with "about 4,500 lines of tests against about
	2,500 lines of source. Animation code *is* testable if you abstract the clock and the JS runtime."
	Links to article 6.
17. **Testability seam.** "`OnFrameAsync(double)` is a plain method. A test drives an entire animation
	by calling it in a loop with whatever deltas it likes, including a 600-second one." Links to
	article 6.

### Thread

18. **Architecture thread (4-5 posts).** One post per layer with an image each: component, animation
	loop, simulation, renderer, JavaScript. Functions as the teaser for article 1 and the best single
	piece of long-form social content in the plan.

## Suggested sequencing

1. **Launch week.** Post 1 (demo GIF) and post 2 (setup snippet) to build interest, then publish
   article 1 as the anchor and promote it with thread 18.
2. **Weeks 2-5.** One deep dive per week in order 4, 3, 5, 2, each seeded a day earlier by its paired
   social post (7, 12, 9, 4).
3. **Weeks 6-8.** Articles 6 and 8 to reach the broader .NET audience beyond the game-development
   crowd, seeded by posts 16 and 14.
4. **Ongoing.** Articles 7, 9, 10, and 11 are evergreen and can be released at a lower cadence
   whenever the schedule allows. Article 11 is the best filler because it is short and visual.
5. **Package release.** Post 3 whenever the NuGet package actually ships, independent of the article
   schedule.

## Notes

- Every X post should link to its related article once that article is live. Posts released before
  their article should link to article 1 instead.
- Articles 2, 4, 5, and 10 each expand a section that already exists in article 1. When writing them,
  go deeper rather than repeating, and link back to article 1 for the overall context.
- Screenshots of code should use a consistent theme and font across all posts so the series reads as
  one body of work.
