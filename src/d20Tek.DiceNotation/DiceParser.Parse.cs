//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation;

public partial class DiceParser
{
    public DiceResult Parse(string expression, IDiceConfiguration config, IDieRoller dieRoller)
    {
        this.config = config;

        // first clean up expression
        expression = this.CorrectExpression(expression);

        // then break the expression down into tokens.
        List<string> tokens = this.Tokenize(expression);

        // finally parse and evaluate the expression tokens
        return this.ParseLogic(expression, tokens, dieRoller);
    }

    private DiceResult ParseLogic(string expression, List<string> tokens, IDieRoller dieRoller)
    {
        List<TermResult> results = new List<TermResult>();

        while (tokens.IndexOf(this.GroupStartOperator) != -1)
        {
            // getting data between grouping symbols: "(" and ")"
            int open = tokens.LastIndexOf(this.GroupStartOperator);
            int close = tokens.IndexOf(this.GroupEndOperator, open);

            if (open >= close)
            {
                throw new ArithmeticException("No matching close-open parenthesis.");
            }

            // get a subexpression list for elements within the grouping symbols
            List<string> subExpression = new List<string>();
            for (var i = open + 1; i < close; i++)
            {
                subExpression.Add(tokens[i]);
            }

            // run the operations on the subexpression
            int subValue = this.HandleBasicOperation(results, subExpression, dieRoller);

            // when subexpression calculation is done, replace the grouping start symbol
            // and removing the tokens for the subexpression from the token list
            tokens[open] = subValue.ToString();
            tokens.RemoveRange(open + 1, close - open);
        }

        // at this point, we should have replaced all groups in the expression
        // with the appropriate values, so need to calculate last simple expression
        int value = this.HandleBasicOperation(results, tokens, dieRoller);

        // now return the dice result from the final value and TermResults list
        return new DiceResult(
            expression,
            value,
            results,
            dieRoller.GetType().ToString(),
            this.config!);
    }

    private int HandleBasicOperation(
        List<TermResult> results,
        List<string> tokens,
        IDieRoller dieRoller)
    {
        if (tokens.Count == 0)
        {
            // if there's nothing in the list, just return 0
            return 0;
        }
        else if (tokens.Count == 1)
        {
            // if there is only one token, then it much be a constant
            return int.Parse(tokens[0]);
        }

        // loop through each operator in our operator list
        // operators order in the list signify their order of operations
        foreach (var op in this.Operators)
        {
            // loop through all of the tokens until we find the operator in the list
            while (tokens.IndexOf(op) != -1)
            {
                try
                {
                    if (op == "d")
                    {
                        // if current operator is the die operator, then process
                        // that part of the expression accordingly
                        this.HandleDieOperator(results, tokens, op, dieRoller);
                    }
                    else if (op == "f")
                    {
                        // if current operator is the fudge die operator, then process
                        // that part of the expression accordingly
                        this.HandleFudgeOperator(results, tokens, op, dieRoller);
                    }
                    else
                    {
                        // otherwise, treat the operator as an arimethic operator,
                        // and perform the correct math operation
                        this.HandleArithmeticOperators(tokens, op);
                    }
                }
                catch (Exception ex)
                {
                    // if any error happens within this processing, then throw an exception
                    throw new FormatException("Dice expression string is incorrect format.", ex);
                }
            }

            // if we are out of tokens, then just stop processing
            if (tokens.Count == 0)
            {
                break;
            }
        }

        if (tokens.Count == 1)
        {
            // if there is only one token left, then return it as evaluation of list of tokens
            return int.Parse(tokens[0]);
        }
        else
        {
            // if there are left over toknes, then the parsing/evaluation failed
            throw new FormatException(
                "Dice expression string is incorrect format: unexpected symbols in the string expression.");
        }
    }

    private void HandleArithmeticOperators(List<string> tokens, string op)
    {
        // find the previous and next numbers in the token list
        var opPosition = tokens.IndexOf(op);
        var numberA = int.Parse(tokens[opPosition - 1]);
        var numberB = int.Parse(tokens[opPosition + 1]);

        // find the action that corresponds to the current operator, then
        // run that action to evaluate the math function
        int result = this.OperatorActions[op](numberA, numberB);

        // put the evaluation result in the first entry and remove
        // the remaining processed tokens
        tokens[opPosition - 1] = result.ToString();
        tokens.RemoveRange(opPosition, 2);
    }

    private void HandleDieOperator(
        List<TermResult> results,
        List<string> tokens,
        string op,
        IDieRoller dieRoller)
    {
        if (tokens.IndexOf("f") >= 0)
        {
            throw new FormatException(
                "Fudge dice and regular dice cannot be used in the same expression");
        }

        // find the previous and next numbers in the token list
        int opPosition = tokens.IndexOf(op);
        int length = 0;
        int sides;

        int numDice = int.Parse(tokens[opPosition - 1]);

        // allow default value for dice (if not digit is specified)
        if (opPosition + 1 < tokens.Count && char.IsDigit(tokens[opPosition + 1], 0))
        {
            sides = int.Parse(tokens[opPosition + 1]);
            length += 2;
        }
        else
        {
            sides = this.config!.DefaultDieSides;
            length++;
        }

        // look-ahead to find other dice operators (like the choose-keep/drop operators)
        int? choose = this.ChooseLookAhead(tokens, opPosition, numDice, ref length);
        int? explode = this.ExplodeLookAhead(tokens, opPosition, sides, ref length);

        // create a dice term based on the values
        DiceTerm term = new DiceTerm(numDice, sides, 1, choose, explode);

        // then evaluate the dice term to roll dice and get the result
        this.EvaluateDiceTerm(results, tokens, dieRoller, opPosition, length, term);
    }

    private void HandleFudgeOperator(
        List<TermResult> results,
        List<string> tokens,
        string op,
        IDieRoller dieRoller)
    {
        // find the previous and next numbers in the token list
        int opPosition = tokens.IndexOf(op);

        int numDice = int.Parse(tokens[opPosition - 1]);
        int length = 1;

        // look-ahead to find other dice operators (like the choose-keep/drop operators)
        int? choose = this.ChooseLookAhead(tokens, opPosition, numDice, ref length);

        // create a dice term based on the values
        IExpressionTerm term = new FudgeDiceTerm(numDice, choose);

        // then evaluate the dice term to roll dice and get the result
        this.EvaluateDiceTerm(results, tokens, dieRoller, opPosition, length, term);
    }
}
