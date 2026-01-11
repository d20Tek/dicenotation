namespace DiceCli.Commands.Favorites;

internal static class CommandConfiguration
{
    public static IConfigurator ConfigureFavoritesCommands(this IConfigurator config)
    {
        config.AddBranch("favorites", favoritesConfig =>
        {
            favoritesConfig.SetDescription("Commands to manage and use favorite rolls.");

            favoritesConfig.AddCommand<ListFavoritesCommand>("list")
                           .WithAlias("ls")
                           .WithDescription("Lists all favorite rolls.")
                           .WithExample(["favorites", "list"]);

            favoritesConfig.AddCommand<AddFavoriteCommand>("add")
                           .WithAlias("a")
                           .WithDescription("Adds a new favorite roll.")
                           .WithExample(["favorites", "add", "-i", "attack-roll", "-n", "Attack (Longsword)", "-e", "1d20+5"]);

            favoritesConfig.AddCommand<EditFavoriteCommand>("edit")
                           .WithAlias("e")
                           .WithDescription("Edits an existing favorite roll.")
                           .WithExample(["favorites", "edit", "-i", "attack-roll", "-n", "Attack (Short sword)", "-e", "1d20+7"]);

            favoritesConfig.AddCommand<DeleteFavoriteCommand>("delete")
                           .WithAlias("del")
                           .WithAlias("d")
                           .WithDescription("Removes a favorite roll by name.")
                           .WithExample(["favorites", "delete", "-i", "damage-roll"]);

            favoritesConfig.AddCommand<RollFavoriteCommand>("roll")
                           .WithAlias("r")
                           .WithDescription("Rolls a favorite roll by name.")
                           .WithExample(["favorites", "roll", "-i", "attack-roll"]);
        }).WithAlias("favs")
          .WithAlias("f");

        return config;
    }
}
