using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests.Helpers;

public static class AssertHelpers
{
    public static void IsWithinRangeInclusive(int min, int max, int value) => 
        Assert.IsTrue(value >= min && value <= max);

    public static void AssertDiceChoose(
        this DiceResult result,
        string expectedExpression,
        string expectedDiceType,
        int expectedTotalResults,
        int expectedAppliedResults,
        int modifier = 0)
    {
        Assert.AreEqual(expectedExpression, result.DiceExpression);
        Assert.HasCount(expectedTotalResults, result.Results);
        int sum = 0, count = 0;
        foreach (TermResult r in result.Results.Where(x => x.Type.Contains(expectedDiceType)))
        {
            Assert.Contains(expectedDiceType, r.Type);
            if (r.AppliesToResultCalculation)
            {
                sum += r.Value;
                count++;
            }
        }
        Assert.AreEqual(sum + modifier, result.Value);
        Assert.AreEqual(expectedAppliedResults, count);
    }

    public static void AssertResult(this DiceResult result, string expression, int expectedCount, int expectedResult)
    {
        Assert.IsNotNull(result);
        Assert.AreEqual(expression, result.DiceExpression);
        Assert.HasCount(expectedCount, result.Results);
        Assert.AreEqual(expectedResult, result.Value);
    }
}
