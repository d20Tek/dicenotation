//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation;

public partial class DiceParser
{
    private int? ChooseLookAhead(List<string> tokens, int opPosition, int numDice, ref int length)
    {
        int? result = null;

        int keepPos = tokens.IndexOf("k", opPosition);
        if (keepPos > 0)
        {
            // if that operator is found, then get the next number token
            result = int.Parse(tokens[keepPos + 1]);
            length += 2;
        }

        int dropPos = tokens.IndexOf("p", opPosition);
        if (dropPos > 0)
        {
            // if that operator is found, then get the next number token
            result = numDice - int.Parse(tokens[dropPos + 1]);
            length += 2;
        }

        int lowestPos = tokens.IndexOf("l", opPosition);
        if (lowestPos > 0)
        {
            // if that operator is found, then get the next number token
            result = -int.Parse(tokens[lowestPos + 1]);
            length += 2;
        }

        return result;
    }

    private int? ExplodeLookAhead(List<string> tokens, int opPosition, int sides, ref int length)
    {
        int? result = null;

        int explodePos = tokens.IndexOf("!", opPosition);
        if (explodePos > 0)
        {
            // if that operator is found, then get the associated number
            if (explodePos + 1 < tokens.Count && char.IsDigit(tokens[explodePos + 1], 0))
            {
                result = int.Parse(tokens[explodePos + 1]);
                length += 2;
            }
            else
            {
                result = sides;
                length++;
            }
        }

        return result;
    }

    private void EvaluateDiceTerm(
        List<TermResult> results,
        List<string> tokens,
        IDieRoller dieRoller,
        int opPosition,
        int length,
        IExpressionTerm term)
    {
        IReadOnlyList<TermResult> t = term.CalculateResults(dieRoller);
        int value = t.Sum(r => (int)Math.Round(r.AppliesToResultCalculation ? r.Value * r.Scalar : 0));
        results.AddRange(t);

        // put the evaluation result in the first entry and remove
        // the remaining processed tokens
        tokens[opPosition - 1] = value.ToString();
        tokens.RemoveRange(opPosition, length);
    }
}
