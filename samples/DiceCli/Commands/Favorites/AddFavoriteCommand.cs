using DiceCli.Models;

namespace DiceCli.Commands.Favorites;

internal class AddFavoriteCommand(IAnsiConsole console, LowDb<FavoriteRollsDocument> db)
    : Command<AddFavoriteCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-i|--id <IDENTIFIER>")]
        [Description("The unique identifier for this favorite roll.")]
        public string? Id { get; init; }

        [CommandOption("-n|--name <DISPLAY-NAME>")]
        [Description("The display name for the favorite roll.")]
        public string? Name { get; init; }

        [CommandOption("-e|--expression <EXPRESSION>")]
        [Description("The dice notation expression for the favorite roll.")]
        public string? Expression { get; init; }
    }

    private readonly IAnsiConsole _console = console;
    private readonly LowDb<FavoriteRollsDocument> _db = db;

    public override int Execute(CommandContext context, Settings request, CancellationToken _)
    {
        var favoriteRoll = CreateFavoriteRoll(request);
        _console.WriteLine();

        _db.Update(x => SaveFavoriteRoll(x, favoriteRoll));
        return 0;
    }

    private FavoriteRoll CreateFavoriteRoll(Settings request) => new()
    {
        Id = _console.PromptIfNull(request.Id, "Enter the unique identifier:"),
        Name = _console.PromptIfNull(request.Name, "Enter the roll display name:"),
        Expression = _console.PromptIfNull(request.Expression, "Enter the dice notation:")
    };

    private void SaveFavoriteRoll(FavoriteRollsDocument doc, FavoriteRoll favoriteRoll) => 
        doc.Rolls.Any(t => t.Id == favoriteRoll.Id).IfTrueOrElse(
            () => _console.WriteMessages(
                    $"[red]Error:[/] Favorite roll with identifier '{favoriteRoll.Id}' already exists.",
                    "Select a different unique identifier."),
            () =>
            {
                doc.Rolls.Add(favoriteRoll);
                _console.WriteMessages($"[green]Favorite Roll '{favoriteRoll.Id}' was created.[/]");
            });
}
