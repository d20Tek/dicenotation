using System.Text.RegularExpressions;

namespace d20Tek.DiceNotation.Parser;

internal sealed class Lexer(string source)
{
    private readonly string _src = source;
    private int _index = 0, _line = 1, _col = 1;

    public Token GetNextToken()
    {
        if (_index >= _src.Length)
            return MakeToken(TokenKind.EndOfInput, string.Empty, null);

        var match = Grammar.GetRegex().Match(_src, _index);
        ParseException.ThrowIfFalse(match.Success, "Unexpected input (no token matched).", new(_index, _line, _col));
        
        return (match.Groups["WS"].Success is true) ? ProcessWhitespace(match) : ProcessTokenGroup(match);
    }

    private Token ProcessWhitespace(Match match)
    {
        Advance(match);
        return GetNextToken();
    }

    private Token ProcessTokenGroup(Match match)
    {
        (TokenKind kind, string lexeme, int? intValue) = MapMatch(match);
        var token = MakeToken(kind, lexeme, intValue);
        Advance(match);

        return token;
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

        throw new ParseException($"Unexpected character '{m.Value}'.", new(_index, _line, _col));
    }

    private void Advance(Match m)
    {
        int consumed = m.Length;
        foreach (char c in _src.AsSpan(_index, consumed))
        {
            (_line, _col) = AdvanceNewLine(c, _line, _col);
        }

        _index += consumed;
    }

    private static (int, int) AdvanceNewLine(char ch, int line, int col) => 
        (ch == Constants.NewLine) ? (line + 1, 1) : (line, col + 1);

    private Token MakeToken(TokenKind kind, string lexeme, int? intval) => 
        new(kind, lexeme, intval, new(_index, _line, _col));
}
