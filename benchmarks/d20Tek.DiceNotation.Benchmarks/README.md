# d20Tek.DiceNotation.Benchmarks

This project contains [BenchmarkDotNet](https://benchmarkdotnet.org/) benchmarks for the
`d20Tek.DiceNotation` library. It measures execution time and allocations across the parsing,
evaluation, and rolling pipeline, to guide performance and allocation reduction work and to guard
against regressions over time.

## Benchmarks

- `NotationRollBenchmarks`: Parses and rolls dice notation strings of increasing complexity
  (`1d20`, `4d6+2`, `4d6k3+1d8+5`, `(2+1)d4+2d6!6-3`), covering the full lexer, parser, and
  evaluator pipeline.
- `ExpressionRollBenchmarks`: Rolls a pre-built `DiceExpression`, isolating the evaluator/roll
  pipeline from the lexer and parser.
- `BatchRollBenchmarks`: Rolls the same notation repeatedly via `IDice.RollMany` for batch sizes of
  10, 100, and 1000, representative of simulation and statistics workloads.

All benchmarks use a seeded `RandomDieRoller` so that results are based on a reproducible sequence
of rolls.

## Running the Benchmarks

Benchmarks must be run in the `Release` configuration.

```
dotnet run -c Release --project benchmarks\d20Tek.DiceNotation.Benchmarks
```

To run a specific benchmark class, use the `--filter` option:

```
dotnet run -c Release --project benchmarks\d20Tek.DiceNotation.Benchmarks --filter *NotationRollBenchmarks*
```

Results, including allocation data from `[MemoryDiagnoser]`, are written to the console and to
`BenchmarkDotNet.Artifacts/results` as Markdown, CSV, and HTML reports. The `BenchmarkDotNet.Artifacts`
folder is gitignored since it contains full per-run diagnostics that do not need to be tracked.

## Tracking Results Over Time (Regression Detection)

To guard against speed/allocation regressions without committing full diagnostic artifacts, use
`Update-BenchmarkHistory.ps1`. It runs the benchmarks, then appends one trimmed row per
benchmark/parameter combination (timestamp, git commit, class, method, params, mean time,
allocated bytes/op) to `benchmarks\BenchmarkHistory.csv`, which is tracked in source control.

```
.\benchmarks\d20Tek.DiceNotation.Benchmarks\Update-BenchmarkHistory.ps1
```

To scope to a subset of benchmarks:

```
.\benchmarks\d20Tek.DiceNotation.Benchmarks\Update-BenchmarkHistory.ps1 -Filter "*NotationRollBenchmarks*"
```

Diff `BenchmarkHistory.csv` against a prior commit (or open it in Excel/a CSV viewer) to compare
`Mean_ns`/`Allocated_bytes` across runs and confirm a change improved, rather than regressed,
performance.
