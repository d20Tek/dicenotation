# Sample: DiceRoller - Web MVC

This sample code shows you how to incorporate the OnePlat.DiceNotation library into your own ASP.NET MVC web application. The sample was created using the VS WebApp - MVC 5 project teamplate, and then installing the OnePlat.DiceNotation.1.0.4 NuGet package.

The source code can be found in this repository ([/Samples/DiceRoller.Mvc](../Samples/DiceRoller.Mvc)), and it is pretty easy to digest. The bulk of the interesting code is in RollController.cs file.

#### 1 - Dice Expression Input

This code takes input from the UI for a dice expression string, and then roll the results.

``` csharp
    /// <summary>
    /// Creates a die roll from dice expression.
    /// GET: Roll/Create
    /// </summary>
    /// <returns>View</returns>
    public ActionResult Create()
    {
        this.vmDiceRoller.RollResults = this.Session[CurrentDiceResultsListSessionKey] as IList<DiceResult> ?? new List<DiceResult>();

        return this.View(this.vmDiceRoller);
    }

    /// <summary>
    /// Creates a die roll from dice expression.
    /// POST: Roll/Create
    /// </summary>
    /// <param name="collection">Forms collection.</param>
    /// <returns>View</returns>
    [HttpPost]
    public ActionResult Create(FormCollection collection)
    {
        try
        {
            this.vmDiceRoller.RollResults = this.Session[CurrentDiceResultsListSessionKey] as IList<DiceResult> ?? new List<DiceResult>();
            this.vmDiceRoller.RollCommand(collection[DiceExpressionFormKey]);
            this.Session[CurrentDiceResultsListSessionKey] = this.vmDiceRoller.RollResults;

            return this.View(this.vmDiceRoller);
        }
        catch
        {
            return this.View(this.vmDiceRoller);
        }
    }
```

This uses the following view model class to provide properties to the roll page and handle operations like the Roll Command.

``` csharp
  public class DiceRollerViewModel
  {
      #region Members
      private IDice dice;
      private IDieRoller dieRoller;
      #endregion

      /// <summary>
      /// Initializes a new instance of the <see cref="DiceRollerViewModel"/> class.
      /// </summary>
      public DiceRollerViewModel()
      {
          AppServices services = AppServices.Instance;
          this.dice = services.DiceService;
          string rollerType = services.AppSettingsService.CurrentDieRollerType;
          this.dieRoller = services.DieRollerFactory.GetDieRoller(rollerType, services.DiceFrequencyTracker);
      }

      #region Properties

      /// <summary>
      /// Gets or sets the dice expression text.
      /// </summary>
      [Display(Name = "Dice Expression")]
      [Required]
      public string DiceExpression { get; set; }

      /// <summary>
      /// Gets or sets the list of roll results.
      /// </summary>
      public IList<DiceResult> RollResults { get; set; } = new List<DiceResult>();
      #endregion

      #region Commands

      /// <summary>
      /// Command method to roll the dice and save the results.
      /// </summary>
      /// <param name="expression">String expression to roll.</param>
      public void RollCommand(string expression)
      {
          this.DiceExpression = expression;
          DiceResult result = this.dice.Roll(this.DiceExpression, this.dieRoller);
          this.RollResults.Insert(0, result);
      }
      #endregion
  }
```

#### 2 - Dice Results Display
Also, notice in the Create.cshtml file the definition of the results list. It uses an HTML helper method to format the ResultsList into the appropriate string representation. This helper uses the value converters are part of OnePlat.DiceNotation v1.0.4. You can use them if you'd like to share the formatting. If not, you're free to display the results however you would like.

```csharp
  /// <summary>
  /// Html helper class to format term results lists.
  /// </summary>
  public static class TermResultListHelper
  {
      private static TermResultListConverter converter = new TermResultListConverter();

      /// <summary>
      /// Formats the specified list of term results into the corresponding text format.
      /// </summary>
      /// <param name="helper">Html helper</param>
      /// <param name="results">Term results to convert</param>
      /// <returns>Text representation for the list.</returns>
      public static string DisplayResultsFor(this HtmlHelper helper, IReadOnlyList<TermResult> results)
      {
          string result = (string)converter.Convert(results, typeof(string), null, "en-us");
          return result;
      }
  }
```
