using System.Text.RegularExpressions;

namespace d20Tek.DiceNotation.Parser;

internal sealed class Lexer(string source)
{
    private readonly string _src = source;
    private int _index;
    private int _line = 1, _col = 1;

    public Token GetNextToken()
    {
        if (_index >= _src.Length) return Make(TokenKind.EndOfInput, string.Empty, null, currentPos: true);

        var match = Grammar.GetRegex().Match(_src, _index);
        if (!match.Success) throw Error("Unexpected input (no token matched).");

        // Handle whitespace by consuming and tail-recur to the next token.
        if (match.Groups["WS"].Success)
        {
            Advance(match);
            return GetNextToken();
        }

        // Map the first successful token group to a TokenKind.
        (TokenKind kind, string lexeme, int? intValue) = MapMatch(match);
        var tok = Make(kind, lexeme, intValue, currentPos: true);
        Advance(match);
        return tok;
    }

    private (TokenKind kind, string lexeme, int? intValue) MapMatch(Match m)
    {
        if (m.Groups["NUMBER"].Success) return (TokenKind.Number, m.Value, int.Parse(m.Value));
        if (m.Groups["PLUS"].Success) return (TokenKind.Plus, m.Value, null);
        if (m.Groups["MINUS"].Success) return (TokenKind.Minus, m.Value, null);
        if (m.Groups["STAR"].Success) return (TokenKind.Star, m.Value, null);
        if (m.Groups["TIMES"].Success) return (TokenKind.Times, m.Value, null);
        if (m.Groups["DIVIDE"].Success) return (TokenKind.Divide, m.Value, null);
        if (m.Groups["GROUPSTART"].Success) return (TokenKind.GroupStart, m.Value, null);
        if (m.Groups["GROUPEND"].Success) return (TokenKind.GroupEnd, m.Value, null);
        if (m.Groups["DICE"].Success) return (TokenKind.Dice, m.Value, null);
        if (m.Groups["FUDGEDICE"].Success) return (TokenKind.FudgeDice, m.Value, null);
        if (m.Groups["PERCENT"].Success) return (TokenKind.Percent, m.Value, null);
        if (m.Groups["EXPLODING"].Success) return (TokenKind.Exploding, m.Value, null);
        if (m.Groups["KEEP"].Success) return (TokenKind.Keep, m.Value, null);
        if (m.Groups["DROP"].Success) return (TokenKind.Drop, m.Value, null);
        if (m.Groups["KEEPLOWEST"].Success) return (TokenKind.KeepLowest, m.Value, null);

        throw Error($"Unexpected character '{m.Value}'.");
    }

    private void Advance(Match m)
    {
        int consumed = m.Length;
        if (consumed == 0) return;

        // Update line/column tracking using the consumed span.
        var span = _src.AsSpan(_index, consumed);
        int lastNl = -1;
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == '\n')
            {
                _line++;
                _col = 1;
                lastNl = i;
            }
            else
            {
                _col++;
            }
        }

        // Adjust column if last char was '\n' (we moved one too far above)
        if (span.Length > 0 && span[^1] == '\n') _col = 1;

        _index += consumed;
    }

    private Token Make(TokenKind kind, string lexeme, int? intval, bool currentPos)
    {
        var pos = new Position(_index, _line, _col);
        return new Token(kind, lexeme, intval, pos);
    }

    private ParseException Error(string msg) => new(msg, new Position(_index, _line, _col));
}
