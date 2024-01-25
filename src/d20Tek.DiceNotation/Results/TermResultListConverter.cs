//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation.Results;

public class TermResultListConverter
{
    public virtual object Convert(object value, Type targetType, object parameter, string language)
    {
        if (targetType != typeof(string))
        {
            throw new ArgumentException("Unexpected type passed to converter.", nameof(targetType));
        }

        ArgumentNullException.ThrowIfNull(value, nameof(value));

        if (value is not List<TermResult> list)
        {
            throw new ArgumentException("Object not of type List<TermResult>.", nameof(value));
        }

        return DiceRollsToString(list);
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotSupportedException();
    }

    private string DiceRollsToString(List<TermResult> results)
    {
        var list = from r in results.Take(100)
                   where r.Type.Contains("DiceTerm")
                   select r;

        string res = string.Empty;
        foreach (TermResult item in list)
        {
            if (item.AppliesToResultCalculation)
            {
                res += item.Value.ToString() + ", ";
            }
            else
            {
                res += item.Value.ToString() + "*, ";
            }
        }

        return res.Trim().TrimEnd(',');
    }
}
