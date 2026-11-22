# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository overview

Three NuGet packages plus samples, benchmarks, and spikes, all in `dicenotation.sln`:

- `src/d20Tek.DiceNotation` (package `d20tek-dicenotation`): parses, evaluates, and rolls dice notation
  (`4d6k3+d8+5`). Targets `net9.0;net10.0`. Marked `IsTrimmable` / `IsAotCompatible` with the trim and
  AOT analyzers on, so new code must stay reflection-free (no `dynamic`, source-generated JSON only).
- `src/d20Tek.DiceNotation.MathNet` (package `d20tek-dicenotation-mathnet`): `MathNetDieRoller`, its
  replay provider, and builder extensions, kept out of core so core has no MathNet.Numerics dependency.
  Its DI extensions live in the `Microsoft.Extensions.DependencyInjection` namespace, like `AddDiceNotation`.
  Its tests are in `d20Tek.DiceNotation.UnitTests`, not a separate project.
- `src/d20Tek.DiceNotation.Blazor` (package `d20tek-dicenotation-blazor`): the `VisualDiceRoller`
  component that animates an already-computed `DiceResult` as physics-driven dice on a canvas.
  Targets `net9.0;net10.0`; the net9.0 build pins `Microsoft.AspNetCore.Components.Web` with a
  `VersionOverride` in the csproj.
- `tests/`: two MSTest projects (both `net10.0`), one for the core and MathNet packages and one for Blazor,
  plus `d20Tek.DiceNotation.AotSmoke`, a console
  app that the `aot-smoke` workflow publishes with Native AOT (with warnings as errors) and runs on pull
  requests. When you add public API that could depend on reflection, add a check to its `Program.cs`. Publishing
  it locally needs the Visual Studio C++ workload.
