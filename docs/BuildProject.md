# Building Code

### Building project code

To build this project, you will need to clone this repository locally.

This was built using Visual Studio 2022 (version 17.8.4). It will work with other versions of Visual Studio, but wasn't tested with them.

* Open the d20Tek.DiceNotation.sln solution file from the repository root in Visual Studio.
* Be sure to update "Restore NuGet Packages" on the solution.
* Then Rebuild the full solution.

Along with the library build, this also build a command-line tool for testing. You can run the D20Tek.DiceNotation.CommandLine project to launch a command-line applet that lets you play with the functionality.

From a command project in the output folder, run the following in the command prompt:

```
dotnet dicenotation.dll d20+3
dotnet dicenotation.dll 4d6k3 -v
```
