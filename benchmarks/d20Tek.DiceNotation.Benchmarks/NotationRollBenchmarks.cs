using BenchmarkDotNet.Attributes;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Benchmarks;

/// <summary>
/// Benchmarks the end-to-end cost of parsing and rolling dice notation strings, covering the
/// lexer, parser, and evaluator pipeline for a range of expression complexities.
/// </summary>
[MemoryDiagnoser]
public class NotationRollBenchmarks
{
    private IDice _dice = null!;
    private IDieRoller _roller = null!;

    //[Params("1d20", "4d6+2", "4d6k3+1d8+5", "(2+1)d4+2d6!6-3")]
    [Params("(2+1)d4+2d6!6-3")]
    public string Notation { get; set; } = "1d20";

    [GlobalSetup]
    public void Setup()
    {
        _dice = new Dice();
        _roller = new RandomDieRoller(seed: 12345);
    }

    [Benchmark(Baseline = true)]
    public DiceResult RollNotation() => _dice.Roll(Notation, _roller);
}
