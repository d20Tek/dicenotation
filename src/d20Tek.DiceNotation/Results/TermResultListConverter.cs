namespace d20Tek.DiceNotation.Results;

/// <summary>
/// Converts a list of <see cref="TermResult"/> values to a display string of dice rolls.
/// </summary>
public class TermResultListConverter
{
    // todo: move strings to constants.
    private const int _maxTerms = 100;
    private const string _separator = ", ";
    private const string _diceTerm = "DiceTerm";

    /// <summary>
    /// Converts a list of <see cref="TermResult"/> values to a comma-separated display string.
    /// </summary>
    /// <param name="value">The list of term results to convert.</param>
    /// <param name="targetType">The target type, which must be <see cref="string"/>.</param>
    /// <param name="parameter">An optional converter parameter (unused).</param>
    /// <param name="language">The language or locale used for formatting.</param>
    /// <returns>The formatted display string of dice rolls.</returns>
    public virtual object Convert(object value, Type targetType, object parameter, string language)
    {
        TypeException.ThrowIfNot<string>(targetType, "Unexpected type passed to converter.");
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        return DiceRollsToString(ConvertList(value));
    }

    /// <summary>
    /// Not supported. Converting back from a display string is not implemented.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">An optional converter parameter.</param>
    /// <param name="language">The language or locale.</param>
    /// <returns>This method always throws.</returns>
    /// <exception cref="NotSupportedException">Always thrown.</exception>
    public virtual object ConvertBack(object value, Type targetType, object parameter, string language) => 
        throw new NotSupportedException();

    private static string DiceRollsToString(List<TermResult> results) =>
        string.Join(_separator, TrimResults(results));

    private static IEnumerable<string> TrimResults(List<TermResult> results) =>
        results.Take(_maxTerms)
               .Where(r => r.Type.Contains(_diceTerm))
               .Select(r => r.AppliesToResultCalculation ? $"{r.Value}" : $"{r.Value}*");

    private static List<TermResult> ConvertList(object value)
    {
        if (value is not List<TermResult> list)
        {
            list = (value is not IReadOnlyList<TermResult> readonlyList) 
                ? throw new ArgumentException("Object not of type List<TermResult>.", nameof(value))
                : [.. readonlyList];
        }

        return list;
    }
}
