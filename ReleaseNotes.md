# Release Notes 
These are the release notes for our various updates. You can see how the library has evolved and the latest capabilities. 

### Current Release (5.1.9):
- Added the `d20Tek.DiceNotation.Blazor` package with the `VisualDiceRoller` component, which animates a completed `DiceResult` in a two-dimensional tray drawn on an HTML canvas. The component never rolls dice, so the faces that come to rest always match the result the application already calculated.
- Added `AddDiceNotationBlazor()`, which registers the component's services along with the core dice notation services, so a consuming application only makes one call.
- Added `DiceRollerOptions` and the `DiceRollerOptionsBuilder` fluent registration API for configuring the tray size, theme, presentation values, physics settings, and reduced-motion preference.
- Added the `DiceTheme` presets `Default`, `DarkFantasy`, and `Parchment`, along with `DiceVisualSettings` and `DiceSimulationSettings` for tuning appearance and physics.
- Added `DiceRollSummary` and `VisualDie` result models, which preserve the authoritative total from the notation library along with each die's role, sides, and minimum face value.
- Added a reduced-motion option and an accessible canvas label so the settled outcome is available to assistive technology.
- Added shape-aware dice rendering: each die is now drawn and simulated with a silhouette matching its type (d2 round coin, d4 triangle, d6 square, d8 diamond, d10 and d100 kite, d12 pentagon, d20 hexagon) instead of every die appearing as a square. Shapes are resolved in C# and sent to the renderer as vertex data, and the physics bodies are built from the same outlines so collision always matches what is drawn.
- Added full documentation for the Blazor package: `docs/introduction-blazor.md`, `docs/getting-started-blazor.md`, and `docs/api-reference-blazor.md`, including supported targets and known limitations.
- Published the Blazor component library as the `d20tek-dicenotation-blazor` NuGet package (version 1.0.0) targeting .NET 9 and .NET 10, with symbol packages and a package README.
- Added the `BlazorDice` sample application, which demonstrates the main scenarios on its home page: keep-highest rolls, single-die rolls with modifiers, mixed die types, coin flips, percentile dice, theme switching, reduced motion, replay, and invalid-notation error handling.
- Fixed an ambiguity in the `VisualDiceRoller` rendering where a 6 and a 9 looked identical on tumbling dice. Dice with nine or more sides now draw a disambiguating dot after those numerals, matching the convention used on physical dice.

### Release (5.1.8):
- Updated package references to newest versions.
- Fixed DiceCli commands due to breaking changes in Spectre.Console.Cli Command and AsyncCommand base classes.
- Added RollMany() overloads to IDice for rolling a dice expression multiple times and returning a list of DiceResults.
- Enabled trimming and Native AOT support for the DiceNotation library by replacing dynamic dispatch in Evaluator with a type-safe switch expression, adding a source-generated DieTrackingJsonContext for JSON serialization, and annotating UseRollTracker<TTracker, TService>() with DynamicallyAccessedMembers for generic service registration.
- Added DiceNotation Benchmarks project to measure performance of the DiceNotation library and compare different die rollers.
- Fixed 3 high memory allocation issues in the Lexer, Parser, and Evaluator for hot paths when rolling the same notation repeatedly. Save 78% of allocations when rolling the same notation repeatedly (with improvement to speed too).

### Release (5.1.7):
- Added `AddDiceNotation()` service-collection extensions to register `IDice`, configuration, die roller, and optional roll tracker cleanly through dependency injection.
- Added the `DiceNotationOptionsBuilder` fluent API for configuring die sides, bounded results, the die roller, and roll tracking.
- Added a verbose/structured breakdown to dice results: each rolled die now records its role (kept, dropped, exploded, or rerolled) through the new `DieRollRole` flags enum and the `TermResult.Roles` property.
- Added `DiceResult` breakdown helpers (`KeptResults`, `DroppedResults`, `ExplodedResults`, `RerolledResults`) and a `VerboseDisplayText` property that annotates each die roll with its role.
- Added seeded/replayable rolling: `RandomDieRoller` and `MathNetDieRoller` now accept a seed, implement the new `IReplayableDieRoller` interface, and expose a `ReplayToken` that captures the algorithm and seed for reproducing a roll sequence.
- Added the `DieRollerFactory` with `CreateSeeded` and `CreateFromReplayToken` methods, plus `UseSeededDieRoller`, `UseSeededMathNetDieRoller`, and `UseReplayDieRoller` options on `DiceNotationOptionsBuilder`. Cryptographic rollers are intentionally excluded because their generators are not seedable.
- Added xml documentation to the public API for the DiceNotation library.
- Added full documentation set for the DiceNotation library.
- Migrated the solution to use central package and central build management.
- Updated package references to newest versions.

### Release (5.1.6):
- Updated nuget-release script to use Nuget Trusted Publishing.
- Updated package references to newest versions.

### Release (5.1.1):
* Updated library package to support .NET 9 & 10.
* Updated to latest package dependency versions.
* Updated build scripts to require .NET 10.

### Release (5.0.1):
* Updated to latest package dependency versions.
* Implemented a brand new lexer/parser/evaluator to parse dice notation strings and roll equivalent dice.
* Simplified the library and IDice interface (making unnecessary items internal and removing unused code).
* Refactored this package code to more modern C# constructs.
* Upgraded to MsTest 4 and fixed up all breaking unit tests.
* Added the DiceCli as a sample application for using this package.

### Release (4.0):
Upgrade libraries to .NET 8.
* Minor breaking changes to upgrade library design to .NET standards and use latest language features.
* Added simplified DiceRoll method using a DiceRequest.

### Release (3.4):
Upgrade libraries to .NET 7.

### Release (3.3):
Upgrade libraries to .NET 6.

### Release (3.2.2):
Upgrade libraries to .NET 5.

### Release (3.0.2):
Rebuild code and packages as .NET Core 3.0 libraries.

### Release 1.0.5:
* Add dice notation for keep lowest by using L token in the notation. Changed drop lowest notation to use P token.

### Release 1.0.4: 
* Added tracking service to keep track of die rolls to provide statistical data. 
* Added abstract RandomDieRollerBase class for shared code between all random rollers, so those classes can just implement minimally the number generation. 
* Added two new die rollers: SecureRandomDieRoller and MathNetDieRoller. 

### Release 1.0.3: 
* Added value converters for DiceResult and TermResultList to help with default display of these classes as text. 
* Two bug fixes: 
    - Added IDice.Clear method to allow dice to be reused with new expressions (without needing to recreating instance). 
    - Dice.Parse now throws an exception if null or empty string is used. 

### Release 1.0.2: 
* Added support percentile (d%) notation.  
* Added support for dropping lowest N dice (similar to keeping highest N dice).  
* Added support for Fudge/FATE dice notation.  
* Added support for exploding or pentrating dice rolls.  
* Added support for default dice sides (3d =>3d6), and ability to set default number of sides as dice config. 

### Release 1.0.1: 
* Added general purpose parser to handle more math and dice expressions. 
* Added support for grouping using ( ). Now you can parse dice expressions like - (2+1)d4 - (4-2). 
* Test validation for new expressions and grouping. 
 
### Release 1.0.0: 
* Initial release with dice notation support, parsing of notation strings, and evaluating die rolls and math operations into a result. 
