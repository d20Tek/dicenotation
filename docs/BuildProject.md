# Building Code

### Building project code

To build this project, you will need to clone this repository locally.

This was built using Visual Studio 2019 (version 16.8.1). It will work with other versions of Visual Studio, but wasn't tested with them.

* Open the d20Tek.DiceNotation.sln solution file from the repository root in Visual Studio.
* Be sure to update "Restore NuGet Packages" on the solution.
* Then Rebuild the full solution.

Along with the library build, this also build a command-line tool for testing. You can run the D20Tek.DiceNotation.CommandLine project to launch a command-line applet that lets you play with the functionality.

From a command project in the output folder, run the following in the command prompt:

```
dotnet dicenotation.dll d20+3
dotnet dicenotation.dll 4d6k3 -v
```

```
old content... needs updating...
### Building samples

To build the samples, you will need to:
* Navigate to the /Samples folder in the repository.
* Open the Samples.sln solution file in Visual Studio.
* Be sure to update "Restore NuGet Packages" on the solution.
* Then Rebuild the full solution.

To run the Win10 sample:
* Ensure that the DiceRoller.Win10 project is selected as the startup project.
* Then Debug-F5 (or Run - Ctrl+F5) on the project.
* The DiceRoller app should start up, give it a try (you can try some [example notations](docs/DiceNotationExamples.md)).

To run the ASP.NET MVC sample:
* Ensure that the DiceRoller.Mvc project is selected as the startup project.
* Then Debug-F5 (or Run - Ctrl+F5) on the project.
* The DiceRoller web app should start up in your default browser, give it a try (you can try some [example notations](docs/DiceNotationExamples.md)).
* From the main page, click the Dice Roller button to get to the input page.
```

Also, if you just want to try out the web application, you can find it online at [d20 Dice Roller](http://dicenotation-diceroller-mvc.azurewebsites.net/).
