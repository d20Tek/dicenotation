using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation;

public class Dice : IDice
{
    private readonly DiceParser _parser = new();

    public IDiceConfiguration Configuration { get; }

    public Dice(IDiceConfiguration diceConfig) => Configuration = diceConfig;

    public Dice() => Configuration = new DiceConfiguration();

    public DiceResult Roll(string notation, IDieRoller? dieRoller = null) =>
        _parser.Parse(notation, Configuration, dieRoller ?? Configuration.DefaultDieRoller);

    public DiceResult Roll(DiceRequest diceRequest, IDieRoller? dieRoller = null)
    {
        List<IExpressionTerm> terms = [ diceRequest.ToDiceTerm() ];
        if (diceRequest.Bonus != 0) terms.Add(new ConstantTerm(diceRequest.Bonus));

        return RollTerms(terms, dieRoller ?? Configuration.DefaultDieRoller);
    }

    public DiceResult Roll(DiceExpression expression, IDieRoller? dieRoller = null) =>
        RollTerms(expression.Roll(), dieRoller ?? Configuration.DefaultDieRoller);

    private DiceResult RollTerms(IReadOnlyList<IExpressionTerm> expresssionTerms, IDieRoller dieRoller) =>
        new(
            string.Join("+", expresssionTerms).Replace("+-", "-"),
            [.. expresssionTerms.SelectMany(t => t.CalculateResults(dieRoller))],
            dieRoller.GetType().ToString(),
            Configuration);
}
