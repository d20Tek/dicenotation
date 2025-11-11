using d20Tek.DiceNotation;
using d20Tek.DiceNotation.DieRoller;
using D20Tek.Spectre.Console.Extensions.Injection;
using DiceCli.Commands;

namespace DiceCli;

internal sealed class Startup : StartupBase
{
    public override IConfigurator ConfigureCommands(IConfigurator config)
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

    public override void ConfigureServices(ITypeRegistrar registrar) =>
        registrar.WithLifetimes().RegisterSingleton<IDice, Dice>()
                                 .RegisterSingleton<IDieRoller, RandomDieRoller>();
}
