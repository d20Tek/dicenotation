using DiceCli.Models;

namespace DiceCli.Commands.Favorites;

internal class DeleteFavoriteCommand(IAnsiConsole console, LowDb<FavoriteRollsDocument> db)
    : Command<DeleteFavoriteCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-i|--id <IDENTIFIER>")]
        [Description("The unique identifier of favorite roll to delete.")]
        public string? Id { get; init; }
    }

    private readonly IAnsiConsole _console = console;
    private readonly LowDb<FavoriteRollsDocument> _db = db;

    protected override int Execute(CommandContext context, Settings request, CancellationToken _)
    {
        _console.WriteMessages("Delete Favorite", "---------------");
        
        var id = _console.PromptIfNull(request.Id, "Enter the unique identifier:");
        _console.WriteMessages();

        _db.Update(x => DeleteFavoriteRoll(x, id));
        return 0;
    }

    private void DeleteFavoriteRoll(FavoriteRollsDocument doc, string id) =>
        doc.Rolls.Any(t => t.Id == id).IfTrueOrElse(
            () =>
            {
                doc.Rolls.RemoveWhere(r => r.Id == id);
                _console.WriteMessages($"[green]Favorite Roll '{id}' was deleted.[/]");
            },
            () => _console.WriteMessages($"[red]Error:[/] Favorite roll with identifier '{id}' does not exist."));
}
