using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation;

internal static class Constants
{
    public const string FudgeDiceIdentifier = "f";
    public const string DefaultLocale = "en-us";
    public const string DefaultParameterName = "none";
    public const string ChooseToken = "k";
    public const string ExplodingToken = "!";

    public const int MaxRerollsAllowed = 1000;
    public const string DiceFormatResultType = "{0}.d{1}";
    public const string DiceFormatDiceTermText = "{0}d{1}{2}";
    public const string FormatDiceMultiplyTermText = "{0}d{1}{2}x{3}";
    public const string FormatDiceDivideTermText = "{0}d{1}{2}/{3}";
    public const string FudgeFormatResultType = "{0}.dF";
    public const string FudgeFormatDiceTermText = "{0}f{2}";
    public const int FudgeNumberSides = 3;
    public const int FudgeFactor = -2;

    public static string JoinSigns(IEnumerable<IExpressionTerm> terms) => string.Join("+", terms).Replace("+-", "-");

    public static string FormatDiceResult(DiceResult dr) => $"{dr.Value} ({dr.DiceExpression})";

    public static class Config
    {
        public const int MinDieSides = 2;
        public const int MaxDieSides = 1000;
        public const int DefaultBoundedMin = 1;
        public const int DefaultDieSides = 6;
    }

    public static class Errors
    {
        public const string UnexpectedConverterType = "Unexpected type passed to converter.";
        public const string NotDiceResult = "Object not of type DiceResult.";
        public const string MaxReRollsError = "Rolling dice past the maximum allowed number of rerolls.";

        public static string NotAssignableType(Type source, Type target) => $"{target} is not of type {source}.";
    }
}
