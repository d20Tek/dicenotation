using d20Tek.DiceNotation.Parser;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class ExceptionsTests
{
    [TestMethod]
    public void ParseException_Creation()
    {
        // arrange

        // act
        var ex = new ParseException("test error", new(1, 2, 3));

        // assert
        Assert.IsNotNull(ex);
        Assert.AreEqual("Parse error: test error @(Line: 2, Column: 3)", ex.Message);
        Assert.AreEqual(new Position(1, 2, 3).ToString(), ex.Position);
    }

    [TestMethod]
    public void ThrowIfFalse_WithFalseCondition_ThrowsException()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ParseException>([ExcludeFromCodeCoverage] () =>
            ParseException.ThrowIfFalse(false, "test error", new()));
    }

    [TestMethod]
    public void ThrowIfFalse_WithTrueCondition_Succeeds()
    {
        // arrange

        // act - assert
        ParseException.ThrowIfFalse(true, "test error", new());
    }

    [TestMethod]
    public void EvalException_Creation()
    {
        // arrange

        // act
        var ex = new EvalException("test error");

        // assert
        Assert.IsNotNull(ex);
        Assert.AreEqual("Evaluation error: test error", ex.Message);
    }
}
