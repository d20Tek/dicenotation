using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests.DiceTerms;

[TestClass]
public class FudgeDiceTermTests
{
    private const string _expectedTermType = "FudgeDiceTerm.dF";
    private readonly IDieRoller _roller = new RandomDieRoller();

    [TestMethod]
    public void FudgeDiceTerm_ConstructorTest()
    {
        // arrange

        // act
        IExpressionTerm term = new FudgeDiceTerm(3);

        // assert
        term.AssertInstanceOf<FudgeDiceTerm>();
    }

    [TestMethod]
    public void FudgeDiceTerm_CalculateResultsTest()
    {
        // arrange
        var term = new FudgeDiceTerm(1);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_roller);

        // assert
        results.AssertInRange(1, _expectedTermType, -1, 1);
    }

    [TestMethod]
    public void FudgeDiceTerm_CalculateResultsMultipleDiceTest()
    {
        // arrange
        var term = new FudgeDiceTerm(3);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_roller);

        // assert
        results.AssertInRange(3, _expectedTermType, -1, 1);
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsChooseDiceTest()
    {
        // arrange
        var term = new FudgeDiceTerm(5, 3);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_roller);

        // assert
        results.AssertWithChoose(5, _expectedTermType, -1, 1, 3);
    }

    [TestMethod]
    public void FudgeDiceTerm_CalculateResultsNullDieRollerTest()
    {
        // arrange
        var term = new FudgeDiceTerm(1);

        // act - assert
        Assert.ThrowsExactly<ArgumentNullException>([ExcludeFromCodeCoverage] () => term.CalculateResults(null));
    }

    [TestMethod]
    public void FudgeDiceTerm_ToStringTest()
    {
        // arrange
        var term = new FudgeDiceTerm(2);

        // act
        string result = term.ToString();

        // assert
        Assert.AreEqual("2f", result);
    }

    [TestMethod]
    public void FudgeDiceTerm_ToStringChooseTest()
    {
        // arrange
        var term = new FudgeDiceTerm(5, 3);

        // act
        string result = term.ToString();

        // assert
        Assert.AreEqual("5fk3", result);
    }
}
