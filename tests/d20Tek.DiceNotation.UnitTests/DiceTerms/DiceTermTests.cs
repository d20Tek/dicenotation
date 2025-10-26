using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests.DiceTerms;

[TestClass]
public class DiceTermTests
{
    private readonly IDieRoller _dieRoller = new RandomDieRoller();
    private readonly IDieRoller _constantRoller = new ConstantDieRoller();

    [TestMethod]
    public void DiceTerm_ConstructorTest()
    {
        // arrange

        // act
        IExpressionTerm term = new DiceTerm(1, 20);

        // assert
        term.AssertInstanceOf<DiceTerm>();
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsTest()
    {
        // arrange
        var term = new DiceTerm(1, 20);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_dieRoller);

        // assert
        results.AssertInRange(1, "DiceTerm.d20", 1, 20);
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsMultipleDiceTest()
    {
        // arrange
        var term = new DiceTerm(3, 6);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_constantRoller);

        // assert
        results.AssertInRange(3, "DiceTerm.d6", 1, 20);
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsChooseDiceTest()
    {
        // arrange
        var term = new DiceTerm(5, 6, choose: 3);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_constantRoller);

        // assert
        results.AssertWithChoose(5, "DiceTerm.d6", 1, 20, 3);
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsExplodingNoneDiceTest()
    {
        // arrange
        var term = new DiceTerm(5, 6, exploding: 6);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_constantRoller);

        // assert
        results.AssertConstant(5, "DiceTerm.d6", 1);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void DiceTerm_CalculateResultsExplodingRandomDiceTest()
    {
        // arrange
        var term = new DiceTerm(10, 6, exploding: 6);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(new RandomDieRoller());

        // assert
        results.AssertWithExploding(10, "DiceTerm.d6", 1, 6, 6);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void DiceTerm_CalculateResultsExplodingLowerThanMaxTest()
    {
        // arrange
        var term = new DiceTerm(10, 12, exploding: 9);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(new RandomDieRoller());

        // assert
        results.AssertWithExploding(10, "DiceTerm.d12", 1, 12, 9);
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsExplodingAndChooseTest()
    {
        // arrange
        var term = new DiceTerm(10, 12, choose: 8, exploding: 9);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(new RandomDieRoller());

        // assert
        results.AssertWithChoose(10, "DiceTerm.d12", 1, 12, 8);
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsMultiplierDiceTest()
    {
        // arrange
        var term = new DiceTerm(2, 8, 10);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_constantRoller);

        // validate results
        results.AssertWithScalar(2, "DiceTerm.d8", 1, 8, 10);
    }
}
