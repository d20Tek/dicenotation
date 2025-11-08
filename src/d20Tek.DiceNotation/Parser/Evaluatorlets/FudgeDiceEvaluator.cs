using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser.Evaluatorlets;

internal class FudgeDiceEvaluator(ModifierEvaluator modsEval) : IEvaluatorlet<FudgeExpression>
{
    private readonly ModifierEvaluator _modsEval = modsEval;

    public int Eval(Evaluator evaluator, IDieRoller roller, List<TermResult> terms, FudgeExpression fudge)
    {
        var fudgeCount = GetDiceCount(evaluator, roller, terms, fudge);
        var choose = _modsEval.EvalChoose(evaluator, fudge.Modifiers, fudgeCount, roller, terms);

        var fudgeTerm = new FudgeDiceTerm(fudgeCount, choose);
        var fudgeResults = fudgeTerm.CalculateResults(roller);
        terms.AddRange(fudgeResults);

        return fudgeResults.Where(r => r.AppliesToResultCalculation).Sum(r => (int)(r.Value * r.Scalar));
    }

    private static int GetDiceCount(Evaluator eval, IDieRoller roller, List<TermResult> terms, FudgeExpression fudge) =>
        fudge.CountArg is null
            ? Constants.DefaultNumDice
            : eval.EvalInternal(fudge.CountArg, roller, terms);
}
