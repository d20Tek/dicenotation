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

    private void Advance() => _current = _tokens.MoveNext() ? _tokens.Current : Constants.EndToken;

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
        var left = ParseUnary();

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
            while (_current is { Type: TokenType.Operator, Value: "k" or "p" or "l" or "!" })
            {
                string mod = _current.Value;
                Advance();

                if (mod == "!")
                {
                    // exploding dice (optionally with threshold)
                    if (_current.Type == TokenType.Number)
                    {
                        explode = int.Parse(_current.Value);
                        Advance();
                    }
                    else
                    {
                        explode = sides; // default threshold = max value
                    }
                    continue;
                }

                // For k/p/l, expect a numeric argument
                if (_current.Type != TokenType.Number)
                    throw new FormatException($"Expected number after '{mod}' modifier.");

                int n = int.Parse(_current.Value);
                Advance();

                keep = mod switch
                {
                    "k" => n,                      // keep n highest
                    "p" => Math.Max(0, ((left as NumberExpression)?.Value ?? 1) - n), // drop n lowest → keep (dice - n)
                    "l" => -n,                     // keep n lowest (negative indicates lowest)
                    _ => keep
                };
            }

            if (left is not NumberExpression nExpr) throw new FormatException("Expected number before die operator");

            return new DiceExpression(nExpr.Value, sides, keep, explode);
        }

        return left;
    }

    private Expression ParseUnary()
    {
        // Handle unary +/- operators
        if (_current is { Type: TokenType.Operator, Value: "+" or "-" })
        {
            string op = _current.Value;
            Advance();
            var operand = ParseUnary(); // recursion handles multiple unary signs like "--5"
            return op == "-" ? new UnaryExpression(op, operand) : operand;
        }

        return ParsePrimary();
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
