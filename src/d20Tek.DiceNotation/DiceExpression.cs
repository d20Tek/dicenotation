using d20Tek.DiceNotation.DiceTerms;

namespace d20Tek.DiceNotation;

public class DiceExpression
{
    private readonly List<IExpressionTerm> _terms = [];

    public static DiceExpression Create() => new();

    public DiceExpression AddConstant(int constant)
    {
        if (constant != 0) _terms.Add(new ConstantTerm(constant));
        return this;
    }

    public DiceExpression AddDice(
        int sides,
        int numberDice = 1,
        double scalar = 1,
        int? choose = null,
        int? exploding = null) =>
        AddDiceTerm(new DiceTerm(numberDice, sides, scalar, choose, exploding));

    public DiceExpression AddFudgeDice(int numberDice = 1, int? choose = null) =>
        AddDiceTerm(new FudgeDiceTerm(numberDice, choose));

    public DiceExpression Clear()
    {
        _terms.Clear();
        return this;
    }

    public DiceExpression Concat(DiceExpression otherDice)
    {
        _terms.AddRange(otherDice._terms);
        return this;
    }

    // todo: rename function to Evaluate
    public IReadOnlyList<IExpressionTerm> Roll() => _terms;

    public override string ToString() => string.Join("+", _terms).Replace("+-", "-");

    private DiceExpression AddDiceTerm(IExpressionTerm term)
    {
        _terms.Add(term);
        return this;
    }
}
