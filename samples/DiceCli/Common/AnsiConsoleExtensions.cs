namespace DiceCli.Common;

internal static class AnsiConsoleExtensions
{
    extension(IAnsiConsole console)
    {
        public T PromptIfNull<T>(T? value, string promptLabel) where T : notnull => 
            value ?? console.Prompt(new TextPrompt<T>(promptLabel));
    }
}