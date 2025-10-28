//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation;

public class Dice : IDice
{
    private readonly List<IExpressionTerm> _terms = [];
    private readonly DiceParser _parser = new();

    public Dice(IDiceConfiguration diceConfig) => Configuration = diceConfig;

    public Dice() => Configuration = new DiceConfiguration();

    public IDiceConfiguration Configuration { get; }

    public void Clear() => _terms.Clear();

    public IDice Constant(int constant) => AddConstantIf(_terms, constant);

    IDice IDice.Dice(int sides, int numberDice, double scalar, int? choose, int? exploding) =>
        AddDiceTerm(new DiceTerm(numberDice, sides, scalar, choose, exploding));

    public IDice FudgeDice(int numberDice, int? choose) => AddDiceTerm(new FudgeDiceTerm(numberDice, choose));

    public IDice Concat(IDice otherDice)
    {
        ArgumentNullException.ThrowIfNull(otherDice);
        _terms.AddRange(((Dice)otherDice)._terms);
        return this;
    }

    public DiceResult Roll(string expression, IDieRoller? dieRoller = null) =>
        _parser.Parse(expression, Configuration, dieRoller ?? Configuration.DefaultDieRoller);

    public DiceResult Roll(DiceRequest diceRequest, IDieRoller? dieRoller = null)
    {
        List<IExpressionTerm> terms =
        [
            new DiceTerm(
                diceRequest.NumberDice,
                diceRequest.Sides,
                diceRequest.Scalar,
                diceRequest.Choose,
                diceRequest.Exploding)
        ];

        AddConstantIf(terms, diceRequest.Bonus);
        return RollTerms(terms, dieRoller ?? Configuration.DefaultDieRoller);
    }

    public DiceResult Roll(IDieRoller? dieRoller = null) =>
        RollTerms(_terms, dieRoller ?? Configuration.DefaultDieRoller);

    public override string ToString() => string.Join("+", _terms).Replace("+-", "-");

    private DiceResult RollTerms(IList<IExpressionTerm> expresssionTerms, IDieRoller dieRoller) =>
        new(
            string.Join("+", expresssionTerms).Replace("+-", "-"),
            [.. expresssionTerms.SelectMany(t => t.CalculateResults(dieRoller))],
            dieRoller.GetType().ToString(),
            Configuration);

    private Dice AddDiceTerm(IExpressionTerm term)
    {
        _terms.Add(term);
        return this;
    }

    private Dice AddConstantIf(List<IExpressionTerm> terms, int bonus)
    {
        if (bonus != 0) terms.Add(new ConstantTerm(bonus));
        return this;
    }
}
