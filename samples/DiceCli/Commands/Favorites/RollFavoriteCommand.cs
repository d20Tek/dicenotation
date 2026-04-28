using d20Tek.DiceNotation;
using DiceCli.Models;

namespace DiceCli.Commands.Favorites;

internal class RollFavoriteCommand(IAnsiConsole console, LowDb<FavoriteRollsDocument> db, IDice dice)
    : Command<RollFavoriteCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-i|--id <IDENTIFIER>")]
        [Description("The unique identifier of favorite to roll.")]
        public string? Id { get; init; }
    }

    private readonly IAnsiConsole _console = console;
    private readonly LowDb<FavoriteRollsDocument> _db = db;
    private readonly IDice _dice = dice;

    protected override int Execute(CommandContext context, Settings request, CancellationToken _)
    {
        _console.WriteMessages("Roll Favorite", "-------------");

        var id = _console.PromptIfNull(request.Id, "Enter the unique identifier:");
        _console.WriteMessages();

        return RollFavorite(GetFavoriteById(id), id);
    }

    private Optional<FavoriteRoll> GetFavoriteById(string id) =>
        _db.Get().Rolls.FirstOrDefault(r => r.Id == id).ToOptional();

    private int RollFavorite(Optional<FavoriteRoll> favoriteRoll, string id) =>
        favoriteRoll.Match(
            r =>
            {
                _console.WriteMessages($"[yellow]Rolling dice for {r.Name} - {r.Expression}[/]:");
                return _console.WriteDiceResult(_dice.Roll(r.Expression));
            },
            () =>
            {
                _console.WriteMessages($"[red]Error:[/] Favorite roll with identifier '{id}' does not exist.");
                return 0;
            });
}
