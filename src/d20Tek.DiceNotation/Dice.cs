using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.Parser;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation;

public class Dice : IDice
{
    private readonly Evaluator _evaluator = new();

    public IDiceConfiguration Configuration { get; }

    public Dice(IDiceConfiguration diceConfig) => Configuration = diceConfig;

    public Dice() => Configuration = new DiceConfiguration();

    public DiceResult Roll(string notation, IDieRoller? dieRoller = null) =>
        _evaluator.Evaluate(notation, dieRoller ?? Configuration.DefaultDieRoller, Configuration);

    public DiceResult Roll(DiceExpression expression, IDieRoller? dieRoller = null) =>
        RollTerms(expression.Evaluate(), dieRoller ?? Configuration.DefaultDieRoller);

    private DiceResult RollTerms(IReadOnlyList<IExpressionTerm> expresssionTerms, IDieRoller dieRoller) =>
        new(
            Constants.JoinSigns(expresssionTerms),
            [.. expresssionTerms.SelectMany(t => t.CalculateResults(dieRoller))],
            dieRoller.GetType().ToString(),
            Configuration);
}
