# Future Features

This document captures potential future enhancements for the d20Tek.DiceNotation library. The
ideas below build on the current functionality: parsing and rolling dice notation (standard dice,
percentile, fudge/FATE, keep-highest/lowest, drop-lowest, exploding/penetrating dice), grouping and
math expressions, multiple die rollers (Random, Crypto, MathNet, Constant), and roll tracking with
frequency statistics.

## Notation and Parsing Enhancements

- **Rerolls (`r` / `ro`)**: Reroll dice below a threshold, for example `4d6r1` (reroll all 1s) or
  `4d6ro1` (reroll once). Very common in tabletop roleplaying games.
- **Success/pool counting (`>=`, `<=`)**: Count dice meeting a target rather than summing, for
  example `10d6>=5` returns the number of successes. Core to games like World of Darkness and
  Shadowrun.
- **Advantage/Disadvantage sugar**: First-class `adv`/`dis` tokens (roll 2d20, keep highest/lowest)
  since D&D 5e made this pattern ubiquitous.
- **Min/Max clamping per term (`min`/`max`)**: For example, `2d6min2` treats any die below 2 as 2
  (Great Weapon Fighting style).
- **Critical detection metadata**: Flag natural max/min rolls (nat 20 / nat 1) in the result so
  consumers do not have to inspect raw rolls.

## Result and API Enhancements

- [DONE] **Verbose/structured breakdown**: A richer result model showing which dice were kept, dropped,
  rerolled, or exploded, useful for UI display and audit trails.
- [DONE] **Seeded/replayable rolling** (low priority / nice-to-have): Supply a seed to produce reproducible
  roll sequences, useful for reroll "original versus rerolled" testing and for app-level replay
  (reproducing a reported sequence or replaying a session). The main gaps this
  fills are asserting that a reroll yields a different value than the original, and runtime replay.

  Reproducibility depends on whether the underlying generator is seedable, not merely on being
  "random," so this applies to more than the pseudo-random roller. The cleaner abstraction is a roller
  wrapping a seedable source, capturing `(algorithm, seed)` as the replay token rather than a single
  `SeededDieRoller`. Per-roller support:

  - **RandomDieRoller** (supported): `System.Random` accepts a seed via `new Random(seed)` and
    produces a fully deterministic sequence. This is the natural home for seeded rolling.
  - **MathNetDieRoller** (supported, with caveats): MathNet random sources (for example
    `MersenneTwister`, `Mrg32k3a`, `SystemRandomSource`) accept a seed. Reproducibility is only
    guaranteed for the same generator type, MathNet version, and platform, so replay data must record
    which generator was used, not just the seed.
  - **CryptoDieRoller** (not supported by design): `RandomNumberGenerator` is a CSPRNG that pulls from
    OS entropy and exposes no seed, because reproducibility would undermine its security guarantees.

## Integration and Infrastructure

- [DONE] **DI extension package**: `AddDiceNotation()` service-collection extensions to register `IDice`,
  configuration, roller, and tracker cleanly.
- [DONE] **Trimming/AOT annotations**: Annotated the library with `IsTrimmable`/`IsAotCompatible` (plus
  the trim and AOT analyzers) so consumers publishing with trimming or Native AOT (for example,
  native console or game clients) get full support. This involved replacing the `dynamic`-based
  evaluator dispatch in `Evaluator` with a type-safe `switch` expression, adding a source-generated
  `DieTrackingJsonContext` (`JsonSerializerContext`) for `DieRollTracker`/`AggregateRollTracker`
  JSON serialization, and annotating `UseRollTracker<TTracker, TService>()` with
  `DynamicallyAccessedMembers` for its generic service registration.

## Performance and Allocation Enhancements

- **Reduce allocations in the parser/evaluator pipeline**: Profile `Lexer`, `Parser`, and `Evaluator`
  for intermediate list/string allocations on hot paths, especially when the same notation is rolled
  repeatedly, and consider `Span<char>`-based tokenization to cut down on allocations.
- **Expression caching/precompilation**: Cache the parsed term tree for a notation string (for
  example, keyed by the `"4d6+2"` text) so repeated rolls of the same notation skip re-parsing. This
  is especially useful for simulations that roll the same expression thousands of times.
- **Pooled buffers for term results**: Use `ArrayPool<T>` or similar pooling for `TermResult` lists in
  high-volume Monte Carlo or simulation scenarios to reduce GC pressure.

## API Enhancements

- [DONE] **`IDice` batch rolling**: Added `RollMany(string notation, int count, IDieRoller? dieRoller = null)`
  and `RollMany(DiceExpression expression, int count, IDieRoller? dieRoller = null)` returning
  `IReadOnlyList<DiceResult>` to support simulation and statistics use cases, pairing naturally with
  `IAggregateRollTracker` for aggregating results across the batch.

## Testing and Quality Infrastructure

- **Property-based/fuzz testing**: Add a lightweight generator-based or fuzz testing pass for the
  parser to catch edge cases in notation combinations beyond the hand-written unit tests.
- [DONE] **Benchmark project**: Added `benchmarks\d20Tek.DiceNotation.Benchmarks`, a BenchmarkDotNet
  project with `NotationRollBenchmarks`, `ExpressionRollBenchmarks`, and `BatchRollBenchmarks`,
  using a seeded `RandomDieRoller` for reproducible inputs. `[MemoryDiagnoser]` reports allocations
  per operation, giving a baseline to guide the parser/evaluator allocation work above and to guard
  against performance regressions over time.
- **Mutation testing**: Run a mutation testing pass (for example, with Stryker.NET) to validate the
  strength of the existing unit test suite.

## Visual Representations
- [Done] **Blazor dice roller component**: A visual dice roller for Blazor that animates the results of
  `IDice` rolls as physics-driven dice on an HTML canvas. The rolled values are determined by the
  dice library and the animation simply presents them, so the visuals never change an outcome.
- **3D dice roller component**: A visual dice roller that animates the results of
  `IDice` rolls as physics-driven dice in a 3D scene. The rolled values are determined by the
  dice library and the animation simply presents them, so the visuals never change an outcome.