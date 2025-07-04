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
        private static readonly Regex WhitespaceRegex = new(@"\s+");
        private static readonly string DecimalSeparator =
            CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;

        private IDiceConfiguration? config = null;

        public List<string> Operators { get; } =
            ["d", "f", "k", "p", "l", "!", "/", "x", "*", "-", "+"];

        public Dictionary<string, Func<int, int, int>> OperatorActions { get; } =
            new Dictionary<string, Func<int, int, int>>
        {
            { "/", (numberA, numberB) => numberA / numberB },
            { "x", (numberA, numberB) => numberA * numberB },
            { "*", (numberA, numberB) => numberA * numberB },
            { "-", (numberA, numberB) => numberA - numberB },
            { "+", (numberA, numberB) => numberA + numberB },
        };

        public string DefaultOperator { get; set; } = "x";

        public string GroupStartOperator { get; set; } = "(";

        public string GroupEndOperator { get; set; } = ")";

        public string DefaultNumDice { get; set; } = "1";
    }
}
