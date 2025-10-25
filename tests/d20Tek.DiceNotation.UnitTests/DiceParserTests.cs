using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;
using D20Tek.DiceNotation.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceParserTests
{
    private readonly DiceConfiguration _config = new();
    private readonly IDieRoller _testRoller = new ConstantDieRoller(2);
    private readonly DiceParser _parser = new();

    [TestMethod]
    public void DiceParser_ConstructorTest()
    {
        // arrange

        // act
        DiceParser parser = new();

        // assert
        Assert.IsNotNull(parser);
        Assert.IsInstanceOfType<DiceParser>(parser);
    }

    [TestMethod]
    public void DiceParser_ParseSimpleDiceTest()
    {
        // arrange

        // act
        var result = _parser.Parse("3d6", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("3d6", result.DiceExpression);
        Assert.HasCount(3, result.Results);
        Assert.AreEqual(6, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseSingleDieTest()
    {
        // arrange

        // act
        var result = _parser.Parse("d20", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d20", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(2, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithModifierTest()
    {
        // arrange

        // act
        var result = _parser.Parse("2d4+3", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("2d4+3", result.DiceExpression);
        Assert.HasCount(2, result.Results);
        Assert.AreEqual(7, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithNegativeModifierTest()
    {
        // arrange

        // act
        var result = _parser.Parse("d12-2", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d12-2", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithKeepTest()
    {
        // arrange

        // act
        var result = _parser.Parse("4d6k3", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        AssertHelpers.AssertDiceChoose(result, "4d6k3", "DiceTerm.d6", 4, 3);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithDropLowestTest()
    {
        // arrange

        // act
        var result = _parser.Parse("6d6p2", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        AssertHelpers.AssertDiceChoose(result, "6d6p2", "DiceTerm.d6", 6, 4);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithEquivalentKeepDropTest()
    {
        // arrange

        // act
        var result = _parser.Parse("4d6p1", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        AssertHelpers.AssertDiceChoose(result, "4d6p1", "DiceTerm.d6", 4, 3);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithKeepLowestTest()
    {
        // arrange

        // act
        var result = _parser.Parse("6d6l2", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        AssertHelpers.AssertDiceChoose(result, "6d6l2", "DiceTerm.d6", 6, 2);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithEquivalentKeepLowestTest()
    {
        // arrange

        // act
        var result = _parser.Parse("4d6l1", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        AssertHelpers.AssertDiceChoose(result, "4d6l1", "DiceTerm.d6", 4, 1);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithExplodingTest()
    {
        // arrange

        // act
        DiceResult result = _parser.Parse("6d6!6", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("6d6!6", result.DiceExpression);
        Assert.HasCount(6, result.Results);
        Assert.AreEqual(12, result.Value);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void DiceParser_ParseDiceWithExplodingRandomTest()
    {
        // arrange

        // act
        var result = _parser.Parse("10d6!6", this._config, new RandomDieRoller());

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("10d6!6", result.DiceExpression);
        int sum = 0, count = 10;
        foreach (TermResult r in result.Results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(1, 6, r.Value);
            Assert.AreEqual("DiceTerm.d6", r.Type);
            sum += r.Value;
            if (r.Value >= 6) count++;
        }
        Assert.HasCount(count, result.Results);
        Assert.AreEqual(sum, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithExplodingNoValueTest()
    {
        // arrange

        // act
        var result = _parser.Parse("6d6!", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("6d6!", result.DiceExpression);
        Assert.HasCount(6, result.Results);
        Assert.AreEqual(12, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithExplodingNoValueRandomTest()
    {
        // arrange

        // act
        var result = _parser.Parse("10d6!", _config, new RandomDieRoller());

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("10d6!", result.DiceExpression);
        int sum = 0, count = 10;
        foreach (TermResult r in result.Results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(1, 6, r.Value);
            Assert.AreEqual("DiceTerm.d6", r.Type);
            sum += r.Value;
            if (r.Value >= 6) count++;
        }
        Assert.HasCount(count, result.Results);
        Assert.AreEqual(sum, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithExplodingNoValueModifierTest()
    {
        // arrange

        // act
        var result = _parser.Parse("6d6!+2", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("6d6!+2", result.DiceExpression);
        Assert.HasCount(6, result.Results);
        Assert.AreEqual(14, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithWhitepaceTest()
    {
        // arrange

        // act
        var result = _parser.Parse(" 4  d6 k 3+  2    ", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        AssertHelpers.AssertDiceChoose(result, "4d6k3+2", "DiceTerm.d6", 4, 3, 2);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithChainedExpressionTest()
    {
        // arrange

        // act
        var result = _parser.Parse("4d6k3 + d8 + 2", _config, _testRoller);

        // validate results
        Assert.IsNotNull(result);
        AssertHelpers.AssertDiceChoose(result, "4d6k3+d8+2", "DiceTerm", 5, 4, 2);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithMultiplyAfterTest()
    {
        // arrange

        // act
        var result = _parser.Parse("2d8x10", _config, _testRoller);

        // validate results
        Assert.IsNotNull(result);
        Assert.AreEqual("2d8x10", result.DiceExpression);
        Assert.HasCount(2, result.Results);
        Assert.AreEqual(40, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithMultiplyBeforeTest()
    {
        // arrange

        // act
        var result = _parser.Parse("10*2d8", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("10*2d8", result.DiceExpression);
        Assert.HasCount(2, result.Results);
        Assert.AreEqual(40, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithDivideTest()
    {
        // arrange

        // act
        var result = _parser.Parse("3d10 / 2", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("3d10/2", result.DiceExpression);
        Assert.HasCount(3, result.Results);
        Assert.AreEqual(3, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithDivideBeforeTest()
    {
        // arrange

        // act
        var result = _parser.Parse("40 / 1d6", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("40/1d6", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(20, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceWithChainedOrderTest()
    {
        // arrange

        // act
        var result = _parser.Parse("2 + 4d6k3 + d8", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        AssertHelpers.AssertDiceChoose(result, "2+4d6k3+d8", "DiceTerm", 5, 4, 2);
    }

    [TestMethod]
    public void DiceParser_ParseConstantOnlyTest()
    {
        // arrange

        // act
        var result = _parser.Parse("42", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("42", result.DiceExpression);
        Assert.IsEmpty(result.Results);
        Assert.AreEqual(42, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseConstantOnlyAdditionTest()
    {
        // arrange

        // act
        var result = _parser.Parse("4 + 2", _config, _testRoller);

        // validate results
        Assert.IsNotNull(result);
        Assert.AreEqual("4+2", result.DiceExpression);
        Assert.IsEmpty(result.Results);
        Assert.AreEqual(6, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseConstantOnlyMultiplyTest()
    {
        // arrange

        // act
        var result = _parser.Parse("4x2", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("4x2", result.DiceExpression);
        Assert.IsEmpty(result.Results);
        Assert.AreEqual(8, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseConstantOnlyDivideTest()
    {
        // arrange

        // act
        var result = _parser.Parse("4/2", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("4/2", result.DiceExpression);
        Assert.IsEmpty(result.Results);
        Assert.AreEqual(2, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceSubractOrderTest()
    {
        // arrange

        // act
        var result = _parser.Parse("100 - 2d12", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("100-2d12", result.DiceExpression);
        Assert.HasCount(2, result.Results);
        Assert.AreEqual(96, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceNegativeConstantTest()
    {
        // arrange

        // act
        var result = _parser.Parse("-5 + 4d6", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("-5+4d6", result.DiceExpression);
        Assert.HasCount(4, result.Results);
        Assert.AreEqual(3, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceMultipleConstantsTest()
    {
        // arrange

        // act
        var result = _parser.Parse("6 + d20 - 3", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("6+d20-3", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(5, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceMultipleConstantsOrderTest()
    {
        // arrange

        // act
        var result = _parser.Parse("2+1d20+2+3x3-10", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("2+1d20+2+3x3-10", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(5, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDicePercentileTest()
    {
        // arrange

        // act
        var result = _parser.Parse("d%+5", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d100+5", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual(7, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseSingleDieNoSidesTest()
    {
        // arrange

        // act
        var result = _parser.Parse("d", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d", result.DiceExpression);
        Assert.HasCount(1, result.Results);
        Assert.AreEqual("DiceTerm.d6", result.Results[0].Type);
        Assert.AreEqual(2, result.Value);
    }

    [TestMethod]
    public void DiceParser_ParseDiceNoSidesOperatorTest()
    {
        // arrange

        // act
        var result = _parser.Parse("2d+3", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("2d+3", result.DiceExpression);
        Assert.HasCount(2, result.Results);
        Assert.AreEqual("DiceTerm.d6", result.Results[0].Type);
        Assert.AreEqual("DiceTerm.d6", result.Results[1].Type);
        Assert.AreEqual(7, result.Value);
    }

    [TestMethod]
    public void DiceParser_SettingCustomOperators()
    {
        // arrange
        DiceParser parser = new()
        {
            DefaultNumDice = "2",
            DefaultOperator = "x",
            GroupStartOperator = "[",
            GroupEndOperator = "]"
        };

        // act
        var result = parser.Parse("3d6", _config, _testRoller);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("3d6", result.DiceExpression);
        Assert.HasCount(3, result.Results);
        Assert.AreEqual(6, result.Value);
        Assert.AreEqual("2", parser.DefaultNumDice);
        Assert.AreEqual("x", parser.DefaultOperator);
        Assert.AreEqual("[", parser.GroupStartOperator);
        Assert.AreEqual("]", parser.GroupEndOperator);
    }
}
