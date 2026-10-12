using BenchmarkDotNet.Attributes;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Benchmarks;

/// <summary>
/// Benchmarks batch rolling via <see cref="IDice.RollMany(string, int, IDieRoller?)"/> for
/// different batch sizes, representative of simulation and statistics workloads.
/// </summary>
[MemoryDiagnoser]
public class BatchRollBenchmarks
{
    private IDice _dice = null!;
    private IDieRoller _roller = null!;

    [Params(10, 100, 1000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _dice = new Dice();
        _roller = new RandomDieRoller(seed: 12345);
    }

    [Benchmark(Baseline = true)]
    public IReadOnlyList<DiceResult> RollMany() => _dice.RollMany("4d6+2", Count, _roller);
}
