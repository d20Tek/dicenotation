namespace d20Tek.DiceNotation.Parser;

internal interface IParser
{
    Expression Parse(int rightPrec);

    bool Match(TokenKind k);

    Token Advance();

    void Consume(TokenKind k);

    ParseException Error(string message);
}
