# Introduction 
## d20Tek.DiceNotation
DiceNotation library written to provide dice notation parsing, evaluation, and rolling. This library is built on .NET 8, so you can incorporate it into any of your .NET Core and beyond projects.

Dice notation (also known as dice algebra, common dice notation, RPG dice notation, and several other titles) is a system to represent different combinations of dice in role-playing games using simple algebra-like notation such as 2d6+12.

The specification for the dice notation supported in the current version on the library is located [here](docs/DiceNotationSpecCurrent.md). There are also [examples of dice notation](docs/DiceNotationExamples.md) strings.

To build the source code, please read [Building Code](docs/BuildProject.md) page.

# Installation
This library is a NuGet package so it is easy to add to your project. To install these packages into your solution, you can use the Package Manager. In PM, please use the following commands:
```  
PM > Install-Package d20tek-dicenotation -Version 4.0
``` 

To install in the Visual Studio UI, go to the Tools menu > "Manage NuGet Packages". Then search for d20Tek.dicenotation and install it from there.

Read more about this release in our [Release Notes](ReleaseNotes.md).

# Usage
d20Tek.DiceNotation has a couple of different modes that it can be used in depending on how you want to build up the dice expression:

### Programmatically:
You can build up the dice to roll by coding the various parts that make up a dice expression. The expression can be build by chaining together operations (as a Fluent API style).

```csharp
IDice dice = new Dice();
// equivalent of dice expression: 4d6k3 + d8 + 5
dice.Dice(6, 4, choose: 3).Dice(8).Constant(5);
DiceResult result = dice.Roll(new RandomDieRoller());
Console.WriteLine("Roll result = " + result.Value);
```
   
### Dice Notation String:
You can also create the dice expression by parsing a string that follows the defined Dice Notation language. When you parse the text, we create a similar expression tree as the programmatic version that is then evaluated.

```csharp
IDice dice = new Dice();
DiceResult result = dice.Roll("d20+4", new RandomDieRoller());
Console.WriteLine("Roll result = " + result.Value);
```

### Dice Rollers:
Both of the usage options above use the RandomDieRoller, which uses the .NET Random class to produce the random dice rolls. There are additional die rollers:
* ConstantDieRoller - lets you create a roller that always returns the same value. This roller is great for testing features and expressions because the results will be consistent in your unit tests.
* CryptoDieRoller - uses Cryptography API to create a more random number generator.
* MathNetDieRoller - provides various strategies for random number generators to produce our die rolls.

Finally, the library defines a IDieRoller interface that you can use to build your own custom die rollers. If the random number generators in our libraries don't suffice, you can override it with your own rolling implementation.

### Samples:
For more detailed examples on how to use this DiceNotation library, please review the following samples:

* [Sample - DiceRoller Windows 10](docs/SampleWin10.md)
* [Sample - DiceRoller ASP.NET MVC](docs/SampleWebMvc.md)

# Feedback
If you use this library and have any feedback, bugs, or suggestions, please file them in the Bugs section of this repository.

I have plans for better notation support, so there will be updates coming for this project. Your feedback would be appreciated.
