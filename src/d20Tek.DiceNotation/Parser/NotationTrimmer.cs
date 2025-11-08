using System.Text.RegularExpressions;

namespace d20Tek.DiceNotation.Parser;

internal static partial class NotationTrimmer
{
    public static string TrimWhitespace(this string notation) => WhitespaceRegex().Replace(notation, string.Empty);

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
