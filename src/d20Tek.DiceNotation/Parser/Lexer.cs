using System.Text.RegularExpressions;

namespace d20Tek.DiceNotation.Parser;

internal sealed partial class Lexer
{
    public IEnumerable<Token> Tokenize(string expression)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(expression);
        Token? previous = null;
        expression = CleanExpression(expression);

        foreach (Match match in TokenRegex().Matches(expression))
        {
            var token = MapValueToToken(match.Value);

            if (HasDefaultDiceNumber(token, previous)) yield return Constants.DefaultDiceNumberToken;

            yield return token;
            previous = token;
        }

        yield return new(TokenType.EndOfInput, string.Empty);
    }

    private static Token MapValueToToken(string v) => v switch
    {
        Constants.GroupStartOperator => new(TokenType.GroupStart, v),
        Constants.GroupEndOperator => new(TokenType.GroupEnd, v),
        _ when Constants.Operators.Contains(v) => new(TokenType.Operator, v),
        _ when int.TryParse(v, out _) => new(TokenType.Number, v),
        _ => new(TokenType.Identifier, v),
    };

    private static bool HasDefaultDiceNumber(Token t, Token? p) =>
        t.Value is Constants.DiceOperator or Constants.FudgeDiceOperator && 
            (p is null || p.Type is not TokenType.Number && p.Value is not Constants.GroupEndOperator);

    private static string CleanExpression(string expression) =>
        expression.Replace(Constants.PercentileNotation, Constants.D100EquivalentNotation)
                  .Replace("--", "+")
                  .Replace("+-", "-")
                  .Replace("-+", "-")
                  .ToLowerInvariant();

    [GeneratedRegex(@"(\d+|[A-Za-z]+|[()+\-*/!kpdl])", RegexOptions.Compiled)]
    private static partial Regex TokenRegex();
}
