using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.DiceTerms;

internal class ConstantTerm(int constant) : IExpressionTerm
{
    private readonly int _constant = constant;

    public IReadOnlyList<TermResult> CalculateResults(IDieRoller dieRoller) =>
        [ new(1, _constant, GetType().Name) ];

    public override string ToString() => _constant.ToString();
}
