namespace d20Tek.DiceNotation.DieRoller;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> FilterIfNotEmpty<T>(this IEnumerable<T> list, string? text, Func<T, bool> predicate)
        => string.IsNullOrEmpty(text) ? list : list.Where(predicate);
}
