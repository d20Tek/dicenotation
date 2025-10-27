using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceParserExplodingTests
{
    private readonly DiceConfiguration _config = new();
    private readonly IDieRoller _testRoller = new ConstantDieRoller(2);
    private readonly DiceParser _parser = new();

    [TestMethod]
    public void DiceParser_ParseDiceWithExplodingTest()
    {
        // arrange

        // act
        var result = _parser.Parse("6d6!6", _config, _testRoller);

        // assert
        result.AssertResult("6d6!6", 6, 12);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithExplodingRandomTest()
    {
        // arrange

        // act
        var result = _parser.Parse("10d6!6", _config, new RandomDieRoller());

        // assert
        AssertExplodingDiceResult(result, "10d6!6", 10, 6);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithExplodingNoValueTest()
    {
        // arrange

        // act
        var result = _parser.Parse("6d6!", _config, _testRoller);

        // assert
        result.AssertResult("6d6!", 6, 12);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithExplodingNoValueRandomTest()
    {
        // arrange

        // act
        var result = _parser.Parse("10d6!", _config, new RandomDieRoller());

        // assert
        AssertExplodingDiceResult(result, "10d6!", 10, 6);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithExplodingNoValueModifierTest()
    {
        // arrange

        // act
        var result = _parser.Parse("6d6!+2", _config, _testRoller);

        // assert
        result.AssertResult("6d6!+2", 6, 14);
    }

    private static void AssertExplodingDiceResult(DiceResult result, string expression, int count, int sides)
    {
        Assert.IsNotNull(result);
        Assert.AreEqual(expression, result.DiceExpression);
        int sum = 0;
        foreach (TermResult r in result.Results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(1, sides, r.Value);
            Assert.AreEqual("DiceTerm.d6", r.Type);
            sum += r.Value;
            if (r.Value >= sides) count++;
        }
        Assert.HasCount(count, result.Results);
        Assert.AreEqual(sum, result.Value);
    }
}
