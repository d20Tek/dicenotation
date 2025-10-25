//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation.Results;

public class TermResultListConverter
{
    private const int _maxTerms = 100;

    public virtual object Convert(object value, Type targetType, object parameter, string language)
    {
        if (targetType != typeof(string))
        {
            throw new ArgumentException("Unexpected type passed to converter.", nameof(targetType));
        }

        ArgumentNullException.ThrowIfNull(value, nameof(value));

        return DiceRollsToString(ConvertList(value));
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, string language) => 
        throw new NotSupportedException();

    private static string DiceRollsToString(List<TermResult> results) =>
        string.Join(
            ", ",
            results.Take(_maxTerms)
                   .Where(r => r.Type.Contains("DiceTerm"))
                   .Select(r => r.AppliesToResultCalculation ? $"{r.Value}" : $"{r.Value}*"));

    private static List<TermResult> ConvertList(object value)
    {
        if (value is not List<TermResult> list)
        {
            list = (value is not IReadOnlyList<TermResult> readonlyList) ?
                        throw new ArgumentException("Object not of type List<TermResult>.", nameof(value)) :
                        [.. readonlyList];
        }

        return list;
    }
}
