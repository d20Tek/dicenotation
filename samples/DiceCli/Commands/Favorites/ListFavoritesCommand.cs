using DiceCli.Models;
using Spectre.Console;

namespace DiceCli.Commands.Favorites;

internal class ListFavoritesCommand(IAnsiConsole console, LowDb<FavoriteRollsDocument> db) : Command
{
    private readonly IAnsiConsole _console = console;
    private readonly LowDb<FavoriteRollsDocument> _db = db;

    public override int Execute(CommandContext context, CancellationToken cancellationToken) =>
        db.Get().ToIdentity()
          .Iter(favoriteDoc =>
                (favoriteDoc.Rolls.Count is 0).IfTrueOrElse(
                    () => _console.WriteLine("No favorite rolls found. Add some favorites..."),
                    () => DisplayRollsList(favoriteDoc.Rolls)))
          .Map(_ => 0);

    private void DisplayRollsList(IEnumerable<FavoriteRoll> rolls) =>
        CreateFavoritesTable()
            .Iter(t => rolls.ForEach(roll => t.AddRow(roll.Id, roll.Name, roll.Expression)))
            .Iter(t => _console.Write(t));

    private static Identity<Table> CreateFavoritesTable() =>
        new Table().AddColumn("Identifier")
                   .AddColumn("Display Name")
                   .AddColumn("Notation");
}
