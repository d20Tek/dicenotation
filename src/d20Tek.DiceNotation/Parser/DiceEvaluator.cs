using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser;

internal static class DiceEvaluator
{
    public static int EvalDice(
        this Evaluator evaluator,
        IDieRoller roller,
        List<TermResult> terms,
        DiceExpression dice)
    {
        var count = dice.CountArg is null ? 1 : evaluator.EvalInternal(dice.CountArg, roller, terms);
        var diceTerm = new DiceTerm(
            count,
            dice.HasPercentSides ? 100 : evaluator.EvalInternal(dice.SidesArg!, roller, terms),
            1,
            evaluator.EvalChoose(dice.Modifiers, count, roller, terms),
            evaluator.EvalExploding(dice.Modifiers, count, roller, terms)
        );
        var diceResults = diceTerm.CalculateResults(roller);
        terms.AddRange(diceResults);
        return diceResults.Where(r => r.AppliesToResultCalculation).Sum(r => (int)(r.Value * r.Scalar));
    }

    public static int EvalFudgeDice(
        this Evaluator evaluator,
        IDieRoller roller,
        List<TermResult> terms,
        FudgeExpression fudge)
    {
        var fudgeCount = fudge.CountArg is null ? 1 : evaluator.EvalInternal(fudge.CountArg, roller, terms);
        var fudgeTerm = new FudgeDiceTerm(
            fudgeCount,
            evaluator.EvalChoose(fudge.Modifiers, fudgeCount, roller, terms)
        );
        var fudgeResults = fudgeTerm.CalculateResults(roller);
        terms.AddRange(fudgeResults);
        return fudgeResults.Where(r => r.AppliesToResultCalculation).Sum(r => (int)(r.Value * r.Scalar));
    }
}
