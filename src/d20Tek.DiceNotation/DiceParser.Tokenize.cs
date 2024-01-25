//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation;

public partial class DiceParser
{
    public List<string> Tokenize(string expression)
    {
        List<string> tokens = new List<string>();
        string vector = string.Empty;

        // first clean up expression
        expression = this.CorrectExpression(expression);

        // loop through the expression characters
        for (var i = 0; i < expression.Length; i++)
        {
            var ch = expression[i].ToString();
            var next = (i + 1) >= expression.Length ? string.Empty : expression[i + 1].ToString();
            var prev = i == 0 ? string.Empty : expression[i - 1].ToString();

            if (char.IsLetter(ch, 0))
            {
                // if it's a letter, then increment the char position until we find the end of the text
                this.TokenizeLetters(expression, tokens, ref vector, ref i, ch, prev);
            }
            else if (char.IsDigit(ch, 0))
            {
                // if it's a digit, then increment char until you find the end of the number
                this.TokenizeNumbers(expression, tokens, ref vector, ref i, ch);
            }
            else if ((i + 1) < expression.Length &&
                     this.Operators.Contains(ch) &&
                     char.IsDigit(expression[i + 1]) && (i == 0 ||
                     ((i - 1) > 0 && prev == this.GroupStartOperator)))
            {
                // if the above is true, then, the token for that negative number will be "-1", not "-","1".
                this.TokenizeUnaryOperators(expression, tokens, ref vector, ref i, ch);
            }
            else if (ch == this.GroupStartOperator)
            {
                // if an open grouping, then if we didn't have an operator, then append the default operator.
                if (i != 0 && (char.IsDigit(prev, 0) || prev == this.GroupEndOperator))
                {
                    tokens.Add(this.DefaultOperator);
                }

                tokens.Add(ch.ToString());
            }
            else if (ch == this.GroupEndOperator)
            {
                // if closing grouping and there's no operator, then append the default operator.
                tokens.Add(ch);

                if ((i + 1) < expression.Length && (char.IsDigit(next, 0) ||
                    (next != this.GroupEndOperator && !this.Operators.Contains(next))))
                {
                    tokens.Add(this.DefaultOperator);
                }
            }
            else
            {
                // if not recognized character just add it as its own token.
                tokens.Add(ch.ToString());
            }
        }

        return tokens;
    }

    private string CorrectExpression(string expression)
    {
        // first verify we have an expression to parse.
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new ArgumentNullException(
                nameof(expression),
                "The expression string is empty or null.");
        }

        // first remove any whitespace from the expression
        string result = WhitespaceRegex.Replace(expression.ToLower(), string.Empty);

        // then replace duplicate operators with their resulting value
        result = result.Replace("+-", "-");
        result = result.Replace("-+", "-");
        result = result.Replace("--", "+");

        // replace any percentile notation with appropriate dice faces
        result = result.Replace(PercentileNotation, D100EquivalentNotation);

        return result;
    }

    private void TokenizeUnaryOperators(
        string expression,
        List<string> tokens,
        ref string substring,
        ref int position,
        string ch)
    {
        // adds the operator to the current token string
        substring += ch;

        while ((position + 1) < expression.Length &&
               (char.IsDigit(expression[position + 1]) ||
               expression[position + 1].ToString() == DecimalSeparator))
        {
            // handles processing a number after a single unary operator
            position++;
            substring += expression[position];
        }

        // now add the element to the token list
        tokens.Add(substring);
        substring = string.Empty;
    }

    private void TokenizeNumbers(
        string expression,
        List<string> tokens,
        ref string substring,
        ref int position,
        string ch)
    {
        substring += ch;

        while ((position + 1) < expression.Length &&
               (char.IsDigit(expression[position + 1]) ||
               expression[position + 1].ToString() == DecimalSeparator))
        {
            // keep processing this element while you have digits (support multi-digit numbers)
            position++;
            substring += expression[position];
        }

        // now add the element to the token list
        tokens.Add(substring);
        substring = string.Empty;
    }

    private void TokenizeLetters(
        string expression,
        List<string> tokens,
        ref string substring,
        ref int position,
        string ch,
        string prev)
    {
        if (position != 0 &&
            (char.IsDigit(prev, 0) || prev == this.GroupEndOperator) &&
            !this.Operators.Contains(ch))
        {
            tokens.Add(this.DefaultOperator);
        }

        // if we have a single die operator (d, f), then default to having a default
        // number of dice (1)
        if ((ch == "d" || ch == "f") && (string.IsNullOrEmpty(prev) ||
                                         this.Operators.Contains(prev) ||
                                         prev == this.GroupStartOperator))
        {
            tokens.Add(this.DefaultNumDice);
        }

        // append the current character
        substring += ch;

        if (!this.Operators.Contains(ch))
        {
            // if the character isn't an operator, then loop ahead while the expression has letters
            while ((position + 1) < expression.Length &&
                   char.IsLetterOrDigit(expression[position + 1]))
            {
                position++;
                substring += expression[position];
            }
        }

        // now add the element to the tokens
        tokens.Add(substring);
        substring = string.Empty;
    }
}
