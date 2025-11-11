using d20Tek.DiceNotation;
using DiceCli.Common;
using System.ComponentModel;

namespace DiceCli.Commands;

internal sealed class RollCommand(IAnsiConsole console, IDice dice)
    : Command<RollCommand.Request>
{
    public sealed class Request : CommandSettings
    {
        [CommandArgument(0, "<DICE-NOTATION>")]
        [Description("Dice notation string to roll (ex: 1d20, 1d8+3, etc).")]
        public string? Notation { get; set; }
    }

    private readonly IAnsiConsole _console = console;
    private readonly IDice _dice = dice;

    public override int Execute(CommandContext context, Request request, CancellationToken cancellation)
    {
        _console.MarkupLine($"Rolling dice for '{request.Notation}':");

        var result = _dice.Roll(request.Notation!);
        return _console.WriteDiceResult(result);
    }
}
