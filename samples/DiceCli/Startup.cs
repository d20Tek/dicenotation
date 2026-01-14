using d20Tek.DiceNotation;
using d20Tek.DiceNotation.DieRoller;
using D20Tek.Spectre.Console.Extensions.Injection;
using DiceCli.Commands;
using DiceCli.Commands.Favorites;
using DiceCli.Models;

namespace DiceCli;

internal sealed class Startup : StartupBase
{
    private const string filename = "fav-rolls.json";

    public override IConfigurator ConfigureCommands(IConfigurator config) =>
        ConfigureBaseCommands(config)
            .ConfigureFavoritesCommands();

    public override void ConfigureServices(ITypeRegistrar registrar) =>
        registrar.WithLifetimes().RegisterSingleton<IDice, Dice>()
                                 .RegisterSingleton<IDieRoller, RandomDieRoller>()
                                 .Services.AddLowDb<FavoriteRollsDocument>(filename);

    private static IConfigurator ConfigureBaseCommands(IConfigurator config)
    {
        config.CaseSensitivity(CaseSensitivity.None)
              .SetApplicationName("dice-cli")
              .SetApplicationVersion("1.0")
              .ValidateExamples();

        config.AddCommand<InteractiveCommand>("start")
              .WithDescription("Starts an interactive prompt for this sample CLI.")
              .WithExample(["start"]);

        config.AddCommand<RollCommand>("roll")
              .WithAlias("r")
              .WithDescription("Rolls the dice described by the notation string.")
              .WithExample(["roll", "1d20+5"]);

        config.AddCommand<NotationCommand>("notation")
              .WithAlias("note")
              .WithAlias("n")
              .WithDescription("Display details about how dice notations are defined and written.")
              .WithExample(["notation"]);

        return config;
    }
}
