using d20Tek.DiceNotation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace D20Tek.DiceNotation.UnitTests.Helpers
{
    public static class AssertHelpers
    {
        public static void IsWithinRangeInclusive(int min, int max, int value)
        {
            Assert.IsTrue(value >= min && value <= max);
        }

        public static void AssertDiceChoose(DiceResult result, string expectedExpression,
                                            string expectedDiceType, int expectedTotalResults,
                                            int expectedAppliedResults, int modifier = 0)
        {
            Assert.AreEqual(expectedExpression, result.DiceExpression);
            Assert.AreEqual(expectedTotalResults, result.Results.Count);
            int sum = 0, count = 0;
            foreach (TermResult r in result.Results)
            {
                Assert.IsTrue(r.Type.Contains(expectedDiceType));
                if (r.AppliesToResultCalculation)
                {
                    sum += r.Value;
                    count++;
                }
            }
            Assert.AreEqual(sum + modifier, result.Value);
            Assert.AreEqual(expectedAppliedResults, count);
        }
    }
}
