using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser.Evalutors;

internal class DiceEvaluator(ModifierEvaluator modsEval) : IEvaluatorlet<DiceExpression>
{
    private readonly ModifierEvaluator _modsEval = modsEval;

    public int Eval(Evaluator evaluator, IDieRoller roller, List<TermResult> terms, DiceExpression dice)
    {
        var count = GetDiceCount(evaluator, roller, terms, dice);
        var sides = dice.HasPercentSides ? Constants.PercentValue : GetSides(evaluator, roller, terms, dice);
        var choose = _modsEval.EvalChoose(evaluator, dice.Modifiers, count, roller, terms);
        var exploding = _modsEval.EvalExploding(evaluator, dice.Modifiers, count, roller, terms);

        var diceTerm = new DiceTerm(count, sides, Constants.DefaultScalar, choose, exploding);
        var diceResults = diceTerm.CalculateResults(roller);
        terms.AddRange(diceResults);

        return diceResults.Where(r => r.AppliesToResultCalculation).Sum(r => (int)(r.Value * r.Scalar));
    }

    private static int GetSides(Evaluator eval, IDieRoller roller, List<TermResult> terms, DiceExpression dice) =>
        dice.SidesArg is null
            ? Constants.DefaultDiceSides
            : eval.EvalInternal(dice.SidesArg, roller, terms);

    private static int GetDiceCount(Evaluator eval, IDieRoller roller, List<TermResult> terms, DiceExpression dice) =>
        dice.CountArg is null
            ? Constants.DefaultNumDice
            : eval.EvalInternal(dice.CountArg, roller, terms);
}
