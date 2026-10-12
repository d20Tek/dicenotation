using BenchmarkDotNet.Attributes;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Benchmarks;

/// <summary>
/// Benchmarks rolling a pre-built <see cref="DiceExpression"/>, isolating the evaluator/roll
/// pipeline from the lexer and parser.
/// </summary>
[MemoryDiagnoser]
public class ExpressionRollBenchmarks
{
    private IDice _dice = null!;
    private IDieRoller _roller = null!;
    private DiceExpression _expression = null!;

    [GlobalSetup]
    public void Setup()
    {
        _dice = new Dice();
        _roller = new RandomDieRoller(seed: 12345);
        _expression = DiceExpression.Create().AddDice(6, 4, choose: 3).AddDice(8).AddConstant(5);
    }

    [Benchmark(Baseline = true)]
    public DiceResult RollExpression() => _dice.Roll(_expression, _roller);
}
