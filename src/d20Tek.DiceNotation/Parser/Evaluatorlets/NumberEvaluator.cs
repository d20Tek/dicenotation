using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser.Evalutors;

internal class NumberEvaluator : IEvaluatorlet<NumberExpression>
{
    public int Eval(Evaluator evaluator, IDieRoller roller, List<TermResult> terms, NumberExpression num) =>
        num.Value;
}
