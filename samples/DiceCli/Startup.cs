using d20Tek.DiceNotation;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;
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
              .WithDescription("Rolls the dice described by the notation string.")
              .WithExample(["roll", "1d20+5"]);

        //config.AddCommand<GetNetWorthCommand>("get-worth")
        //      .WithAlias("w")
        //      .WithDescription("Requests current input for net worth.")
        //      .WithExample(["get-worth"]);

        return config;
    }

    public override void ConfigureServices(ITypeRegistrar registrar) =>
        registrar.WithLifetimes().RegisterSingleton<IDice, Dice>()
                                 .RegisterSingleton<IDieRoller, RandomDieRoller>()
                                 .RegisterSingleton<TermResultListConverter, TermResultListConverter>();
}
