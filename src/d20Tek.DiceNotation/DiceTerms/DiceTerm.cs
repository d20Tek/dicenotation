using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.DiceTerms;

public partial class DiceTerm : IExpressionTerm
{
    private readonly int _numberDice;
    private readonly int _sides;
    private readonly double _scalar;
    private readonly int? _choose;
    private readonly int? _exploding;

    protected string FormatResultType { get; set; } = DiceTermHelper.DiceFormatResultType;

    protected string FormatDiceTermText { get; set; } = DiceTermHelper.DiceFormatDiceTermText;

    public DiceTerm(int numberDice, int sides, double scalar = 1, int? choose = null, int? exploding = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(numberDice, 0, nameof(numberDice));
        ArgumentOutOfRangeException.ThrowIfLessThan(sides, 1, nameof(sides));
        ArgumentOutOfRangeException.ThrowIfEqual(scalar, 0, nameof(scalar));

        if (choose is not null)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(choose.Value, numberDice, nameof(choose));
            ArgumentOutOfRangeException.ThrowIfLessThan(choose.Value, -numberDice, nameof(choose));
            ArgumentOutOfRangeException.ThrowIfEqual(choose.Value, 0, nameof(choose));
        }

        if (exploding is not null)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(exploding.Value, 0, nameof(exploding));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(exploding.Value, sides, nameof(exploding));
        }

        _numberDice = numberDice;
        _sides = sides;
        _scalar = scalar;
        _choose = choose;
        _exploding = exploding;
    }

    public IReadOnlyList<TermResult> CalculateResults(IDieRoller dieRoller)
    {
        ArgumentNullException.ThrowIfNull(dieRoller);

        var results = new List<TermResult>();
        var termType = string.Format(FormatResultType, GetType().Name, _sides);
        var rerolls = 0;

        // go through the number of dice and roll each one, saving them as term results.
        for (int i = 0; i < _numberDice + rerolls; i++)
        {
            int value = RollTerm(dieRoller, _sides);
            results.Add(new(_scalar, value, termType));

            rerolls = DiceTermHelper.EvaluateExplodingDice(rerolls, value, _exploding);
        }

        return DiceTermHelper.OrderTermResults(results, _choose);
    }

    protected virtual int RollTerm(IDieRoller dieRoller, int sides) => dieRoller.Roll(sides);
}
