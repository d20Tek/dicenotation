using System.Text.RegularExpressions;

namespace d20Tek.DiceNotation.Parser;

internal sealed partial class Lexer
{
    private static Token _defaultDiceNumberToken = new(TokenType.Number, "1");

    public IEnumerable<Token> Tokenize(string expression)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(expression);
        expression = CleanExpression(expression);
        Token? previous = null;

        foreach (Match match in TokenRegex().Matches(expression))
        {
            string v = match.Value;
            Token token = v switch
            {
                "(" => new(TokenType.GroupStart, v),
                ")" => new(TokenType.GroupEnd, v),
                "+" or "-" or "*" or "/" or "!" or "k" or "p" or "l" or "f" or "d"
                    => new(TokenType.Operator, v),
                _ when int.TryParse(v, out _) => new(TokenType.Number, v),
                _ => new(TokenType.Identifier, v),
            };

            if (HasDefaultDiceNumber(token, previous)) yield return _defaultDiceNumberToken;

            yield return token;
            previous = token;
        }

        yield return new(TokenType.EndOfInput, string.Empty);
    }

    private static bool HasDefaultDiceNumber(Token t, Token? p) =>
        t.Value is "d" or "f" && (p is null || p.Type is not TokenType.Number && p.Value is not ")");

    private static string CleanExpression(string expression) =>
        expression.Replace("d%", "d100")
                  .Replace("--", "+")
                  .Replace("+-", "-")
                  .Replace("-+", "-")
                  .ToLowerInvariant();

    [GeneratedRegex(@"(\d+|[A-Za-z]+|[()+\-*/!kpdl])", RegexOptions.Compiled)]
    private static partial Regex TokenRegex();
}
