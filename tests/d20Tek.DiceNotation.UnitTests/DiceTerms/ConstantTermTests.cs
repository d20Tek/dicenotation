using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests.DiceTerms;

[TestClass]
public class ConstantTermTests
{
    private const string _expectedTermType = "ConstantTerm";
    private static readonly IDieRoller _dieRoller = new RandomDieRoller();

    [TestMethod]
    public void ConstantTerm_ConstructorTest()
    {
        // arrange

        // act
        IExpressionTerm term = new ConstantTerm(16);

        // assert
        term.AssertInstanceOf<ConstantTerm>();
    }

    [TestMethod]
    public void ConstantTerm_CalculateResultsTest()
    {
        // arrange
        var constantValue = 4;
        var term = new ConstantTerm(constantValue);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_dieRoller);

        // assert
        results.AssertConstant(1, _expectedTermType, constantValue);
    }

    [TestMethod]
    public void ConstantTerm_CalculateResultsNullDieRollerTest()
    {
        // arrange
        var constantValue = 8;
        var term = new ConstantTerm(constantValue);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(null);

        // assert
        results.AssertConstant(1, _expectedTermType, constantValue);
    }

    [TestMethod]
    public void ConstantTerm_ToStringTest()
    {
        // arrange
        var term = new ConstantTerm(3);

        // act
        string result = term.ToString();

        // assert
        Assert.AreEqual("3", result);
    }
}
