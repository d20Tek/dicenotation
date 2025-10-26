using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests.Results;

[TestClass]
public class TermResultListConverterTests
{
    private const string _expectedRollerType = "ConstantDieRoller";
    private const string _diceTermType = "DiceTerm";
    private const string _locale = "en-us";
    private readonly TermResultListConverter _conv = new();

    [TestMethod]
    public void TermResultListConverter_ConstructorTest()
    {
        // arrange

        // act
        TermResultListConverter conv = new();

        // assert
        Assert.IsNotNull(conv);
        Assert.IsInstanceOfType<TermResultListConverter>(conv);
    }

    [TestMethod]
    public void TermResultListConverter_ConvertTextTest()
    {
        // arrange
        DiceResult diceResult = new()
        {
            DiceExpression = "d6",
            DieRollerUsed = _expectedRollerType,
            Results =
            [
                new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = true },
            ],
            Value = 3,
        };

        // act
        var result = _conv.Convert(diceResult.Results, typeof(string), null, _locale) as string;

        // assert
        Assert.AreEqual("3", result);
    }

    [TestMethod]
    public void TermResultListConverter_ConvertChooseTextTest()
    {
        // arrange
        DiceResult diceResult = new()
        {
            DiceExpression = "6d6k3",
            DieRollerUsed = _expectedRollerType,
            Results =
            [
                new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = false },
                new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = false },
                new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = false },
            ],
            Value = 9,
        };

        // act
        var result = _conv.Convert(diceResult.Results, typeof(string), null, _locale) as string;

        // assert
        Assert.AreEqual("3, 3, 3, 3*, 3*, 3*", result);
    }

    [TestMethod]
    public void TermResultListConverter_ConvertComplexTextTest()
    {
        // arrange
        DiceResult diceResult = new()
        {
            DiceExpression = "4d6k3+d8+5",
            DieRollerUsed = _expectedRollerType,
            Results =
            [
                new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = true },
                new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = false },
                new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = true },
            ],
            Value = 17,
        };

        // act
        var result = _conv.Convert(diceResult.Results, typeof(string), null, _locale) as string;

        // assert
        Assert.AreEqual("3, 3, 3, 3*, 3", result);
    }

    [TestMethod]
    public void TermResultListConverter_ConvertEmptyResultListTest()
    {
        // arrange
        IReadOnlyList<TermResult> list = [];

        // act
        var result = _conv.Convert(list, typeof(string), null, _locale) as string;

        // assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void TermResultListConverter_ConvertErrorTargetTypeTest()
    {
        // arrange
        DiceResult diceResult = new()
        {
            DiceExpression = "d20",
            DieRollerUsed = _expectedRollerType,
            Results =
            [
                new() { Scalar = 1, Type = _diceTermType, Value = 3, AppliesToResultCalculation = true },
            ],
            Value = 3,
        };

        // act - assert
        Assert.ThrowsExactly<ArgumentException>(
            [ExcludeFromCodeCoverage] () => _conv.Convert(diceResult.Results, typeof(int), null, _locale));
    }

    [TestMethod]
    public void TermResultListConverter_ConvertErrorValueNullTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _conv.Convert(null, typeof(string), null, _locale));
    }

    [TestMethod]
    public void TermResultListConverter_ConvertErrorValueTypeTest()
    {
        // arrange
        var value = "testString";

        // act - assert
        Assert.ThrowsExactly<ArgumentException>(
            [ExcludeFromCodeCoverage] () => _conv.Convert(value, typeof(string), null, _locale));
    }

    [TestMethod]
    public void TermResultListConverter_ConvertBackTest()
    {
        // arrange
        var value = "testString";

        // act - assert
        Assert.ThrowsExactly<NotSupportedException>(
            [ExcludeFromCodeCoverage] () => _conv.ConvertBack(value, typeof(string), null, _locale));
    }
}
