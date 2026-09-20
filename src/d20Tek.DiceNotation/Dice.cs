using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.Parser;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation;

/// <summary>
/// Provides the default implementation for parsing and rolling dice expressions.
/// </summary>
public class Dice : IDice
{
    private readonly Evaluator _evaluator = new();

    /// <inheritdoc />
    public IDiceConfiguration Configuration { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dice"/> class with the specified configuration.
    /// </summary>
    /// <param name="diceConfig">The configuration that controls default dice behavior.</param>
    public Dice(IDiceConfiguration diceConfig) => Configuration = diceConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dice"/> class with the default configuration.
    /// </summary>
    public Dice() => Configuration = new DiceConfiguration();

    /// <inheritdoc />
    public DiceResult Roll(string notation, IDieRoller? dieRoller = null) =>
        _evaluator.Evaluate(notation, dieRoller ?? Configuration.DefaultDieRoller, Configuration);

    /// <inheritdoc />
    public DiceResult Roll(DiceExpression expression, IDieRoller? dieRoller = null) =>
        RollTerms(expression.Evaluate(), dieRoller ?? Configuration.DefaultDieRoller);

    private DiceResult RollTerms(IReadOnlyList<IExpressionTerm> expresssionTerms, IDieRoller dieRoller) =>
        new(
            Constants.JoinSigns(expresssionTerms),
            [.. expresssionTerms.SelectMany(t => t.CalculateResults(dieRoller))],
            dieRoller.GetType().ToString(),
            Configuration);
}
