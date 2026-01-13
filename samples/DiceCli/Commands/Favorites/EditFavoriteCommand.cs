using D20Tek.Functional;
using DiceCli.Models;
using System;
using static DiceCli.Commands.RollCommand;

namespace DiceCli.Commands.Favorites;

internal class EditFavoriteCommand(IAnsiConsole console, LowDb<FavoriteRollsDocument> db)
    : Command<EditFavoriteCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-i|--id <IDENTIFIER>")]
        [Description("The unique identifier for the favorite roll to edit.")]
        public string? Id { get; init; }

        [CommandOption("-n|--name <DISPLAY-NAME>")]
        [Description("Updated display name for the favorite roll.")]
        public string? Name { get; init; }

        [CommandOption("-e|--expression <EXPRESSION>")]
        [Description("Updated dice notation expression for the favorite roll.")]
        public string? Expression { get; init; }
    }

    private readonly IAnsiConsole _console = console;
    private readonly LowDb<FavoriteRollsDocument> _db = db;

    public override int Execute(CommandContext context, Settings request, CancellationToken _)
    {
        _console.WriteMessages("Edit Favorite", "-------------");
        var id = _console.PromptIfNull(request.Id, "Enter the identifier of favorite to edit:");
        _db.Update(x => UpdateFavoriteRoll(x, id, request));

        return 0;
    }

    private void UpdateFavoriteRoll(FavoriteRollsDocument doc, string favoriteId, Settings request)
    {
        var favoriteRoll = doc.Rolls.FirstOrDefault(r => r.Id == favoriteId);
        (favoriteRoll is null).IfTrueOrElse(
            () => _console.WriteMessages($"[red]Error:[/] Favorite roll with identifier '{favoriteId}' does not exist."),
            () =>
            {
                favoriteRoll!.Update(
                    _console.PromptIfNull(request.Name, "Change the roll display name", favoriteRoll.Name),
                    _console.PromptIfNull(request.Expression, "Change the dice notation", favoriteRoll.Expression));

                _console.WriteMessages();
                _console.WriteMessages($"[green]Favorite Roll '{favoriteRoll!.Id}' was updated.[/]");
            });
    }
}
