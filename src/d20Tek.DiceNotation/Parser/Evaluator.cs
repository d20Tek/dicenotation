using d20Tek.DiceNotation.Parser.Evaluatorlets;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser;

/// <summary>
/// Parses dice notation and evaluates the resulting expression tree into a dice result.
/// </summary>
public sealed class Evaluator
{
    private static readonly ModifierEvaluator _modsEval = new();
    private static readonly Dictionary<Type, object> _evaluatorlets = new()
    {
        { typeof(NumberExpression), new NumberEvaluator() },
        { typeof(UnaryExpression), new UnaryEvaluator() },
        { typeof(BinaryExpression), new BinaryEvaluator() },
        { typeof(GroupExpression), new GroupEvaluator() },
        { typeof(DiceExpression), new DiceEvaluator(_modsEval) },
        { typeof(FudgeExpression), new FudgeDiceEvaluator(_modsEval) },
    };

    /// <summary>
    /// Parses and evaluates the specified dice notation into a <see cref="DiceResult"/>.
    /// </summary>
    /// <param name="notation">The dice notation to parse and evaluate.</param>
    /// <param name="roller">The die roller used to generate random values.</param>
    /// <param name="config">The configuration that controls default dice behavior.</param>
    /// <returns>A <see cref="DiceResult"/> containing the outcome of the evaluation.</returns>
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
        (ex is ParseException || ex is EvalException) ? ex.Message : Constants.Errors.UnknownException(ex.Message),
        notation);

    internal int EvalInternal(Expression expr, IDieRoller roller, List<TermResult> terms) =>
        _evaluatorlets.TryGetValue(expr.GetType(), out var eval)
            ? ((dynamic)eval).Eval(this, roller, terms, (dynamic)expr)
            : throw new EvalException(Constants.Errors.UnknownExpression(expr));
}
