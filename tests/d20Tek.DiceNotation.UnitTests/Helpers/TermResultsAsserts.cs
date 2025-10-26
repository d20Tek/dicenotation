using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests.Helpers;

internal static class TermResultsAsserts
{
    public static void AssertInstanceOf<TConcrete>(this IExpressionTerm termExpression)
    {
        Assert.IsNotNull(termExpression);
        Assert.IsInstanceOfType<IExpressionTerm>(termExpression);
        Assert.IsInstanceOfType<TConcrete>(termExpression);
    }

    public static void AssertConstant(
        this IReadOnlyList<TermResult> results,
        int expectedCount,
        string expectedTermType,
        int expectedValue)
    {
        Assert.IsNotNull(results);
        Assert.HasCount(expectedCount, results);

        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.Contains(expectedTermType, r.Type);
            Assert.AreEqual(1, r.Scalar);
            Assert.AreEqual(expectedValue, r.Value);
        }
    }

    public static void AssertInRange(
        this IReadOnlyList<TermResult> results,
        int expectedCount,
        string expectedTermType,
        int expectedMin,
        int expectedMax)
    {
        Assert.IsNotNull(results);
        Assert.HasCount(expectedCount, results);

        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.Contains(expectedTermType, r.Type);
            Assert.AreEqual(1, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(expectedMin, expectedMax, r.Value);
        }
    }

    public static void AssertWithChoose(
        this IReadOnlyList<TermResult> results,
        int expectedCount,
        string expectedTermType,
        int expectedMin,
        int expectedMax,
        int expectedChosen)
    {
        Assert.IsNotNull(results);
        Assert.IsGreaterThanOrEqualTo(expectedCount, results.Count);

        int chosen = 0;
        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.Contains(expectedTermType, r.Type);
            Assert.AreEqual(1, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(expectedMin, expectedMax, r.Value);
            if (r.AppliesToResultCalculation) chosen++;
        }
        Assert.AreEqual(expectedChosen, chosen);
    }

    public static void AssertWithExploding(
        this IReadOnlyList<TermResult> results,
        int expectedCount,
        string expectedTermType,
        int expectedMin,
        int expectedMax,
        int explodingValue)
    {
        Assert.IsNotNull(results);

        int runningCount = expectedCount;
        int explodeCount = 0;
        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.Contains(expectedTermType, r.Type);
            Assert.AreEqual(1, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(expectedMin, expectedMax, r.Value);
            if (r.Value >= explodingValue)
            {
                runningCount++;
                explodeCount++;
            }
        }
        Assert.AreEqual(expectedCount + explodeCount, runningCount);
    }

    public static void AssertWithScalar(
        this IReadOnlyList<TermResult> results,
        int expectedCount,
        string expectedTermType,
        int expectedMin,
        int expectedMax,
        int expectedScalar)
    {
        Assert.IsNotNull(results);
        Assert.HasCount(expectedCount, results);
        foreach (TermResult r in results)
        {
            Assert.IsNotNull(r);
            Assert.AreEqual(expectedScalar, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(expectedMin, expectedMax, r.Value);
            Assert.AreEqual(expectedTermType, r.Type);
        }
    }
}
