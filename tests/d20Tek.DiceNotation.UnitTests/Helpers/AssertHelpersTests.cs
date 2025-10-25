using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.DiceNotation.UnitTests.Helpers;

[TestClass]
public class AssertHelpersTests
{
    [TestMethod]
    public void IsWithinRangeInclusive_Valid()
    {
        // arrange

        // act

        // assert
        AssertHelpers.IsWithinRangeInclusive(1, 20, 5);
    }

    [TestMethod]
    public void IsWithinRangeInclusive_LessThanMin()
    {
        // arrange

        // act

        // assert
        Assert.Throws<AssertFailedException>(
            [ExcludeFromCodeCoverage] () => AssertHelpers.IsWithinRangeInclusive(1, 20, -2));
    }

    [TestMethod]
    public void IsWithinRangeInclusive_GreaterThanMax()
    {
        // arrange

        // act

        // assert
        Assert.Throws<AssertFailedException>(
            [ExcludeFromCodeCoverage] () => AssertHelpers.IsWithinRangeInclusive(1, 20, 25));
    }
}
