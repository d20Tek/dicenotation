//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation.Results;

public class DiceResultConverter
{
    public virtual object Convert(object value, Type targetType, object parameter, string language)
    {
        if (targetType != typeof(string))
        {
            throw new ArgumentException("Unexpected type passed to converter.", nameof(targetType));
        }

        ArgumentNullException.ThrowIfNull(nameof(value));

        if (value is not DiceResult dr)
        {
            throw new ArgumentException("Object not of type DiceResult.", nameof(value));
        }

        return string.Format("{0} ({1})", dr.Value, dr.DiceExpression);
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        // the reverse conversion is not supported.
        throw new NotSupportedException();
    }
}
