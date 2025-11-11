namespace DiceCli.Commands;

internal class InteractiveCommand(ICommandApp app, IAnsiConsole console) : InteractiveCommandBase(app, console)
{
    protected override void ShowWelcomeMessage(IAnsiConsole console)
    {
        console.Write(new FigletText("Dice-Cli").Centered().Color(Color.Green));
        console.WriteMessages(
            "[green]Running interactive mode.[/] Type 'exit' to quit or '--help' to see available commands.\r\n",
            "Welcome to DiceCli! Use this tool to roll random dice of any type.");
    }

    protected override string GetAppPromptPrefix() => "d20>";

    protected override void ShowExitMessage(IAnsiConsole console) => console.WriteLine("Bye!");
}
