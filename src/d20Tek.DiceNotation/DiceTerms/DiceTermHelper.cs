using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.DiceTerms;

internal static class DiceTermHelper
{
    public static int EvaluateExplodingDice(int rerolls, int value, int? exploding)
    {
        if (exploding is null || value < exploding) return rerolls;
        EnsureMaxRerolls(rerolls);

        return ++rerolls;
    }

    public static List<TermResult> OrderTermResults(List<TermResult> results, int? choose)
    {
        var tempChoose = choose ?? results.Count;
        var ordered = tempChoose > 0 ?
                        [.. results.OrderByDescending(d => d.Value)] :
                        results.OrderBy(d => d.Value).ToList();

        for (int i = 0; i < ordered.Count; i++)
        {
            var dropped = i >= Math.Abs(tempChoose);
            if (dropped)
            {
                ordered[i].AppliesToResultCalculation = false;
            }

            ordered[i].Roles = (ordered[i].Roles & ~(DieRollRole.Kept | DieRollRole.Dropped)) |
                               (dropped ? DieRollRole.Dropped : DieRollRole.Kept);
        }

        return ordered;
    }

    private static void EnsureMaxRerolls(int rerolls)
    {
        if (rerolls > Constants.MaxRerollsAllowed) throw new OverflowException(Constants.Errors.MaxReRollsError);
    }
}
