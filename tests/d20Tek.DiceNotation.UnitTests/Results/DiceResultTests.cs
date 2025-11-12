using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests.Results;

[TestClass]
public partial class DiceResultTests
{
    private const string _expectedRollerType = "ConstantDieRoller";
    private const string _randomRollerType = "RandomDieRoller";
    private const string _diceTermType = "DiceTerm";
    private readonly MockDiceConfiguration config = new();

    [TestMethod]
    public void ValidateProperties()
    {
        // arrange
        var termList = CreateSimpleTerms(3);

        // act
        DiceResult result = new()
        {
            DiceExpression = "d6",
            DieRollerUsed = _expectedRollerType,
            Results = termList,
            Value = 3,
        };

        // assert
        AssertDiceResult(result, termList, _expectedRollerType, "d6", 3);
    }

    [TestMethod]
    public void GetRollsDisplayText_WithNullList()
    {
        // arrange

        // act
        DiceResult result = new()
        {
            DiceExpression = "d6",
            DieRollerUsed = _expectedRollerType,
            Results = null,
            Value = 3,
        };

        // assert
        AssertDiceResult(result, null, _expectedRollerType, "d6", 3, string.Empty);
    }

    [TestMethod]
    public void Constructor_WithValueSpecified()
    {
        // arrange
        var termList = CreateSimpleTerms(5);

        // act
        var result = new DiceResult("d6", 5, termList, _randomRollerType, config);

        // assert
        AssertDiceResult(result, termList, _randomRollerType, "d6", 5);
    }

    [TestMethod]
    public void Constructor_WithValueCalculated()
    {
        // arrange
        var termList = CreateSimpleTerms(5);

        // act
        var result = new DiceResult("d6", termList, _randomRollerType, config);

        // assert
        AssertDiceResult(result, termList, _randomRollerType, "d6", 5);
    }

    [TestMethod]
    public void Constructor_WithFudgeDice()
    {
        // arrange
        var termList = CreateSimpleTerms(1);

        // act
        var result = new DiceResult("1f", 1, termList, "FudgeDieRoller", config);

        // assert
        AssertDiceResult(result, termList, "FudgeDieRoller", "1f", 1);
    }

    [TestMethod]
    public void Constructor_WithNonCalculatedResult()
    {
        // arrange
        List<TermResult> termList =
        [
            new() { Scalar = 1, Type = _diceTermType, Value = 5, AppliesToResultCalculation = true },
            new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = false },
        ];

        // act
        var result = new DiceResult("2d6", termList, _randomRollerType, config);

        // assert
        AssertDiceResult(result, termList, _randomRollerType, "2d6", 5, "5, 3*");
    }

    [TestMethod]
    public void Constructor_WithErrorCondition()
    {
        // arrange
        var termList = CreateSimpleTerms(5);

        // act
        var result = new DiceResult("Unexpected error.", "1d20+5");

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Unexpected error.", result.Error);
        Assert.IsTrue(result.HasError);
        Assert.AreEqual(-404, result.Value);
        Assert.AreEqual("1d20+5", result.DiceExpression);
        Assert.IsEmpty(result.DieRollerUsed);
        Assert.IsEmpty(result.Results);
    }


    private static List<TermResult> CreateSimpleTerms(int value) =>
        [ new() { Scalar = 1, Type = _diceTermType, Value = value, AppliesToResultCalculation = true } ];


    private static void AssertDiceResult(
        DiceResult result,
        List<TermResult> expectedTerms,
        string rollerType,
        string expression,
        int expectedValue,
        string rollText = null)
    {
        rollText ??= $"{expectedValue}";
        Assert.AreEqual(expression, result.DiceExpression);
        Assert.AreEqual(rollerType, result.DieRollerUsed);
        CollectionAssert.AreEqual(expectedTerms, result.Results?.ToList());
        Assert.AreEqual(expectedValue, result.Value);
        Assert.AreEqual(rollText, result.RollsDisplayText);
        Assert.IsNull(result.Error);
        Assert.IsFalse(result.HasError);
    }
}
