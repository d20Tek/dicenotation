using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceParserFudgeTests
{
    private const string _expectedRollerType = "FudgeDiceTerm.dF";
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
        result.AssertDiceChoose("f", _expectedRollerType, 1, 1);
    }

    [TestMethod]
    public void DiceParser_ParseDiceFudgeTest()
    {
        // arrange

        // act
        var result = _parser.Parse("3f", _config, _roller);

        // assert
        result.AssertDiceChoose("3f", _expectedRollerType, 3, 3);
    }

    [TestMethod]
    public void DiceParser_ParseDiceFudgeModifierTest()
    {
        // arrange

        // act
        var result = _parser.Parse("3f+1", _config, _roller);

        // assert
        result.AssertDiceChoose("3f+1", _expectedRollerType, 3, 3, 1);
    }

    [TestMethod]
    public void DiceParser_ParseDiceFudgeKeepTest()
    {
        // arrange

        // act
        var result = _parser.Parse("6fk4", _config, _roller);

        // assert
        result.AssertDiceChoose("6fk4", _expectedRollerType, 6, 4);
    }

    [TestMethod]
    public void DiceParser_ParseDiceFudgeDropTest()
    {
        // arrange

        // act
        var result = _parser.Parse("6fp3", _config, _roller);

        // assert
        result.AssertDiceChoose("6fp3", _expectedRollerType, 6, 3);
    }
}
