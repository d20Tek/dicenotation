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
public class FudgeDiceTermTests
{
    private readonly IDieRoller _roller = new RandomDieRoller();

    [TestMethod]
    public void FudgeDiceTerm_ConstructorTest()
    {
        // arrange

        // act
        IExpressionTerm term = new FudgeDiceTerm(3);

        // assert
        Assert.IsNotNull(term);
        Assert.IsInstanceOfType<IExpressionTerm>(term);
        Assert.IsInstanceOfType<FudgeDiceTerm>(term);
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsTest()
    {
        // arrange
        var term = new FudgeDiceTerm(1);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_roller);

        // assert
        Assert.IsNotNull(results);
        Assert.HasCount(1, results);
        TermResult r = results[0];
        Assert.IsNotNull(r);
        Assert.AreEqual(1, r.Scalar);
        AssertHelpers.IsWithinRangeInclusive(-1, 1, r.Value);
        Assert.AreEqual("FudgeDiceTerm.dF", r.Type);
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsMultipleDiceTest()
    {
        // arrange
        var term = new FudgeDiceTerm(3);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_roller);

        // assert
        Assert.IsNotNull(results);
        Assert.HasCount(3, results);
        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(-1, 1, r.Value);
            Assert.AreEqual("FudgeDiceTerm.dF", r.Type);
        }
    }

    [TestMethod]
    public void DiceTerm_CalculateResultsChooseDiceTest()
    {
        // arrange
        var term = new FudgeDiceTerm(5, 3);

        // act
        IReadOnlyList<TermResult> results = term.CalculateResults(_roller);

        // assert
        Assert.IsNotNull(results);
        Assert.HasCount(5, results);
        int included = 0;
        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(-1, 1, r.Value);
            Assert.AreEqual("FudgeDiceTerm.dF", r.Type);
            if (r.AppliesToResultCalculation) included++;
        }
        Assert.AreEqual(3, included);
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
        Assert.IsFalse(string.IsNullOrEmpty(result));
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
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.AreEqual("5fk3", result);
    }

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FudgeDiceTermErrorTests
    {
        [TestMethod]
        public void FudgeDiceTerm_ConstructorInvalidNumDiceTest()
        {
            // arrange

            // act - assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(-5));
        }

        [TestMethod]
        public void FudgeDiceTerm_ConstructorInvalidChooseTest()
        {
            // arrange

            // act - assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(3, choose: 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(3, choose: -4));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(3, choose: 4));
        }
    }
}
