namespace d20Tek.DiceNotation.Parser;

internal sealed class DiceParser
{
    private IEnumerator<Token> _tokens = Enumerable.Empty<Token>().GetEnumerator();
    private Token _current = new(TokenType.StartOfInput, string.Empty);

    public Expression ParseExpression(IEnumerable<Token> tokens)
    {
        _tokens = tokens.GetEnumerator();
        Advance();

        return ParseExpressionInternal();
    }

    private Expression ParseExpressionInternal() => ParseAddition();

    private void Advance() => _current = _tokens.MoveNext() ? _tokens.Current : new(TokenType.EndOfInput, "");

    // precedence levels
    private Expression ParseAddition()
    {
        var expr = ParseMultiplication();
        while (_current is { Type: TokenType.Operator, Value: "+" or "-" })
        {
            string op = _current.Value;
            Advance();
            var right = ParseMultiplication();
            expr = new BinaryExpression(expr, op, right);
        }
        return expr;
    }

    private Expression ParseMultiplication()
    {
        var expr = ParseDice();
        while (_current is { Type: TokenType.Operator, Value: "*" or "/" or "x" })
        {
            string op = _current.Value;
            Advance();
            var right = ParseDice();
            expr = new BinaryExpression(expr, op, right);
        }
        return expr;
    }

    private Expression ParseDice()
    {
        var left = ParsePrimary();

        if (_current is { Type: TokenType.Operator, Value: "d" or "f" })
        {
            string dieType = _current.Value;
            Advance();

            int sides = 6;
            if (_current.Type == TokenType.Number)
            {
                sides = int.Parse(_current.Value);
                Advance();
            }

            int? keep = null, explode = null;
            while (_current.Value is "k" or "p" or "l" or "!")
            {
                string mod = _current.Value;
                Advance();
                if (mod == "!") explode = sides;
                else if (_current.Type == TokenType.Number)
                {
                    int n = int.Parse(_current.Value);
                    Advance();
                    if (mod == "k") keep = n;
                    else if (mod == "p") keep = (int?)null; // adjust for drop rules
                    else if (mod == "l") keep = -n;
                }
            }

            if (left is not NumberExpression nExpr)
                throw new FormatException("Expected number before die operator");

            return new DiceExpression(nExpr.Value, sides, keep, explode);
        }

        return left;
    }

    private Expression ParsePrimary()
    {
        if (_current.Type == TokenType.Number)
        {
            int value = int.Parse(_current.Value);
            Advance();
            return new NumberExpression(value);
        }

        if (_current.Type == TokenType.GroupStart)
        {
            Advance();
            var inner = ParseExpressionInternal();
            if (_current.Type != TokenType.GroupEnd) throw new FormatException("Missing closing parenthesis");

            Advance();
            return new GroupExpression(inner);
        }

        throw new FormatException($"Unexpected token: {_current.Value}");
    }
}
