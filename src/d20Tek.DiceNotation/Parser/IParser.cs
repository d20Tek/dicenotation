namespace d20Tek.DiceNotation.Parser;

internal interface IParser
{
    Expression Parse(int rightPrecendence);

    bool Match(TokenKind kind);

    Token Advance();

    void Consume(TokenKind kind);

    ParseException Error(string message);
}
