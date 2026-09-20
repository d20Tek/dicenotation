using d20Tek.DiceNotation.Results;

namespace DiceCli.Common;

internal static class DiceResultDisplay
{
    public static int WriteDiceResult(this IAnsiConsole console, DiceResult diceResult)
    {
        if (diceResult.HasError)
        {
            console.MarkupLine($"[red]{diceResult.Error}[/]" ?? "");
            return -1;
        }

        console.WriteMessages(
            $"Total result: {diceResult.Value}",
            $"Dice rolls: {diceResult.VerboseDisplayText}");

        WriteRoleSummary(console, "Kept", diceResult.KeptResults);
        WriteRoleSummary(console, "Dropped", diceResult.DroppedResults);
        WriteRoleSummary(console, "Exploded", diceResult.ExplodedResults);
        WriteRoleSummary(console, "Rerolled", diceResult.RerolledResults);
        return 0;
    }

    private static void WriteRoleSummary(IAnsiConsole console, string label, IReadOnlyList<TermResult> results)
    {
        if (results.Count == 0) return;

        var values = string.Join(", ", results.Select(r => r.Value));
        console.WriteMessages($"{label}: {values}");
    }
}
