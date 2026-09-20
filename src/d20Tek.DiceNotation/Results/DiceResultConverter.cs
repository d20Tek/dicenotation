namespace d20Tek.DiceNotation.Results;

/// <summary>
/// Converts a <see cref="DiceResult"/> to its display string representation.
/// </summary>
public class DiceResultConverter
{
    /// <summary>
    /// Converts a <see cref="DiceResult"/> to a formatted display string.
    /// </summary>
    /// <param name="value">The <see cref="DiceResult"/> to convert.</param>
    /// <param name="targetType">The target type, which must be <see cref="string"/>.</param>
    /// <param name="parameter">An optional converter parameter (unused).</param>
    /// <param name="language">The language or locale used for formatting.</param>
    /// <returns>The formatted display string.</returns>
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
}
