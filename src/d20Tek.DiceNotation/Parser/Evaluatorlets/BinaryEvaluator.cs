using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser.Evalutors;

internal class BinaryEvaluator : IEvaluatorlet<BinaryExpression>
{
    public int Eval(Evaluator evaluator, IDieRoller roller, List<TermResult> terms, BinaryExpression binary)
    {
        var left = evaluator.EvalInternal(binary.Left, roller, terms);
        var right = evaluator.EvalInternal(binary.Right, roller, terms);

        return binary.Operator switch
        {
            BinaryOperator.Add => left + right,
            BinaryOperator.Subtract => left - right,
            BinaryOperator.Multiply => left * right,
            BinaryOperator.Divide => SafeDivide(left, right),
            _ => throw new EvalException(Constants.Errors.UnknownBinaryOperator(binary.Operator))
        };
    }

    private static int SafeDivide(int left, int right) =>
        right == 0 ? throw new EvalException(Constants.Errors.DivideByZero) : left / right;
}