- `samples/DiceCli` (Spectre.Console CLI) and `samples/BlazorDice` (Blazor WebAssembly demo).
- `benchmarks/d20Tek.DiceNotation.Benchmarks`: BenchmarkDotNet suite with a tracked
  `benchmarks/BenchmarkHistory.csv` (see that project's README for `Update-BenchmarkHistory.ps1`).
- `spikes/`: throwaway prototypes for the Blazor work. The library exposes internals to
  `D20Tek.DiceNotation.BlazorSpike`.
- `.plans/`: design docs (`visual-dice-roller-design.md`) and the feature backlog
  (`future-features.md`, items marked `[DONE]` as they land). Check these before designing a feature.
- `docs/`: user-facing docs, including the notation spec (`DiceNotationSpecCurrent.md`) and API
  references. Update them when the public API or notation changes.

Package versions are managed centrally in `Directory.Packages.props` (csproj files use
`<PackageReference>` without versions). `Directory.Build.props` turns on nullable with nullable warnings
as errors.

## Commands

```powershell
dotnet build                                   # whole solution (packages are generated on build)
dotnet test                                    # all tests (what CI runs)
dotnet test --project tests/d20Tek.DiceNotation.UnitTests
dotnet test --project tests/d20Tek.DiceNotation.Blazor.UnitTests

# single test or class
dotnet test --project tests/d20Tek.DiceNotation.UnitTests --filter "FullyQualifiedName~ParserTests"
dotnet test --project tests/d20Tek.DiceNotation.UnitTests --filter "Name=Roll_WithSingleDieExpression"

# benchmarks (Release only)
dotnet run -c Release --project benchmarks/d20Tek.DiceNotation.Benchmarks --filter *NotationRollBenchmarks*

# samples
dotnet run --project samples/DiceCli -- roll 4d6k3
dotnet run --project samples/BlazorDice
```

The test projects use `MSTest.Sdk` on Microsoft.Testing.Platform. `global.json` pins the MSTest.Sdk
version and sets the `dotnet test` runner, so a test project path needs `--project` (a positional path
does not work). Coverage, with a separate results directory per project:

```powershell
dotnet test --project tests/d20Tek.DiceNotation.UnitTests --results-directory TestResults/core -- --coverage --coverage-output-format cobertura --coverage-output c.xml
dotnet test --project tests/d20Tek.DiceNotation.Blazor.UnitTests --results-directory TestResults/blazor -- --coverage --coverage-output-format cobertura --coverage-output c.xml
```

`tests/Directory.Build.props` copies the shared `tests/testconfig.json` next to each test assembly. It
includes the test assemblies in coverage and excludes generated `.g.cs` sources (the System.Text.Json
and Regex source generators). The Blazor report also lists the core library with low coverage; ignore
it there, since the core library's own test project measures it.

CI (`.github/workflows/ci-build.yml`) runs restore/build/test on ubuntu with .NET 10. Pushing a `vX.Y.Z`
tag on a commit in `main` runs `nuget-release.yml`, which builds with `/p:Version` from the tag, tests,
packs, and publishes to GitHub Packages and nuget.org.

## Core library architecture

`IDice` / `Dice` is the entry point. There are two paths to a `DiceResult`:

1. **Notation string**: `Dice.Roll(string)` → `Parser.Evaluator` → `Lexer` (tokens) → `Parser`, a Pratt
   parser driven by `Parselets/ParseletTable` (prefix and infix parselets keyed by `TokenKind`, with
   binding power from `Precedence`) → an `Expression` tree (`Parser/Expressions.cs`). `Evaluator`
   dispatches each node to an `Evaluatorlets/*Evaluator` with a type `switch` (it replaced `dynamic` for
   AOT). Dice modifiers (keep, drop, explode, and so on) are parsed by `ModifierParser` and applied by
   `ModifierEvaluator`. `ParseException` / `EvalException` are caught in `Evaluator.Evaluate` and turned
   into an error `DiceResult` instead of being thrown.
2. **Fluent expression**: `DiceExpression.Create().AddDice(...).AddConstant(...)` builds
   `DiceTerms/IExpressionTerm`s (`DiceTerm`, `FudgeDiceTerm`, `ConstantTerm`) that compute their own
   `TermResult`s.

Adding notation means touching the lexer/`TokenKind`, a parselet and its `ParseletTable` entry, an
`Expression` node, an evaluatorlet plus its case in `Evaluator.EvalInternal`, and the spec doc.

`Results/`: `DiceResult` holds the value, per-die `TermResult`s (with `DieRollRole` for
kept/dropped/exploded breakdowns), the roller name, and any error.

`DieRoller/`: `IDieRoller` implementations (`RandomDieRoller`, `CryptoDieRoller`, `ConstantDieRoller`,
and `MathNetDieRoller` in the MathNet package) share `RandomDieRollerBase`. `IReplayableDieRoller` /
`ReplayToken` support seeded, reproducible rolls (Crypto is not replayable by design). `DieRollerFactory`
builds `Random` rollers itself and every other algorithm through an `ISeededDieRollerProvider` (passed in,
or registered with `AddSeededDieRollerProvider` for DI replay), which is how the MathNet package plugs in. `DieRollTracker` / `AggregateRollTracker` record
frequency statistics and serialize through the source-generated `DieTrackingJsonContext`.
`DieRollerFactory` builds rollers by name.

`DependencyInjection/`: `AddDiceNotation()` plus `DiceNotationOptionsBuilder` register `IDice`,
configuration, the default roller, and trackers.

## Blazor package architecture

The core rule: **the component never rolls dice.** `VisualDiceRoller` takes a finished `DiceResult` and
only animates it, so physics can never change an outcome.

- `Components/VisualDiceRoller`: coordinates everything and owns one loop, simulation, and canvas per
  instance. It does not re-render per frame. Failures go to `OnRollFailed`.
- `Services/VisualDieFactory` maps `TermResult`s to `VisualDie` models. `DieGeometry` and
  `DieVisualCalculator` hold the pure shape and face math for d2 through d100.
- `Services/IDiceSimulation` / `AetherDiceSimulation`: the Aether.Physics2D world with fixed timestep
  stepping and settling detection, produced as a `DiceSceneState`.
- `Services/DiceAnimationLoop` (built by `IDiceAnimationLoopFactory`) drives frames. The browser's
  `requestAnimationFrame` in `wwwroot/js/dice-roller.js` calls back into `OnFrameAsync`, which steps the
  simulation and pushes a `RenderScene` to the canvas through `Interop/CanvasRenderer` (a thin
  JS-module wrapper; drawing lives in the JS file).
- `Models/`: immutable options (`DiceRollerOptions`, `DiceTheme` presets, `DiceTraySize`, visual and
  simulation settings). `AddDiceNotationBlazor()` in `DependencyInjection/` registers the core library
  and these services together.

Most of the Blazor types are `internal` and tested through `InternalsVisibleTo`. Tests use bUnit with
fakes in `tests/d20Tek.DiceNotation.Blazor.UnitTests/Fakes` (`FakeJSRuntime`, `FakeDiceSimulation`,
`FakeDiceAnimationLoopFactory`, `SequenceDieRoller`, `VisualDiceRollerContext`), which replace JS interop
and physics.

## Test conventions in this repo

- Large test classes are split into partial files by topic (`DiceTests.RollMany.cs`,
  `DiceTermTests.Errors.cs`).
- Domain assertion helpers live in `tests/d20Tek.DiceNotation.UnitTests/Helpers` (`DiceAsserts`,
  `TermResultsAsserts`, `DieTrackingDataAsserts`) and `Fakes/ElementAssertions` for bUnit. Use them
  before writing new raw asserts.
- Use `SequenceDieRoller` or `ConstantDieRoller` for deterministic rolls.
