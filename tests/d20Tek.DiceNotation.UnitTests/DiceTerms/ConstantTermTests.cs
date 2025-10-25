using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace d20Tek.DiceNotation.UnitTests.DiceTerms;

[TestClass]
public class ConstantTermTests
{
    private static readonly IDieRoller _dieRoller = new RandomDieRoller();

    [TestMethod]
    public void ConstantTerm_ConstructorTest()
    {
        // arrange

        // act
        IExpressionTerm term = new ConstantTerm(16);

        // assert
        Assert.IsNotNull(term);
        Assert.IsInstanceOfType<IExpressionTerm>(term);
        Assert.IsInstanceOfType<ConstantTerm>(term);
    }

    [TestMethod]
    public void ConstantTerm_CalculateResultsTest()
    {
        // arrange
        var term = new ConstantTerm(4);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_dieRoller);

        // assert
        Assert.IsNotNull(results);
        Assert.HasCount(1, results);
        TermResult r = results[0];
        Assert.IsNotNull(r);
        Assert.AreEqual(1, r.Scalar);
        Assert.AreEqual(4, r.Value);
        Assert.AreEqual("ConstantTerm", r.Type);
    }

    [TestMethod]
    public void ConstantTerm_CalculateResultsNullDieRollerTest()
    {
        // arrange
        var term = new ConstantTerm(8);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(null);

        // assert
        Assert.IsNotNull(results);
        Assert.HasCount(1, results);
        TermResult r = results[0];
        Assert.IsNotNull(r);
        Assert.AreEqual(1, r.Scalar);
        Assert.AreEqual(8, r.Value);
        Assert.AreEqual("ConstantTerm", r.Type);
    }

    [TestMethod]
    public void ConstantTerm_ToStringTest()
    {
        // arrange
        var term = new ConstantTerm(3);

        // act
        string result = term.ToString();

        // assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.AreEqual("3", result);
    }
}
