using System.Text.RegularExpressions;

namespace d20Tek.DiceNotation.Parser;

internal static partial class Grammar
{
    [GeneratedRegex("""
        (?ix)                                               # i=ignore case, x=ignore whitespace/comments
        \G                                                  # anchor to current position (_index)
        (?:
            (?<WS>          [\u0009\u0020\u000D\u000A]+ )   # \t space \r \n
          | (?<NUMBER>      (?:0|[1-9][0-9]*) )
          | (?<PLUS>        \+ )
          | (?<MINUS>       - )
          | (?<STAR>        \* )
          | (?<TIMES>       x )
          | (?<DIVIDE>      / )
          | (?<GROUPSTART>  \() 
          | (?<GROUPEND>    \) )
          | (?<DICE>        d )
          | (?<FUDGEDICE>   f )
          | (?<PERCENT>     % )
          | (?<EXPLODING>   ! )
          | (?<KEEP>        k )
          | (?<DROP>        p )
          | (?<KEEPLOWEST>  l )
          | (?<INVALID>     . )                             # any other single char (error)
        )
        """, RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    public static partial Regex GetRegex();

    public static readonly IReadOnlyDictionary<string, Func<Match, (TokenKind, string, int?)>> MatchFactories =
        new Dictionary<string, Func<Match, (TokenKind, string, int?)>>
        {
            ["NUMBER"]      = m => (TokenKind.Number, m.Value, int.Parse(m.Value)),
            ["PLUS"]        = m => (TokenKind.Plus, m.Value, null),
            ["MINUS"]       = m => (TokenKind.Minus, m.Value, null),
            ["STAR"]        = m => (TokenKind.Star, m.Value, null),
            ["TIMES"]       = m => (TokenKind.Times, m.Value, null),
            ["DIVIDE"]      = m => (TokenKind.Divide, m.Value, null),
            ["GROUPSTART"]  = m => (TokenKind.GroupStart, m.Value, null),
            ["GROUPEND"]    = m => (TokenKind.GroupEnd, m.Value, null),
            ["DICE"]        = m => (TokenKind.Dice, m.Value, null),
            ["FUDGEDICE"]   = m => (TokenKind.FudgeDice, m.Value, null),
            ["PERCENT"]     = m => (TokenKind.Percent, m.Value, null),
            ["EXPLODING"]   = m => (TokenKind.Exploding, m.Value, null),
            ["KEEP"]        = m => (TokenKind.Keep, m.Value, null),
            ["DROP"]        = m => (TokenKind.Drop, m.Value, null),
            ["KEEPLOWEST"]  = m => (TokenKind.KeepLowest, m.Value, null)
        };
}
