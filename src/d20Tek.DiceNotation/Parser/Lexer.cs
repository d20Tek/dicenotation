using System.Text.RegularExpressions;

namespace d20Tek.DiceNotation.Parser;

internal sealed partial class Lexer
{
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
                "(" => new Token(TokenType.GroupStart, v),
                ")" => new Token(TokenType.GroupEnd, v),
                "+" or "-" or "*" or "/" or "!" or "k" or "p" or "l" or "f" or "d"
                    => new Token(TokenType.Operator, v),
                _ when int.TryParse(v, out _) => new Token(TokenType.Number, v),
                _ => new Token(TokenType.Identifier, v),
            };

            if (token.Value is "d" or "f" &&
                (previous is null || previous.Type is not TokenType.Number && previous.Value is not ")"))
            {
                yield return new Token(TokenType.Number, "1");
            }

            yield return token;
            previous = token;
        }

        yield return new Token(TokenType.EndOfInput, string.Empty);
    }

    private static string CleanExpression(string expression) =>
        expression.Replace("d%", "d100")
                  .Replace("--", "+")
                  .Replace("+-", "-")
                  .Replace("-+", "-")
                  .ToLowerInvariant();

    [GeneratedRegex(@"(\d+|[A-Za-z]+|[()+\-*/!kpdl])", RegexOptions.Compiled)]
    private static partial Regex TokenRegex();
}
