using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceParserTokenizeTests
{
    private readonly DiceParser _parser = new();

    [TestMethod]
    public void DiceParser_TokenizeSimpleTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("d20");

        // assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<List<string>>(result);
        Assert.HasCount(3, result);
        Assert.AreEqual("1", result[0]);
        Assert.AreEqual("d", result[1]);
        Assert.AreEqual("20", result[2]);
    }

    [TestMethod]
    public void DiceParser_TokenizeSimpleConstantTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("42");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(1, result);
        Assert.AreEqual("42", result[0]);
    }

    [TestMethod]
    public void DiceParser_TokenizeSimpleModifierTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("1d20+10");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(5, result);
        Assert.AreEqual("1", result[0]);
        Assert.AreEqual("d", result[1]);
        Assert.AreEqual("20", result[2]);
        Assert.AreEqual("+", result[3]);
        Assert.AreEqual("10", result[4]);
    }

    [TestMethod]
    public void DiceParser_TokenizeModifierFirstTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("2+d6");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(5, result);
        Assert.AreEqual("2", result[0]);
        Assert.AreEqual("+", result[1]);
        Assert.AreEqual("1", result[2]);
        Assert.AreEqual("d", result[3]);
        Assert.AreEqual("6", result[4]);
    }

    [TestMethod]
    public void DiceParser_TokenizeNegativeModifierTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("1d20-1");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(5, result);
        Assert.AreEqual("1", result[0]);
        Assert.AreEqual("d", result[1]);
        Assert.AreEqual("20", result[2]);
        Assert.AreEqual("-", result[3]);
        Assert.AreEqual("1", result[4]);
    }

    [TestMethod]
    public void DiceParser_TokenizeMultiplyTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("2d6x10");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(5, result);
        Assert.AreEqual("2", result[0]);
        Assert.AreEqual("d", result[1]);
        Assert.AreEqual("6", result[2]);
        Assert.AreEqual("x", result[3]);
        Assert.AreEqual("10", result[4]);
    }

    [TestMethod]
    public void DiceParser_TokenizeMultiplyFirstTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("10x2d6");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(5, result);
        Assert.AreEqual("10", result[0]);
        Assert.AreEqual("x", result[1]);
        Assert.AreEqual("2", result[2]);
        Assert.AreEqual("d", result[3]);
        Assert.AreEqual("6", result[4]);
    }

    [TestMethod]
    public void DiceParser_TokenizeDivideTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("2d6/10");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(5, result);
        Assert.AreEqual("2", result[0]);
        Assert.AreEqual("d", result[1]);
        Assert.AreEqual("6", result[2]);
        Assert.AreEqual("/", result[3]);
        Assert.AreEqual("10", result[4]);
    }

    [TestMethod]
    public void DiceParser_TokenizeDivideFirstTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("10/2d6");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(5, result);
        Assert.AreEqual("10", result[0]);
        Assert.AreEqual("/", result[1]);
        Assert.AreEqual("2", result[2]);
        Assert.AreEqual("d", result[3]);
        Assert.AreEqual("6", result[4]);
    }

    [TestMethod]
    public void DiceParser_TokenizeChooseTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("4d6k3");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(5, result);
        Assert.AreEqual("4", result[0]);
        Assert.AreEqual("d", result[1]);
        Assert.AreEqual("6", result[2]);
        Assert.AreEqual("k", result[3]);
        Assert.AreEqual("3", result[4]);
    }

    [TestMethod]
    public void DiceParser_TokenizeDropLowestTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("4d6p2");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(5, result);
        Assert.AreEqual("4", result[0]);
        Assert.AreEqual("d", result[1]);
        Assert.AreEqual("6", result[2]);
        Assert.AreEqual("p", result[3]);
        Assert.AreEqual("2", result[4]);
    }

    [TestMethod]
    public void DiceParser_TokenizeKeepLowestTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("4d6l2");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(5, result);
        Assert.AreEqual("4", result[0]);
        Assert.AreEqual("d", result[1]);
        Assert.AreEqual("6", result[2]);
        Assert.AreEqual("l", result[3]);
        Assert.AreEqual("2", result[4]);
    }

    [TestMethod]
    public void DiceParser_TokenizeExplodingTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("4d6!5");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(5, result);
        Assert.AreEqual("4", result[0]);
        Assert.AreEqual("d", result[1]);
        Assert.AreEqual("6", result[2]);
        Assert.AreEqual("!", result[3]);
        Assert.AreEqual("5", result[4]);
    }

    [TestMethod]
    public void DiceParser_TokenizeExplodingNoValueTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("4d6!");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(4, result);
        Assert.AreEqual("4", result[0]);
        Assert.AreEqual("d", result[1]);
        Assert.AreEqual("6", result[2]);
        Assert.AreEqual("!", result[3]);
    }

    [TestMethod]
    public void DiceParser_TokenizeChainedDiceExpressionTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("4d6k3 + d8 + 3");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(11, result);
        Assert.AreEqual("4", result[0]);
        Assert.AreEqual("d", result[1]);
        Assert.AreEqual("6", result[2]);
        Assert.AreEqual("k", result[3]);
        Assert.AreEqual("3", result[4]);
        Assert.AreEqual("+", result[5]);
        Assert.AreEqual("1", result[6]);
        Assert.AreEqual("d", result[7]);
        Assert.AreEqual("8", result[8]);
        Assert.AreEqual("+", result[9]);
        Assert.AreEqual("3", result[10]);
    }

    [TestMethod]
    public void DiceParser_TokenizeUnaryOperatorTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("-1 + 4d6k3");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(7, result);
        Assert.AreEqual("-1", result[0]);
        Assert.AreEqual("+", result[1]);
        Assert.AreEqual("4", result[2]);
        Assert.AreEqual("d", result[3]);
        Assert.AreEqual("6", result[4]);
        Assert.AreEqual("k", result[5]);
        Assert.AreEqual("3", result[6]);
    }

    [TestMethod]
    public void DiceParser_TokenizeUnaryOperatorPositiveTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("+1 + 4d6k3");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(7, result);
        Assert.AreEqual("+1", result[0]);
        Assert.AreEqual("+", result[1]);
        Assert.AreEqual("4", result[2]);
        Assert.AreEqual("d", result[3]);
        Assert.AreEqual("6", result[4]);
        Assert.AreEqual("k", result[5]);
        Assert.AreEqual("3", result[6]);
    }

    [TestMethod]
    public void DiceParser_TokenizeParenthesesSimpleTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("4d6k3 + (d8 - 2)");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(13, result);
        Assert.AreEqual("4", result[0]);
        Assert.AreEqual("d", result[1]);
        Assert.AreEqual("6", result[2]);
        Assert.AreEqual("k", result[3]);
        Assert.AreEqual("3", result[4]);
        Assert.AreEqual("+", result[5]);
        Assert.AreEqual("(", result[6]);
        Assert.AreEqual("1", result[7]);
        Assert.AreEqual("d", result[8]);
        Assert.AreEqual("8", result[9]);
        Assert.AreEqual("-", result[10]);
        Assert.AreEqual("2", result[11]);
        Assert.AreEqual(")", result[12]);
    }

    [TestMethod]
    public void DiceParser_TokenizeParenthesesMuliplyTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("(2d6 + 1)x2");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(9, result);
        Assert.AreEqual("(", result[0]);
        Assert.AreEqual("2", result[1]);
        Assert.AreEqual("d", result[2]);
        Assert.AreEqual("6", result[3]);
        Assert.AreEqual("+", result[4]);
        Assert.AreEqual("1", result[5]);
        Assert.AreEqual(")", result[6]);
        Assert.AreEqual("x", result[7]);
        Assert.AreEqual("2", result[8]);
    }

    [TestMethod]
    public void DiceParser_TokenizeDefaultMuliplyTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("(2d6 + 1)2");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(9, result);
        Assert.AreEqual("(", result[0]);
        Assert.AreEqual("2", result[1]);
        Assert.AreEqual("d", result[2]);
        Assert.AreEqual("6", result[3]);
        Assert.AreEqual("+", result[4]);
        Assert.AreEqual("1", result[5]);
        Assert.AreEqual(")", result[6]);
        Assert.AreEqual("x", result[7]);
        Assert.AreEqual("2", result[8]);
    }

    [TestMethod]
    public void DiceParser_TokenizeDefaultMuliplyFirstTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("2(2d6 + 1)");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(9, result);
        Assert.AreEqual("2", result[0]);
        Assert.AreEqual("x", result[1]);
        Assert.AreEqual("(", result[2]);
        Assert.AreEqual("2", result[3]);
        Assert.AreEqual("d", result[4]);
        Assert.AreEqual("6", result[5]);
        Assert.AreEqual("+", result[6]);
        Assert.AreEqual("1", result[7]);
        Assert.AreEqual(")", result[8]);
    }

    [TestMethod]
    public void DiceParser_TokenizeParenthesesNestedTest()
    {
        // arrange

        // act
        List<string> result = _parser.Tokenize("4d6k3 - (2 + 3) + (2(d8 - 2))");

        // assert
        Assert.IsNotNull(result);
        Assert.HasCount(23, result);
        Assert.AreEqual("4", result[0]);
        Assert.AreEqual("d", result[1]);
        Assert.AreEqual("6", result[2]);
        Assert.AreEqual("k", result[3]);
        Assert.AreEqual("3", result[4]);
        Assert.AreEqual("-", result[5]);
        Assert.AreEqual("(", result[6]);
        Assert.AreEqual("2", result[7]);
        Assert.AreEqual("+", result[8]);
        Assert.AreEqual("3", result[9]);
        Assert.AreEqual(")", result[10]);
        Assert.AreEqual("+", result[11]);
        Assert.AreEqual("(", result[12]);
        Assert.AreEqual("2", result[13]);
        Assert.AreEqual("x", result[14]);
        Assert.AreEqual("(", result[15]);
        Assert.AreEqual("1", result[16]);
        Assert.AreEqual("d", result[17]);
        Assert.AreEqual("8", result[18]);
        Assert.AreEqual("-", result[19]);
        Assert.AreEqual("2", result[20]);
        Assert.AreEqual(")", result[21]);
        Assert.AreEqual(")", result[22]);
    }
}
