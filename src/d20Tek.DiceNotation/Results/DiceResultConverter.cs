namespace d20Tek.DiceNotation.Results;

public class DiceResultConverter
{
    public virtual object Convert(object value, Type targetType, object parameter, string language)
    {
        TypeException.ThrowIfNot<string>(targetType, Constants.Errors.UnexpectedConverterType);
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        if (value is not DiceResult dr)
        {
            throw new ArgumentException(Constants.Errors.NotDiceResult, nameof(value));
        }

        return Constants.FormatDiceResult(dr);
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
