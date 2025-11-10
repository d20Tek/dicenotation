using d20Tek.DiceNotation;
using d20Tek.DiceNotation.Results;
using System.ComponentModel;

namespace DiceCli.Commands;

internal sealed class RollCommand(IAnsiConsole console, IDice dice, TermResultListConverter converter)
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
    private readonly TermResultListConverter _converter = converter;

    public override int Execute(CommandContext context, Request request)
    {
        _console.MarkupLine($"Rolling dice for '{request.Notation}':");
        var result = _dice.Roll(request.Notation!);
        if (result.HasError)
        {
            _console.MarkupLine($"[red]{result.Error}[/]" ?? "");
            return -1;
        }

        _console.MarkupLine($"Total result: {result.Value}");
        _console.MarkupLine($"Dice rolls: {_converter.Convert(result.Results, typeof(string), null!, "default")}");

        return 0;
    }

}
