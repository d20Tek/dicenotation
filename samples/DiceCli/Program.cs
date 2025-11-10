global using D20Tek.Spectre.Console.Extensions;
global using D20Tek.Spectre.Console.Extensions.Commands;
global using Spectre.Console;
global using Spectre.Console.Cli;

using DiceCli;

return await new CommandAppBuilder().WithDIContainer()
                                    .WithStartup<Startup>()
                                    .WithDefaultCommand<InteractiveCommand>()
                                    .Build()
                                    .RunAsync(args);
