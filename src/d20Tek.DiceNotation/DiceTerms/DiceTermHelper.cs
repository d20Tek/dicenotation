using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.DiceTerms;

internal static class DiceTermHelper
{
    private const int MaxRerollsAllowed = 1000;
    private const string MaxReRollsError = "Rolling dice past the maximum allowed number of rerolls.";
    public const string DiceFormatResultType = "{0}.d{1}";
    public const string DiceFormatDiceTermText = "{0}d{1}{2}";
    public const string FormatDiceMultiplyTermText = "{0}d{1}{2}x{3}";
    public const string FormatDiceDivideTermText = "{0}d{1}{2}/{3}";

    public static int EvaluateExplodingDice(int rerolls, int value, int? exploding)
    {
        if (exploding is null || value < exploding) return rerolls;
        EnsureMaxRerolls(rerolls);

        return ++rerolls;
    }

    public static List<TermResult> OrderTermResults(List<TermResult> results, int? choose)
    {
        int tempChoose = choose ?? results.Count;
        var ordered = tempChoose > 0 ?
                        [.. results.OrderByDescending(d => d.Value)] :
                        results.OrderBy(d => d.Value).ToList();

        for (int i = Math.Abs(tempChoose); i < ordered.Count; i++)
        {
            ordered[i].AppliesToResultCalculation = false;
        }

        return ordered;
    }

    private static void EnsureMaxRerolls(int rerolls)
    {
        if (rerolls > MaxRerollsAllowed) throw new OverflowException(MaxReRollsError);
    }
}
