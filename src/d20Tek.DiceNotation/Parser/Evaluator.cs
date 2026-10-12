using d20Tek.DiceNotation.Parser.Evaluatorlets;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser;

/// <summary>
/// Parses dice notation and evaluates the resulting expression tree into a dice result.
/// </summary>
public sealed class Evaluator
{
    private readonly ModifierEvaluator _modsEval = new();
    private readonly NumberEvaluator _numberEval = new();
    private readonly UnaryEvaluator _unaryEval = new();
    private readonly BinaryEvaluator _binaryEval = new();
    private readonly GroupEvaluator _groupEval;
    private readonly DiceEvaluator _diceEval;
    private readonly FudgeDiceEvaluator _fudgeEval;

    /// <summary>
    /// Initializes a new instance of the <see cref="Evaluator"/> class.
    /// </summary>
    public Evaluator()
    {
        _groupEval = new GroupEvaluator();
        _diceEval = new DiceEvaluator(_modsEval);
        _fudgeEval = new FudgeDiceEvaluator(_modsEval);
    }

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

    internal int EvalInternal(Expression expr, IDieRoller roller, List<TermResult> terms) => expr switch
    {
        NumberExpression num => _numberEval.Eval(this, roller, terms, num),
        UnaryExpression unary => _unaryEval.Eval(this, roller, terms, unary),
        BinaryExpression binary => _binaryEval.Eval(this, roller, terms, binary),
        GroupExpression group => _groupEval.Eval(this, roller, terms, group),
        DiceExpression dice => _diceEval.Eval(this, roller, terms, dice),
        FudgeExpression fudge => _fudgeEval.Eval(this, roller, terms, fudge),
        _ => throw new EvalException(Constants.Errors.UnknownExpression(expr)),
    };
}
