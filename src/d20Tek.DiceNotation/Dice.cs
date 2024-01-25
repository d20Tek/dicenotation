//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation;

public class Dice : IDice
{
    private readonly IList<IExpressionTerm> terms = new List<IExpressionTerm>();
    private readonly DiceParser parser = new();

    public Dice()
    {
    }

    public IDiceConfiguration Configuration { get; } = new DiceConfiguration();

    public void Clear()
    {
        terms.Clear();
    }

    public IDice Constant(int constant)
    {
        // do not add a constant term if it's 0.
        if (constant != 0)
        {
            terms.Add(new ConstantTerm(constant));
        }

        return this;
    }

    IDice IDice.Dice(int sides, int numberDice, double scalar, int? choose, int? exploding)
    {
        terms.Add(new DiceTerm(numberDice, sides, scalar, choose, exploding));
        return this;
    }

    public IDice FudgeDice(int numberDice, int? choose)
    {
        terms.Add(new FudgeDiceTerm(numberDice, choose));
        return this;
    }

    public IDice Concat(IDice otherDice)
    {
        Dice other = (Dice)otherDice;
        if (other == null)
        {
            throw new ArgumentNullException(nameof(otherDice));
        }

        foreach (IExpressionTerm term in ((Dice)otherDice).terms)
        {
            terms.Add(term);
        }

        return this;
    }

    public DiceResult Roll(string expression, IDieRoller? dieRoller = null)
    {
        dieRoller ??= Configuration.DefaultDieRoller;
        return parser.Parse(
            expression,
            Configuration,
            dieRoller ?? Configuration.DefaultDieRoller);
    }

    public DiceResult Roll(DiceRequest diceRequest, IDieRoller? dieRoller = null)
    {
        dieRoller ??= Configuration.DefaultDieRoller;
        var term = new DiceTerm(
                diceRequest.NumberDice,
                diceRequest.Sides,
                diceRequest.Scalar,
                diceRequest.Choose,
                diceRequest.Exploding);

        var termResults = term.CalculateResults(dieRoller).ToList();

        return new DiceResult(
            term.ToString(),
            termResults,
            dieRoller.GetType().ToString(),
            Configuration);
    }

    public DiceResult Roll(IDieRoller? dieRoller = null)
    {
        dieRoller ??= Configuration.DefaultDieRoller;
        List<TermResult> termResults = terms.SelectMany(t => t.CalculateResults(dieRoller))
                                            .ToList();
        return new DiceResult(
            ToString(),
            termResults,
            dieRoller.GetType().ToString(),
            Configuration);
    }

    public override string ToString()
    {
        return string.Join("+", terms).Replace("+-", "-");
    }
}
