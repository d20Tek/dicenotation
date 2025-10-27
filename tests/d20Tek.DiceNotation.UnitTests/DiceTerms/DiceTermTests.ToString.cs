using d20Tek.DiceNotation.DiceTerms;

namespace d20Tek.DiceNotation.UnitTests.DiceTerms;

[TestClass]
public class DiceTermToStringTests
{
    [TestMethod]
    public void DiceTerm_ToStringTest()
    {
        // arrange
        var term = new DiceTerm(2, 10);

        // act
        string result = term.ToString();

        // assert
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
        Assert.AreEqual("1d4/2", result);
    }
}
