using System.Text.RegularExpressions;

namespace d20Tek.DiceNotation.Parser;

internal static partial class Grammar
{
    // Only the WS group is named; all other tokens are classified from the matched text itself
    // in Lexer.MapMatch to avoid allocating a Group/GroupCollection entry per named capture.
    [GeneratedRegex("""
        (?ix)                                               # i=ignore case, x=ignore whitespace/comments
        \G                                                  # anchor to current position (_index)
        (?:
            (?<WS>  [\u0009\u0020\u000D\u000A]+ )           # \t space \r \n
          | (?:0|[1-9][0-9]*)                               # NUMBER
          | [+\-*x/()dfk%!pl]                                # single-char tokens
          | .                                                # any other single char (error)
        )
        """, RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    public static partial Regex GetRegex();
}
