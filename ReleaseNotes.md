# Release Notes 
These are the release notes for our various updates. You can see how the library has evolved and the latest capabilities. 

### Current release (5.0.1):
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
