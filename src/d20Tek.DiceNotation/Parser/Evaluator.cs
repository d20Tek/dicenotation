using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser;

public sealed class Evaluator
{
    public DiceResult Evaluate(string notation, IDieRoller roller, IDiceConfiguration config)
    {
        try
        {
            var parser = new Parser(new Lexer(notation));
            var expression = parser.ParseExpression();

            List<TermResult> terms = [];
            var value = EvalInternal(expression, roller, terms);

            return new DiceResult(notation.TrimWhitespace(), value, terms, roller.GetType().Name, config);
        }
        catch (Exception ex)
        {
            return ProcessException(ex, notation);
        }
    }

    internal static DiceResult ProcessException(Exception ex, string notation) => new(
        (ex is ParseException || ex is EvalException) ? ex.Message : $"Unexpected error: {ex.Message}",
        notation);

    internal int EvalInternal(Expression expr, IDieRoller roller, List<TermResult> terms) => expr switch
    {
        NumberExpression num => num.Value,
        UnaryExpression unary => EvalUnary(roller, terms, unary),
        BinaryExpression binary => this.EvalBinary(roller, terms, binary),
        DiceExpression dice => this.EvalDice(roller, terms, dice),
        FudgeExpression fudge => this.EvalFudgeDice(roller, terms, fudge),
        GroupExpression group => EvalInternal(group.Inner, roller, terms),
        _ => throw new EvalException($"Unknown expression type: {expr.GetType().Name}"),
    };

    private int EvalUnary(IDieRoller roller, List<TermResult> terms, UnaryExpression unary)
    {
        var operand = EvalInternal(unary.Operand, roller, terms);
        var result = unary.Operator == UnaryOperator.Negative ? -operand : operand;

        return result;
    }
}
