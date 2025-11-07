using System.Text.RegularExpressions;

namespace d20Tek.DiceNotation.Parser;

internal sealed class Lexer(string source)
{
    private readonly string _src = source;
    private int _index = 0, _line = 1, _col = 1;

    private Position CurrentPos => new(_index, _line, _col);

    public Token GetNextToken()
    {
        if (_index >= _src.Length)
            return MakeToken(TokenKind.EndOfInput, string.Empty, null);

        var match = Grammar.GetRegex().Match(_src, _index);
        ParseException.ThrowIfFalse(match.Success, Constants.Errors.UnmatchedToken, CurrentPos);
        
        return (match.Groups[Constants.WhiteSpaceGroup].Success is true)
            ? ProcessWhitespace(match) 
            : ProcessTokenGroup(match);
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
        var (key, factoryFunc) = Grammar.MatchFactories.FirstOrDefault(e => m.Groups[e.Key].Success);
        return key is null
            ? throw new ParseException(Constants.Errors.UnexpectedCharacter(m.Value), CurrentPos)
            : factoryFunc(m);        
    }

    private void Advance(Match m)
    {
        foreach (char c in _src.AsSpan(_index, m.Length))
        {
            (_line, _col) = AdvanceNewLine(c, _line, _col);
        }

        _index += m.Length;
    }

    private static (int, int) AdvanceNewLine(char ch, int line, int col) => 
        (ch == Constants.NewLine) ? (line + 1, 1) : (line, col + 1);

    private Token MakeToken(TokenKind kind, string lexeme, int? intval) => new(kind, lexeme, intval, CurrentPos);
}
