using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser.Evalutors;

internal class UnaryEvaluator : IEvaluatorlet<UnaryExpression>
{
    public int Eval(Evaluator evaluator, IDieRoller roller, List<TermResult> terms, UnaryExpression unary)
    {
        var operand = evaluator.EvalInternal(unary.Operand, roller, terms);
        return unary.Operator == UnaryOperator.Negative ? -operand : operand;
    }
}
