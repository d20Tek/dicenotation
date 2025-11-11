using d20Tek.DiceNotation.Results;

namespace DiceCli.Common;

internal static class DiceResultDisplay
{
    private static readonly TermResultListConverter _converter = new();

    public static int WriteDiceResult(this IAnsiConsole console, DiceResult diceResult)
    {
        if (diceResult.HasError)
        {
            console.MarkupLine($"[red]{diceResult.Error}[/]" ?? "");
            return -1;
        }

        console.WriteMessages(
            $"Total result: {diceResult.Value}",
            $"Dice rolls: {_converter.Convert(diceResult.Results, typeof(string), null!, "default")}");
        return 0;
    }
}
