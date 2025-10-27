namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceParserTokenizeTests
{
    private readonly DiceParser _parser = new();

    [TestMethod]
    [DataRow("d20", "1,d,20", DisplayName = "SimpleTest")]
    [DataRow("42", "42", DisplayName = "ConstantTest")]
    [DataRow("1d20+10", "1,d,20,+,10", DisplayName = "WithBonusTest")]
    [DataRow("2+d6", "2,+,1,d,6", DisplayName = "BonusFirstTest")]
    [DataRow("1d20-1", "1,d,20,-,1", DisplayName = "WithNegativeBonusTest")]
    [DataRow("2d6x10", "2,d,6,x,10", DisplayName = "MultiplyTest")]
    [DataRow("10x2d6", "10,x,2,d,6", DisplayName = "MultiplyTest")]
    [DataRow("2d6/10", "2,d,6,/,10", DisplayName = "DivideTest")]
    [DataRow("10/2d6", "10,/,2,d,6", DisplayName = "DivideFirstTest")]
    [DataRow("4d6k3", "4,d,6,k,3", DisplayName = "ChooseTest")]
    [DataRow("4d6p2", "4,d,6,p,2", DisplayName = "DropLowestTest")]
    [DataRow("4d6l2", "4,d,6,l,2", DisplayName = "KeepLowestTest")]
    [DataRow("4d6!5", "4,d,6,!,5", DisplayName = "ExplodingTest")]
    [DataRow("4d6!", "4,d,6,!", DisplayName = "ExplodingNoValueTest")]
    public void DiceParser_TokenizeSimpleTests(string expression, string tokens)
    {
        // arrange
        List<string> expected = [.. tokens.Split(',')];

        // act
        var result = _parser.Tokenize(expression);

        // assert
        AssertTokens(result, expected);
    }

    [TestMethod]
    [DataRow("4d6k3 + d8 + 3", "4,d,6,k,3,+,1,d,8,+,3", DisplayName = "ChainedDiceTest")]
    [DataRow("-1 + 4d6k3", "-1,+,4,d,6,k,3", DisplayName = "UnaryOperatorTest")]
    [DataRow("+1 + 4d6k3", "+1,+,4,d,6,k,3", DisplayName = "UnaryOperatorPositiveTest")]
    [DataRow("4d6k3 + (d8 - 2)", "4,d,6,k,3,+,(,1,d,8,-,2,)", DisplayName = "ParenthesesTest")]
    [DataRow("(2d6 + 1)x2", "(,2,d,6,+,1,),x,2", DisplayName = "ParenthesesMultiplyTest")]
    [DataRow("(2d6 + 1)2", "(,2,d,6,+,1,),x,2", DisplayName = "DefaultMultiplyTest")]
    [DataRow("2(2d6 + 1)", "2,x,(,2,d,6,+,1,)", DisplayName = "DefaultMultiplyFirstTest")]
    [DataRow("4d6k3 - (2 + 3) + (2(d8 - 2))", "4,d,6,k,3,-,(,2,+,3,),+,(,2,x,(,1,d,8,-,2,),)", DisplayName = "ParenthesesNestedTest")]
    public void DiceParser_TokenizeChainedTests(string expression, string tokens)
    {
        // arrange
        List<string> expected = [.. tokens.Split(',')];

        // act
        List<string> result = _parser.Tokenize(expression);

        // assert
        AssertTokens(result, expected);
    }

    private static void AssertTokens(List<string> result, List<string> expectedTokens)
    {
        Assert.IsNotNull(result);
        Assert.HasCount(expectedTokens.Count, result);
        CollectionAssert.AreEqual(expectedTokens, result);
    }
}
