namespace d20Tek.DiceNotation.Parser;


internal readonly record struct Token(TokenKind Kind, string Lexeme, int? IntValue, Position Pos)
{
    public override string ToString() => $"{Kind} '{Lexeme}' @ {Pos}";
}
