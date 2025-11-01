using System.Runtime.CompilerServices;

namespace d20Tek.DiceNotation.Results;

public class TypeException
{
    public static void ThrowIfNot<T>(
        Type targetType,
        string message,
        [CallerArgumentExpression(nameof(targetType))] string? paramName = "none")
    {
        if (targetType != typeof(T)) throw new ArgumentException(message, paramName);
    }

    public static void ThrowIfNotAssignableFrom<T>(
        Type targetType,
        [CallerArgumentExpression(nameof(targetType))] string? paramName = "none")
    {
        var typeT = typeof(T);
        if (typeT.IsAssignableFrom(targetType) is false)
        {
            throw new ArgumentException($"{targetType} is not of type {typeT}.", paramName);
        }
    }
}
