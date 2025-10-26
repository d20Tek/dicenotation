using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceParserFudgeTests
{
    private readonly DiceConfiguration _config = new();
    private readonly IDieRoller _roller = new RandomDieRoller();
    private readonly DiceParser _parser = new();

    [TestMethod]
    public void DiceParser_ParseSingleFudgeDieTest()
    {
        // arrange

        // act
        var result = _parser.Parse("f", _config, _roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("f", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        AssertHelpers.IsWithinRangeInclusive(-1, 1, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceFudgeTest()
    {
        // arrange

        // act
        var result = _parser.Parse("3f", _config, _roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("3f", result.DiceExpression);
        Assert.HasCount(3, result.Results);
        int sum = 0;
        foreach (TermResult r in result.Results)
        {
            Assert.AreEqual("FudgeDiceTerm.dF", r.Type);
            sum += r.Value;
        }
        Assert.AreEqual(sum, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceFudgeModifierTest()
    {
        // arrange

        // act
        var result = _parser.Parse("3f+1", _config, _roller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("3f+1", result.DiceExpression);
        Assert.HasCount(3, result.Results);
        int sum = 0;
        foreach (TermResult r in result.Results)
        {
            Assert.AreEqual("FudgeDiceTerm.dF", r.Type);
            sum += r.Value;
        }
        Assert.AreEqual(sum + 1, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceFudgeKeepTest()
    {
        // arrange

        // act
        var result = _parser.Parse("6fk4", _config, _roller);

        // assert
        Assert.IsNotNull(result);
        AssertHelpers.AssertDiceChoose(result, "6fk4", "FudgeDiceTerm.dF", 6, 4);
    }

    [TestMethod]
    public void DiceParser_ParseDiceFudgeDropTest()
    {
        // arrange

        // act
        var result = _parser.Parse("6fp3", _config, _roller);

        // assert
        Assert.IsNotNull(result);
        AssertHelpers.AssertDiceChoose(result, "6fp3", "FudgeDiceTerm.dF", 6, 3);
    }
}
