using System.Runtime.CompilerServices;

namespace d20Tek.DiceNotation.Results;

public class TypeException
{
    public static void ThrowIfNot<T>(
        Type targetType,
        string message,
        [CallerArgumentExpression(nameof(targetType))] string? paramName = "none")
    {
        if (targetType != typeof(T))
        {
            throw new ArgumentException(message, paramName);
        }
    }
}
