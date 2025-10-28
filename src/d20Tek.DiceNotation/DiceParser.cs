//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using System.Globalization;
using System.Text.RegularExpressions;

namespace d20Tek.DiceNotation
{
    public partial class DiceParser
    {
        private const string PercentileNotation = "d%";
        private const string D100EquivalentNotation = "d100";
        private const string DefaultOperator = "x";
        private const string GroupStartOperator = "(";
        private const string GroupEndOperator = ")";
        private const string DefaultNumDice = "1";

        private static readonly Regex WhitespaceRegex = new(@"\s+");
        private static readonly string DecimalSeparator =
            CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;

        private static readonly List<string> Operators = ["d", "f", "k", "p", "l", "!", "/", "x", "*", "-", "+"];
        private static readonly Dictionary<string, Func<int, int, int>> OperatorActions = new()
        {
            { "/", (numberA, numberB) => numberA / numberB },
            { "x", (numberA, numberB) => numberA * numberB },
            { "*", (numberA, numberB) => numberA * numberB },
            { "-", (numberA, numberB) => numberA - numberB },
            { "+", (numberA, numberB) => numberA + numberB },
        };

        private IDiceConfiguration? _config = null;
    }
}
