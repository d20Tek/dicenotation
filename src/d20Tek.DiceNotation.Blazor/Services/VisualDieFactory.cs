namespace d20Tek.DiceNotation.Blazor.Services;

using System.Globalization;
using d20Tek.DiceNotation.Blazor.Models;
using d20Tek.DiceNotation.Results;

/// <summary>
/// Projects a completed <see cref="DiceResult"/> from the notation library onto the
/// visual dice the simulation animates. The mapping only reads the result, so the values
/// the renderer displays are always the values the library produced.
/// </summary>
internal static class VisualDieFactory
{
    private const string DiceTermPrefix = "DiceTerm";
    private const string FudgeTermPrefix = "FudgeDiceTerm";
    private const string ConstantTermType = "ConstantTerm";
    private const int FudgeSides = 3;
    private const int FudgeMinValue = -1;
    private const int DefaultSides = 6;

    /// <summary>
    /// Creates the animated dice and display totals for a completed roll.
    /// </summary>
    /// <param name="result">The result produced by the notation library.</param>
    /// <returns>A summary containing every die in the roll along with its modifier and total.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the result represents a failed roll.</exception>
    public static DiceRollSummary CreateSummary(DiceResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.HasError)
        {
            throw new ArgumentException($"Cannot animate a failed roll: {result.Error}", nameof(result));
        }

        return new DiceRollSummary(
            CreateDice(result),
            CalculateModifier(result),
            result.Value,
            result.DiceExpression);
    }

    /// <summary>
    /// Creates one <see cref="VisualDie"/> for each die in the result, skipping the
    /// constant terms that represent modifiers rather than dice.
    /// </summary>
    /// <param name="result">The result produced by the notation library.</param>
    /// <returns>The dice to animate, in the order the notation produced them.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is null.</exception>
    public static IReadOnlyList<VisualDie> CreateDice(DiceResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var dice = new List<VisualDie>();
        foreach (var term in result.Results ?? [])
        {
            if (term is null || !IsDieTerm(term.Type)) continue;

            var (sides, minValue) = ResolveFaces(term.Type);
            dice.Add(new VisualDie(
                $"die-{dice.Count}",
                FormatDieType(sides, minValue),
                term.Value,
                sides,
                term.Roles,
                minValue));
        }

        return dice;
    }

    /// <summary>
    /// Determines the non-dice portion of a roll by subtracting what the dice contributed
    /// from the library's total. The notation parser folds constants directly into the
    /// total rather than recording them as terms, so the modifier cannot be read from
    /// <see cref="DiceResult.Results"/> and must be derived instead.
    /// </summary>
    /// <param name="result">The result produced by the notation library.</param>
    /// <returns>The combined modifier applied to the roll.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is null.</exception>
    public static int CalculateModifier(DiceResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var diceTotal = 0;
        foreach (var term in result.Results ?? [])
        {
            if (term is null || !IsDieTerm(term.Type) || !term.AppliesToResultCalculation)
            {
                continue;
            }

            diceTotal += (int)Math.Round(term.Value * term.Scalar);
        }

        return result.Value - diceTotal;
    }

    private static bool IsDieTerm(string? type) =>
        type is not null &&
        !string.Equals(type, ConstantTermType, StringComparison.Ordinal) &&
        type.Contains(DiceTermPrefix, StringComparison.Ordinal);

    /// <summary>
    /// Reads the face range out of a term type such as <c>DiceTerm.d20</c>. Fudge dice are
    /// three-sided but run from minus one to plus one, so the range cannot be derived from
    /// the side count alone.
    /// </summary>
    private static (int Sides, int MinValue) ResolveFaces(string type)
    {
        if (type.StartsWith(FudgeTermPrefix, StringComparison.Ordinal)) return (FudgeSides, FudgeMinValue);

        var separator = type.LastIndexOf(".d", StringComparison.Ordinal);
        if (separator >= 0 &&
            int.TryParse(
                type[(separator + 2)..],
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var sides) &&
            sides > 0)
        {
            return (sides, 1);
        }

        // The input is library-produced, so an unrecognized shape is treated as an
        // ordinary die rather than failing the whole animation.
        return (DefaultSides, 1);
    }

    private static string FormatDieType(int sides, int minValue) =>
        minValue == FudgeMinValue && sides == FudgeSides
            ? "dF"
            : $"d{sides.ToString(CultureInfo.InvariantCulture)}";
}
