using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser.Evaluatorlets;

internal class GroupEvaluator : IEvaluatorlet<GroupExpression>
{
    public int Eval(Evaluator evaluator, IDieRoller roller, List<TermResult> terms, GroupExpression group) =>
        evaluator.EvalInternal(group.Inner, roller, terms);
}
