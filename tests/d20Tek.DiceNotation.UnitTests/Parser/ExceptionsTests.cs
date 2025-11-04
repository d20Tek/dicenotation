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
        Assert.AreEqual("test error @ L:2,C:3", ex.Message);
        Assert.AreEqual(new(1, 2, 3), ex.Position);
    }

    [TestMethod]
    public void EvalException_Creation()
    {
        // arrange

        // act
        var ex = new EvalException("test error");

        // assert
        Assert.IsNotNull(ex);
        Assert.AreEqual("test error", ex.Message);
    }
}
