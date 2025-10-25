using d20Tek.DiceNotation.DiceTerms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace d20Tek.DiceNotation.UnitTests.DiceTerms;

[TestClass]
[ExcludeFromCodeCoverage]
public class DiceTermErrorTests
{
    [TestMethod]
    public void DiceTerm_ConstructorInvalidNumDiceTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DiceTerm(0, 6));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DiceTerm(-5, 6));
    }

    [TestMethod]
    public void DiceTerm_ConstructorInvalidSidesTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DiceTerm(3, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DiceTerm(0, 1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DiceTerm(1, -20));
    }

    [TestMethod]
    public void DiceTerm_ConstructorInvalidScalarTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DiceTerm(2, 8, 0));
    }

    [TestMethod]
    public void DiceTerm_ConstructorInvalidChooseTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DiceTerm(3, 6, choose: 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DiceTerm(3, 6, choose: -4));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DiceTerm(3, 6, choose: 4));
    }

    [TestMethod]
    public void DiceTerm_ConstructorInvalidExplodingTest()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DiceTerm(3, 6, exploding: 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DiceTerm(3, 6, exploding: -1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DiceTerm(3, 6, exploding: 7));
    }
}
