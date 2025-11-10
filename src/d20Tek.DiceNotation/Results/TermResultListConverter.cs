namespace d20Tek.DiceNotation.Results;

public class TermResultListConverter
{
    // todo: move strings to constants.
    private const int _maxTerms = 100;
    private const string _separator = ", ";
    private const string _diceTerm = "DiceTerm";

    public virtual object Convert(object value, Type targetType, object parameter, string language)
    {
        TypeException.ThrowIfNot<string>(targetType, "Unexpected type passed to converter.");
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        return DiceRollsToString(ConvertList(value));
    }

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
