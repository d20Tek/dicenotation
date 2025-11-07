namespace d20Tek.DiceNotation.Parser.Parselets;

internal sealed class SelectKindMapper
{
    public static SelectKind FromTokenKind(Token token) =>
        token.Kind switch
        {
            TokenKind.Keep => SelectKind.KeepHigh,
            TokenKind.Drop => SelectKind.DropLow,
            TokenKind.KeepLowest => SelectKind.KeepLow,
            _ => throw new ParseException(Constants.Errors.UnexpectedSelectKind(token.Kind), token.Pos)
        };
}
