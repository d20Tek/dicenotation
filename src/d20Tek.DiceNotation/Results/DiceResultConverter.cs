namespace d20Tek.DiceNotation.Results;

public class DiceResultConverter
{
    public virtual object Convert(object value, Type targetType, object parameter, string language)
    {
        TypeException.ThrowIfNot<string>(targetType, "Unexpected type passed to converter.");
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        if (value is not DiceResult dr)
        {
            throw new ArgumentException("Object not of type DiceResult.", nameof(value));
        }

        return $"{dr.Value} ({dr.DiceExpression})";
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
