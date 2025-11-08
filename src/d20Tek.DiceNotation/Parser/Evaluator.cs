using d20Tek.DiceNotation.DiceTerms;
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
            return new DiceResult(
                (ex is ParseException || ex is EvalException) ? ex.Message : $"Unexpected error: {ex.Message}",
                notation);
        }
    }

    private int EvalInternal(Expression expr, IDieRoller roller, List<TermResult> terms)
    {
        switch (expr)
        {
            case NumberExpression num:
                return num.Value;

            case UnaryExpression unary:
                var operand = EvalInternal(unary.Operand, roller, terms);
                var result = unary.Operator == UnaryOperator.Negative ? -operand : operand;

                return result;

            case BinaryExpression binary:
                var left = EvalInternal(binary.Left, roller, terms);
                var right = EvalInternal(binary.Right, roller, terms);
                var binResult = binary.Operator switch
                {
                    BinaryOperator.Add => left + right,
                    BinaryOperator.Subtract => left - right,
                    BinaryOperator.Multiply => left * right,
                    BinaryOperator.Divide => right == 0
                        ? throw new EvalException("Division by zero.")
                        : left / right,
                    _ => throw new EvalException("Unknown binary operator.")
                };

                return binResult;

            case DiceExpression dice:
                var count = dice.CountArg is null ? 1 : EvalInternal(dice.CountArg, roller, terms);
                var diceTerm = new DiceTerm(
                    count,
                    dice.HasPercentSides ? 100 : EvalInternal(dice.SidesArg!, roller, terms),
                    1,
                    EvalChoose(dice.Modifiers, count, roller, terms),
                    EvalExploding(dice.Modifiers, count, roller, terms)
                );
                var diceResults = diceTerm.CalculateResults(roller);
                terms.AddRange(diceResults);
                return diceResults.Where(r => r.AppliesToResultCalculation).Sum(r => (int)(r.Value * r.Scalar));

            case FudgeExpression fudge:
                var fudgeCount = fudge.CountArg is null ? 1 : EvalInternal(fudge.CountArg, roller, terms);
                var fudgeTerm = new FudgeDiceTerm(
                    fudgeCount,
                    EvalChoose(fudge.Modifiers, fudgeCount, roller, terms)
                );
                var fudgeResults = fudgeTerm.CalculateResults(roller);
                terms.AddRange(fudgeResults);
                return fudgeResults.Where(r => r.AppliesToResultCalculation).Sum(r => (int)(r.Value * r.Scalar));

            case GroupExpression group:
                return EvalInternal(group.Inner, roller, terms);

            default:
                throw new EvalException($"Unknown expression type: {expr.GetType().Name}");
        }
    }

    private int? EvalChoose(IReadOnlyList<Modifier> mods, int diceCount, IDieRoller roller, List<TermResult> terms)
    {
        var selectMod = mods.OfType<SelectModifier>().LastOrDefault();
        return selectMod switch
        {
            { Kind: SelectKind.KeepHigh } => EvalInternal(selectMod.CountArg, roller, terms),
            { Kind: SelectKind.DropLow } => diceCount - EvalInternal(selectMod.CountArg, roller, terms),
            { Kind: SelectKind.KeepLow } => -EvalInternal(selectMod.CountArg, roller, terms),
            _ => null
        };
    }

    private int? EvalExploding(IReadOnlyList<Modifier> mods, int diceCount, IDieRoller roller, List<TermResult> terms)
    {
        var explodeMod = mods.OfType<ExplodingModifier>().LastOrDefault();
        if (explodeMod is null) return null;
        return explodeMod.ThresholdArg is null ? diceCount : EvalInternal(explodeMod.ThresholdArg, roller, terms);
    }
}
