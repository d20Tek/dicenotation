using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.DiceNotation.UnitTests.Helpers
{
    [TestClass]
    public class AssertHelpersTests
    {
        [TestMethod]
        public void IsWithinRangeInclusive_Valid()
        {
            // setup test

            // run test

            // validate results
            AssertHelpers.IsWithinRangeInclusive(1, 20, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        [ExcludeFromCodeCoverage]
        public void IsWithinRangeInclusive_LessThanMin()
        {
            // setup test

            // run test

            // validate results
            AssertHelpers.IsWithinRangeInclusive(1, 20, -2);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        [ExcludeFromCodeCoverage]
        public void IsWithinRangeInclusive_GreaterThanMax()
        {
            // setup test

            // run test

            // validate results
            AssertHelpers.IsWithinRangeInclusive(1, 20, 25);
        }
    }
}
