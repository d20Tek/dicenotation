global using D20Tek.Functional;
global using D20Tek.LowDb;
global using D20Tek.Spectre.Console.Extensions;
global using D20Tek.Spectre.Console.Extensions.Commands;
global using D20Tek.Spectre.Console.Extensions.Controls;
global using DiceCli.Common;
global using Spectre.Console;
global using Spectre.Console.Cli;
global using System.ComponentModel;

using DiceCli;
using DiceCli.Commands;

return await new CommandAppBuilder().WithDIContainer()
                                    .WithStartup<Startup>()
                                    .WithDefaultCommand<InteractiveCommand>()
                                    .Build()
                                    .RunAsync(args);
