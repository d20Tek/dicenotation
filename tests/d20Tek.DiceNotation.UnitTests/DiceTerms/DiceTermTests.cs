using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;
using D20Tek.DiceNotation.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
        Assert.IsNotNull(term);
        Assert.IsInstanceOfType<IExpressionTerm>(term);
        Assert.IsInstanceOfType<DiceTerm>(term);
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsTest()
    {
        // arrange
        var term = new DiceTerm(1, 20);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_dieRoller);

        // assert
        Assert.IsNotNull(results);
        Assert.HasCount(1, results);
        TermResult r = results[0];
        Assert.IsNotNull(r);
        Assert.AreEqual(1, r.Scalar);
        AssertHelpers.IsWithinRangeInclusive(1, 20, r.Value);
        Assert.AreEqual("DiceTerm.d20", r.Type);
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsMultipleDiceTest()
    {
        // arrange
        var term = new DiceTerm(3, 6);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_constantRoller);

        // assert
        Assert.IsNotNull(results);
        Assert.HasCount(3, results);
        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            Assert.AreEqual(1, r.Value);
            Assert.AreEqual("DiceTerm.d6", r.Type);
        }
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsChooseDiceTest()
    {
        // arrange
        var term = new DiceTerm(5, 6, choose: 3);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_constantRoller);

        // assert
        Assert.IsNotNull(results);
        Assert.HasCount(5, results);
        int included = 0;
        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            Assert.AreEqual(1, r.Value);
            Assert.AreEqual("DiceTerm.d6", r.Type);
            if (r.AppliesToResultCalculation) included++;
        }
        Assert.AreEqual(3, included);
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsExplodingNoneDiceTest()
    {
        // arrange
        var term = new DiceTerm(5, 6, exploding: 6);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_constantRoller);

        // assert
        Assert.IsNotNull(results);
        Assert.HasCount(5, results);
        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            Assert.AreEqual(1, r.Value);
            Assert.AreEqual("DiceTerm.d6", r.Type);
        }
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
        Assert.IsNotNull(results);
        int count = 10;
        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(1, 6, r.Value);
            if (r.Value >= 6) count++;
            Assert.AreEqual("DiceTerm.d6", r.Type);
        }
        Assert.HasCount(count, results);
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
        Assert.IsNotNull(results);
        int count = 10;
        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(1, 12, r.Value);
            if (r.Value >= 9) count++;
            Assert.AreEqual("DiceTerm.d12", r.Type);
        }
        Assert.HasCount(count, results);
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsExplodingAndChooseTest()
    {
        // arrange
        var term = new DiceTerm(10, 12, choose: 8, exploding: 9);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(new RandomDieRoller());

        // assert
        Assert.IsNotNull(results);
        int included = 0;
        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(1, 12, r.Value);
            Assert.AreEqual("DiceTerm.d12", r.Type);
            if (r.AppliesToResultCalculation) included++;
        }
        Assert.AreEqual(8, included);
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsErrorMaxRerollsTest()
    {
        // arrange
        var term = new DiceTerm(10, 12, exploding: 9);

        // act - assert
        Assert.ThrowsExactly<OverflowException>(
            [ExcludeFromCodeCoverage] () => term.CalculateResults(new ConstantDieRoller(10)));
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsMultiplierDiceTest()
    {
        // arrange
        var term = new DiceTerm(2, 8, 10);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_constantRoller);

        // validate results
        Assert.IsNotNull(results);
        Assert.HasCount(2, results);
        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(10, r.Scalar);
            Assert.AreEqual(1, r.Value);
            Assert.AreEqual("DiceTerm.d8", r.Type);
        }
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsNullDieRollerTest()
    {
        // arrange
        var term = new DiceTerm(1, 10);

        // act - assert
        Assert.ThrowsExactly<ArgumentNullException>([ExcludeFromCodeCoverage] () => term.CalculateResults(null));
    }

    [TestMethod]
    public void DiceTerm_ToStringTest()
    {
        // arrange
        var term = new DiceTerm(2, 10);

        // act
        string result = term.ToString();

        // assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.AreEqual("2d10", result);
    }

    [TestMethod]
    public void DiceTerm_ToStringChooseTest()
    {
        // arrange
        var term = new DiceTerm(5, 6, choose: 3);

        // act
        string result = term.ToString();

        // assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.AreEqual("5d6k3", result);
    }

    [TestMethod]
    public void DiceTerm_ToStringMultiplierTest()
    {
        // arrange
        var term = new DiceTerm(2, 8, 10);

        // act
        string result = term.ToString();

        // assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.AreEqual("2d8x10", result);
    }

    [TestMethod]
    public void DiceTerm_ToStringExplodingNoneDiceTest()
    {
        // arrange
        var term = new DiceTerm(5, 6, exploding: 6);

        // act
        string result = term.ToString();

        // assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.AreEqual("5d6!6", result);
    }

    [TestMethod]
    public void DiceTerm_ToStringExplodingLowerThanMaxTest()
    {
        // arrange
        var term = new DiceTerm(10, 12, exploding: 9);

        // act
        string result = term.ToString();

        // assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.AreEqual("10d12!9", result);
    }

    [TestMethod]
    public void DiceTerm_ToStringAllTermsTest()
    {
        // arrange
        var term = new DiceTerm(4, 6, 10, 3, 6);

        // act
        string result = term.ToString();

        // assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.AreEqual("4d6k3!6x10", result);
    }

    [TestMethod]
    public void DiceTerm_ToStringTest_NegativeScalar()
    {
        // arrange
        var term = new DiceTerm(1, 4, -1);

        // act
        string result = term.ToString();

        // assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.AreEqual("-1d4", result);
    }

    [TestMethod]
    public void DiceTerm_ToStringTest_FractionalScalar()
    {
        // arrange
        var term = new DiceTerm(1, 4, 0.5);

        // act
        string result = term.ToString();

        // assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.AreEqual("1d4/2", result);
    }
}
