namespace d20Tek.DiceNotation.Parser;


internal readonly record struct Token(TokenKind Kind, string Lexeme, int? IntValue, Position Pos)
{
    public override string ToString() => $"{Kind} '{Lexeme}' @ {Pos}";

    public void EnsureExpectedKind(TokenKind expectedKind)
    {
        if (Kind != expectedKind)
            throw new ParseException(Constants.Errors.UnexpectedTokenKind(expectedKind, Kind), Pos);
    }
}
