using System.Runtime.CompilerServices;

namespace d20Tek.DiceNotation.Results;

internal class TypeException
{
    public static void ThrowIfNot<T>(
        Type targetType,
        string message,
        [CallerArgumentExpression(nameof(targetType))] string? paramName = Constants.DefaultParameterName)
    {
        if (targetType != typeof(T)) throw new ArgumentException(message, paramName);
    }

    public static void ThrowIfNotAssignableFrom<T>(
        Type targetType,
        [CallerArgumentExpression(nameof(targetType))] string? paramName = Constants.DefaultParameterName)
    {
        var typeT = typeof(T);
        if (typeT.IsAssignableFrom(targetType) is false)
        {
            throw new ArgumentException(Constants.Errors.NotAssignableType(typeT, targetType));
        }
    }
}
