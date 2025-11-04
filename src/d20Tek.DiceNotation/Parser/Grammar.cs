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
}
