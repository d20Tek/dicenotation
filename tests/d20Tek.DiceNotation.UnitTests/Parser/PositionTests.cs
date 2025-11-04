using d20Tek.DiceNotation.Parser;

namespace d20Tek.DiceNotation.UnitTests.Parser;

[TestClass]
public class PositionTests
{
    [TestMethod]
    public void Position_WithChanges_ReturnsUpdatedRecord()
    {
        // arrange
        var pos = new Position();

        // act
        var result = pos with { Index = 3, Line = 1, Column = 4 };

        // assert
        Assert.AreEqual(3, result.Index);
        Assert.AreEqual(1, result.Line);
        Assert.AreEqual(4, result.Column);
    }

    [TestMethod]
    public void Position_ToString()
    {
        // arrange
        var pos = new Position(5, 1, 6);

        // act
        var result = pos.ToString();

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("L:1,C:6", result);
    }
}
