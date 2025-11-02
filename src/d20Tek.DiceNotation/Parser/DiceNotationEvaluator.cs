using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser;

internal class DiceNotationEvaluator
{
    private readonly Lexer _lexer = new();
    private readonly DiceParser _parser = new();

    public DiceResult Roll(string notation, IDieRoller dieRoller, IDiceConfiguration config)
    {
        var tokens = _lexer.Tokenize(notation);
        var expression = _parser.ParseExpression(tokens);

        var (value, results) = EvaluateInternal(expression, dieRoller);

        return new(notation, value, results, dieRoller.GetType().Name, config);
    }

    private static (int Value, List<TermResult> Results) EvaluateInternal(Expression expr, IDieRoller dieRoller) =>
        expr switch
        {
            NumberExpression n => EvaluateNumber(n, dieRoller),
            DiceExpression d => EvaluateDice(d, dieRoller),
            GroupExpression g => EvaluateInternal(g.Inner, dieRoller),
            BinaryExpression b => EvaluateBinary(b, dieRoller),
            _ => throw new InvalidOperationException($"Unsupported expression type: {expr.GetType().Name}")
        };

    private static (int, List<TermResult>) EvaluateNumber(NumberExpression n, IDieRoller dieRoller)
    {
        var results = new List<TermResult> { new(1, n.Value, "Number") }; // new ConstantTerm(n.Value).CalculateResults(dieRoller).ToList();
        return (n.Value, results);
    }

    private static (int, List<TermResult>) EvaluateDice(DiceExpression dice, IDieRoller dieRoller)
    {
        var term = new DiceTerm(dice.Count, dice.Sides, 1, dice.Keep, dice.Explode);
        var results = term.CalculateResults(dieRoller).ToList();

        var value = results.Where(r => r.AppliesToResultCalculation)
                           .Sum(r => (int)Math.Round(r.Value * r.Scalar));

        return (value, results);
    }

    private static (int, List<TermResult>) EvaluateBinary(BinaryExpression expr, IDieRoller dieRoller)
    {
        var (leftValue, leftResults) = EvaluateInternal(expr.Left, dieRoller);
        var (rightValue, rightResults) = EvaluateInternal(expr.Right, dieRoller);

        var resultValue = ApplyOperator(expr.Operator, leftValue, rightValue);
        var combined = leftResults.Concat(rightResults).ToList();

        return (resultValue, combined);
    }

    private static int ApplyOperator(string op, int a, int b) =>
        op switch
        {
            "+" => a + b,
            "-" => a - b,
            "*" or "x" => a * b,
            "/" => b == 0 ? 0 : a / b,
            _ => throw new InvalidOperationException($"Unsupported operator: {op}")
        };
}