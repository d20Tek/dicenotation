# API Reference

This reference documents the public types and members of the d20Tek.DiceNotation library, organized
by namespace. The descriptions are derived from the library's XML documentation comments.

## Table of Contents

- [Namespace: d20Tek.DiceNotation](#namespace-d20tekdicenotation)
  - [IDice](#idice)
  - [Dice](#dice)
  - [IDiceConfiguration](#idiceconfiguration)
  - [DiceConfiguration](#diceconfiguration)
  - [DiceExpression](#diceexpression)
  - [IDieRoller](#idieroller)
- [Namespace: d20Tek.DiceNotation.DiceTerms](#namespace-d20tekdicenotationdiceterms)
  - [IExpressionTerm](#iexpressionterm)
- [Namespace: d20Tek.DiceNotation.DieRoller](#namespace-d20tekdicenotationdieroller)
  - [IDieRoller Implementations](#idieroller-implementations)
  - [RandomDieRollerBase](#randomdierollerbase)
  - [RandomDieRoller](#randomdieroller)
  - [ConstantDieRoller](#constantdieroller)
  - [CryptoDieRoller](#cryptodieroller)
  - [MathNetDieRoller](#mathnetdieroller)
  - [IAllowRollTrackerEntry](#iallowrolltrackerentry)
  - [IDieRollTracker](#idierolltracker)
  - [DieRollTracker](#dierolltracker)
  - [IAggregateRollTracker](#iaggregaterolltracker)
  - [AggregateRollTracker](#aggregaterolltracker)
  - [DieTrackingData](#dietrackingdata)
  - [AggregateDieTrackingData](#aggregatedietrackingdata)
- [Namespace: d20Tek.DiceNotation.Parser](#namespace-d20tekdicenotationparser)
  - [Evaluator](#evaluator)
  - [ParseException](#parseexception)
  - [EvalException](#evalexception)
- [Namespace: d20Tek.DiceNotation.Results](#namespace-d20tekdicenotationresults)
  - [DiceResult](#diceresult)
  - [TermResult](#termresult)
  - [DiceResultConverter](#diceresultconverter)
  - [TermResultListConverter](#termresultlistconverter)

---

## Namespace: d20Tek.DiceNotation

### IDice

Defines the primary entry point for parsing and rolling dice expressions.

**Properties**

| Member | Description |
| --- | --- |
| `IDiceConfiguration Configuration { get; }` | Gets the configuration that controls default dice behavior for this instance. |

**Methods**

`DiceResult Roll(DiceExpression expression, IDieRoller? dieRoller = null)`

Rolls the specified dice expression and returns the calculated result.

- `expression` - The dice expression to evaluate.
- `dieRoller` - The die roller used to generate random values, or `null` to use the configured
  default die roller.
- Returns a `DiceResult` containing the outcome of the roll.

`DiceResult Roll(string notation, IDieRoller? dieRoller = null)`

Parses and rolls the specified dice notation and returns the calculated result.

- `notation` - The dice notation to parse and evaluate (for example, "3d6+1").
- `dieRoller` - The die roller used to generate random values, or `null` to use the configured
  default die roller.
- Returns a `DiceResult` containing the outcome of the roll.

### Dice

Provides the default implementation for parsing and rolling dice expressions. Implements `IDice`.

**Constructors**

| Member | Description |
| --- | --- |
| `Dice(IDiceConfiguration diceConfig)` | Initializes a new instance of the `Dice` class with the specified configuration. |
| `Dice()` | Initializes a new instance of the `Dice` class with the default configuration. |

**Properties**

| Member | Description |
| --- | --- |
| `IDiceConfiguration Configuration { get; }` | Gets the configuration that controls default dice behavior for this instance. |

**Methods**

| Member | Description |
| --- | --- |
| `DiceResult Roll(string notation, IDieRoller? dieRoller = null)` | Parses and rolls the specified dice notation and returns the calculated result. |
| `DiceResult Roll(DiceExpression expression, IDieRoller? dieRoller = null)` | Rolls the specified dice expression and returns the calculated result. |

### IDiceConfiguration

Defines the configurable settings that control default dice behavior.

**Properties**

| Member | Description |
| --- | --- |
| `int DefaultDieSides { get; }` | Gets the default number of sides used when a die notation omits an explicit side count. |
| `bool HasBoundedResult { get; }` | Gets a value indicating whether roll results are bounded by a minimum value. |
| `int BoundedResultMinimum { get; }` | Gets the minimum value that a bounded result can produce. |
| `IDieRoller DefaultDieRoller { get; }` | Gets the die roller used by default when no explicit roller is supplied. |

**Methods**

| Member | Description |
| --- | --- |
| `void SetDefaultDieSides(int dieSides)` | Sets the default number of sides used when a die notation omits an explicit side count. |
| `void SetHasBoundedResult(bool hasBoundedResult)` | Sets whether roll results are bounded by a minimum value. |
| `void SetBoundedMinimumResult(int boundedMinResult)` | Sets the minimum value that a bounded result can produce. |
| `void SetDefaultDieRoller(IDieRoller dieRoller)` | Sets the die roller used by default when no explicit roller is supplied. |

### DiceConfiguration

Provides the default implementation of dice configuration settings. Implements
`IDiceConfiguration`.

**Constructors**

| Member | Description |
| --- | --- |
| `DiceConfiguration(int dieSides, int boundedMinResult, bool hasBoundedResult, IDieRoller? dieRoller = null)` | Initializes a new instance with the specified settings. The die roller defaults to a new `RandomDieRoller` when `null`. |
| `DiceConfiguration()` | Initializes a new instance with default settings. |

**Properties**

| Member | Description |
| --- | --- |
| `bool HasBoundedResult { get; }` | Gets a value indicating whether roll results are bounded by a minimum value. |
| `int BoundedResultMinimum { get; }` | Gets the minimum value that a bounded result can produce. |
| `int DefaultDieSides { get; }` | Gets the default number of die sides. |
| `IDieRoller DefaultDieRoller { get; }` | Gets the die roller used by default when no explicit roller is supplied. |

**Methods**

| Member | Description |
| --- | --- |
| `void SetDefaultDieSides(int dieSides)` | Sets the default number of die sides. |
| `void SetHasBoundedResult(bool hasBoundedResult)` | Sets whether roll results are bounded by a minimum value. |
| `void SetBoundedMinimumResult(int boundedMinResult)` | Sets the minimum bounded result value. |
| `void SetDefaultDieRoller(IDieRoller dieRoller)` | Sets the default die roller to use. |

### DiceExpression

Represents a composable dice expression built from constant, dice, and fudge dice terms.

**Static Methods**

`static DiceExpression Create()`

Creates a new, empty `DiceExpression` instance.

**Methods**

`DiceExpression AddConstant(int constant)`

Adds a constant term to the expression. A value of zero is ignored. Returns the current instance for
chaining.

`DiceExpression AddDice(int sides, int numberDice = 1, double scalar = 1, int? choose = null, int? exploding = null)`

Adds a dice term to the expression.

- `sides` - The number of sides on each die.
- `numberDice` - The number of dice to roll.
- `scalar` - A multiplier applied to the term result.
- `choose` - The number of highest dice to keep, or `null` to keep all dice.
- `exploding` - The threshold at which dice explode, or `null` for no exploding dice.
- Returns the current instance for chaining.

`DiceExpression AddFudgeDice(int numberDice = 1, int? choose = null)`

Adds a Fudge/FATE dice term to the expression.

- `numberDice` - The number of Fudge dice to roll.
- `choose` - The number of highest dice to keep, or `null` to keep all dice.
- Returns the current instance for chaining.

`DiceExpression Clear()`

Removes all terms from the expression. Returns the current instance for chaining.

`DiceExpression Concat(DiceExpression otherDice)`

Appends the terms of another expression to this expression. Returns the current instance for
chaining.

`IReadOnlyList<IExpressionTerm> Evaluate()`

Returns the read-only list of terms that make up this expression.

`override string ToString()`

Returns the dice notation representation of this expression.

### IDieRoller

Defines a mechanism for rolling a single die and producing a numeric result.

**Methods**

`int Roll(int sides, int? factor = null)`

Rolls a single die with the specified number of sides.

- `sides` - The number of sides on the die.
- `factor` - An optional factor applied to the roll, or `null` when no factor is used.
- Returns the value produced by the die roll.

---

## Namespace: d20Tek.DiceNotation.DiceTerms

### IExpressionTerm

Represents a single term within a dice expression that can be evaluated to produce results.

**Methods**

`IReadOnlyList<TermResult> CalculateResults(IDieRoller dieRoller)`

Calculates the results of this term using the specified die roller.

- `dieRoller` - The die roller used to generate random values.
- Returns the list of `TermResult` values produced by this term.

---

## Namespace: d20Tek.DiceNotation.DieRoller

### IDieRoller Implementations

The die roller implementations in this namespace produce the random values used when rolling. Most
derive from `RandomDieRollerBase`, which handles roll factoring and tracking; each derived roller
supplies its own random number generation strategy.

### RandomDieRollerBase

Provides a base implementation for random die rollers, handling roll factoring and tracking.
Abstract. Implements `IDieRoller`.

**Constructors**

| Member | Description |
| --- | --- |
| `RandomDieRollerBase(IAllowRollTrackerEntry? tracker = null)` | Initializes the base roller with an optional tracker used to record roll entries. |

**Methods**

| Member | Description |
| --- | --- |
| `int Roll(int sides, int? factor = null)` | Rolls a die, applies the optional factor, records the roll with the tracker if one is present, and returns the result. |
| `protected abstract int GetNextRandom(int sides)` | Generates the next random value for a die with the specified number of sides. |

### RandomDieRoller

A die roller that uses the standard `System.Random` generator to produce values. Inherits
`RandomDieRollerBase`.

**Constructors**

| Member | Description |
| --- | --- |
| `RandomDieRoller(Random random, IAllowRollTrackerEntry? tracker)` | Initializes a new instance using the specified random generator and optional tracker. |
| `RandomDieRoller(IAllowRollTrackerEntry? tracker = null)` | Initializes a new instance using a shared default generator and optional tracker. |

**Methods**

| Member | Description |
| --- | --- |
| `protected override int GetNextRandom(int sides)` | Generates the next random value for a die with the specified number of sides. |

### ConstantDieRoller

A die roller that always returns a fixed, constant value. Useful for testing. Implements
`IDieRoller`.

**Constructors**

| Member | Description |
| --- | --- |
| `ConstantDieRoller(int rollValue = 1)` | Initializes a new instance with the constant value returned by every roll. |

**Methods**

| Member | Description |
| --- | --- |
| `int Roll(int sides, int? factor = null)` | Returns the configured constant value. |

### CryptoDieRoller

A die roller that uses a cryptographically secure random number generator. Inherits
`RandomDieRollerBase`.

**Constructors**

| Member | Description |
| --- | --- |
| `CryptoDieRoller(IAllowRollTrackerEntry? tracker = null)` | Initializes a new instance with an optional tracker used to record roll entries. |

**Methods**

| Member | Description |
| --- | --- |
| `protected override int GetNextRandom(int sides)` | Generates the next cryptographically strong random value for a die with the specified number of sides. |

### MathNetDieRoller

A die roller that uses a Math.NET Numerics random source to generate values. Inherits
`RandomDieRollerBase`.

**Constructors**

| Member | Description |
| --- | --- |
| `MathNetDieRoller(RandomSource source, IAllowRollTrackerEntry? tracker = null)` | Initializes a new instance using the specified Math.NET random source and optional tracker. |
| `MathNetDieRoller(IAllowRollTrackerEntry? tracker = null)` | Initializes a new instance using a Mersenne Twister source and optional tracker. |

**Methods**

| Member | Description |
| --- | --- |
| `protected override int GetNextRandom(int sides)` | Generates the next random value for a die with the specified number of sides. |

### IAllowRollTrackerEntry

Defines the ability to record individual die roll entries with a tracker.

**Methods**

`void AddDieRoll(int dieSides, int result, Type dieRoller)`

Records a single die roll with the tracker.

- `dieSides` - The number of sides on the die that was rolled.
- `result` - The value produced by the roll.
- `dieRoller` - The type of die roller that produced the roll.

### IDieRollTracker

Defines a tracker that records individual die rolls and provides statistical views asynchronously.
Inherits `IAllowRollTrackerEntry`.

**Properties**

| Member | Description |
| --- | --- |
| `int TrackerDataLimit { get; set; }` | Gets or sets the maximum number of roll entries retained by the tracker. |

**Methods**

| Member | Description |
| --- | --- |
| `Task<IList<DieTrackingData>> GetTrackingDataAsync(string? dieType = null, string? dieSides = null)` | Asynchronously gets the tracked die roll data, optionally filtered by die type and sides. |
| `Task<IList<AggregateDieTrackingData>> GetFrequencyDataViewAsync()` | Asynchronously gets a view of aggregated die roll frequency data. |
| `void Clear()` | Removes all tracked roll data. |
| `Task<string> ToJsonAsync()` | Asynchronously serializes the tracked roll data to a JSON string. |
| `Task LoadFromJsonAsync(string jsonText)` | Asynchronously loads tracked roll data from a JSON string. |

### DieRollTracker

Provides a tracker that records individual die rolls and produces statistical views asynchronously.
Implements `IDieRollTracker`.

**Properties**

| Member | Description |
| --- | --- |
| `int TrackerDataLimit { get; set; }` | Gets or sets the maximum number of roll entries retained by the tracker. |

**Methods**

| Member | Description |
| --- | --- |
| `void AddDieRoll(int dieSides, int result, Type dieRoller)` | Records a single die roll with the tracker. |
| `Task<IList<DieTrackingData>> GetTrackingDataAsync(string? dieType = null, string? dieSides = null)` | Asynchronously gets the tracked die roll data, optionally filtered by die type and sides. |
| `void Clear()` | Removes all tracked roll data. |
| `Task<string> ToJsonAsync()` | Asynchronously serializes the tracked roll data to a JSON string. |
| `Task LoadFromJsonAsync(string jsonText)` | Asynchronously loads tracked roll data from a JSON string. |
| `Task<IList<AggregateDieTrackingData>> GetFrequencyDataViewAsync()` | Asynchronously gets a view of aggregated die roll frequency data. |

### IAggregateRollTracker

Defines an in-memory tracker that aggregates die roll frequency data. Inherits
`IAllowRollTrackerEntry`.

**Methods**

| Member | Description |
| --- | --- |
| `IList<AggregateDieTrackingData> GetFrequencyDataView()` | Gets a view of aggregated die roll frequency data. |
| `void Clear()` | Removes all tracked roll data. |
| `string ToJson()` | Serializes the tracked roll data to a JSON string. |
| `void LoadFromJson(string jsonText)` | Loads tracked roll data from a JSON string. |

### AggregateRollTracker

Provides an in-memory tracker that aggregates die roll frequency data synchronously. Implements
`IAggregateRollTracker`.

**Methods**

| Member | Description |
| --- | --- |
| `void AddDieRoll(int dieSides, int result, Type dieRoller)` | Records a single die roll with the tracker. |
| `void Clear()` | Removes all tracked roll data. |
| `IList<AggregateDieTrackingData> GetFrequencyDataView()` | Gets a view of aggregated die roll frequency data. |
| `void LoadFromJson(string jsonText)` | Loads tracked roll data from a JSON string. |
| `string ToJson()` | Serializes the tracked roll data to a JSON string. |

### DieTrackingData

Represents a single tracked die roll entry.

**Properties**

| Member | Description |
| --- | --- |
| `Guid Id { get; set; }` | Gets or sets the unique identifier for this roll entry. |
| `string RollerType { get; set; }` | Gets or sets the name of the die roller type that produced the roll. |
| `string DieSides { get; set; }` | Gets or sets the number of die sides, represented as a string. |
| `int Result { get; set; }` | Gets or sets the value produced by the roll. |
| `DateTime Timpstamp { get; set; }` | Gets or sets the timestamp at which the roll was recorded. |

**Static Methods**

`static DieTrackingData Create(string rollerName, int dieSides, int result)`

Creates a new `DieTrackingData` entry with a generated identifier and current timestamp.

### AggregateDieTrackingData

Represents aggregated frequency data for die rolls grouped by roller type, die sides, and result.

**Properties**

| Member | Description |
| --- | --- |
| `string RollerType { get; set; }` | Gets or sets the name of the die roller type that produced the tracked rolls. |
| `string DieSides { get; set; }` | Gets or sets the number of die sides, represented as a string. |
| `int Result { get; set; }` | Gets or sets the roll result value that this data aggregates. |
| `int Count { get; set; }` | Gets or sets the number of times the result was rolled. |
| `float Percentage { get; set; }` | Gets or sets the percentage of rolls that produced this result within its group. |

**Static Methods**

`static AggregateDieTrackingData Create(string rollerType, int dieSides, int result)`

Creates a new `AggregateDieTrackingData` entry with zeroed counts.

---

## Namespace: d20Tek.DiceNotation.Parser

### Evaluator

Parses dice notation and evaluates the resulting expression tree into a dice result.

**Methods**

`DiceResult Evaluate(string notation, IDieRoller roller, IDiceConfiguration config)`

Parses and evaluates the specified dice notation into a `DiceResult`.

- `notation` - The dice notation to parse and evaluate.
- `roller` - The die roller used to generate random values.
- `config` - The configuration that controls default dice behavior.
- Returns a `DiceResult` containing the outcome of the evaluation.

### ParseException

The exception thrown when dice notation cannot be parsed. Sealed. Inherits `Exception`.

**Properties**

| Member | Description |
| --- | --- |
| `string Position { get; }` | Gets the position within the notation where the parse error occurred. |

### EvalException

The exception thrown when a parsed dice expression cannot be evaluated. Sealed. Inherits
`Exception`.

**Constructors**

| Member | Description |
| --- | --- |
| `EvalException(string message)` | Initializes a new instance with the message that describes the evaluation error. |

---

## Namespace: d20Tek.DiceNotation.Results

### DiceResult

Represents the outcome of rolling a dice expression, including the total value and individual term
results.

**Constructors**

| Member | Description |
| --- | --- |
| `DiceResult(string expression, List<TermResult> results, string rollerUsed, IDiceConfiguration config)` | Initializes a new instance, calculating the value from the results. |
| `DiceResult(string expression, int value, List<TermResult> results, string roller, IDiceConfiguration config)` | Initializes a new instance with a precalculated value. |
| `DiceResult(string error, string expression)` | Initializes a new instance that represents an error. |
| `DiceResult()` | Initializes a new empty instance. |

**Properties**

| Member | Description |
| --- | --- |
| `string DiceExpression { get; set; }` | Gets or sets the dice expression that produced this result. |
| `string DieRollerUsed { get; set; }` | Gets or sets the name of the die roller type that was used. |
| `IReadOnlyList<TermResult> Results { get; set; }` | Gets or sets the individual term results that make up this dice result. |
| `int Value { get; set; }` | Gets or sets the total calculated value of the roll. |
| `string? Error { get; set; }` | Gets or sets the error message describing why the roll failed, if any. |
| `bool HasError { get; }` | Gets a value indicating whether this result represents an error. |
| `string RollsDisplayText { get; }` | Gets the display text describing the individual dice rolls. |

### TermResult

Represents the result of evaluating a single term within a dice expression.

**Constructors**

| Member | Description |
| --- | --- |
| `TermResult(double scalar, int value, string type)` | Initializes a new instance with the specified scalar, value, and term type. |
| `TermResult()` | Initializes a new empty instance. |

**Properties**

| Member | Description |
| --- | --- |
| `double Scalar { get; set; }` | Gets or sets the scalar multiplier applied to this term's value. |
| `int Value { get; set; }` | Gets or sets the value produced by the term. |
| `string Type { get; set; }` | Gets or sets the type identifier of the term that produced this result. |
| `bool AppliesToResultCalculation { get; set; }` | Gets or sets a value indicating whether this result contributes to the overall calculation. |

### DiceResultConverter

Converts a `DiceResult` to its display string representation.

**Methods**

`virtual object Convert(object value, Type targetType, object parameter, string language)`

Converts a `DiceResult` to a formatted display string.

- `value` - The `DiceResult` to convert.
- `targetType` - The target type, which must be `string`.
- `parameter` - An optional converter parameter (unused).
- `language` - The language or locale used for formatting.
- Returns the formatted display string.

`virtual object ConvertBack(object value, Type targetType, object parameter, string language)`

Not supported. Converting back from a display string is not implemented and always throws
`NotSupportedException`.

### TermResultListConverter

Converts a list of `TermResult` values to a display string of dice rolls.

**Methods**

`virtual object Convert(object value, Type targetType, object parameter, string language)`

Converts a list of `TermResult` values to a comma-separated display string.

- `value` - The list of term results to convert.
- `targetType` - The target type, which must be `string`.
- `parameter` - An optional converter parameter (unused).
- `language` - The language or locale used for formatting.
- Returns the formatted display string of dice rolls.

`virtual object ConvertBack(object value, Type targetType, object parameter, string language)`

Not supported. Converting back from a display string is not implemented and always throws
`NotSupportedException`.
