using d20Tek.DiceNotation.DiceTerms;

namespace d20Tek.DiceNotation.UnitTests;

[TestClass]
public class DiceExpressionTests
{
    private readonly DiceExpression _expression = DiceExpression.Create();

    [TestMethod]
    public void AddConstant_AddsConstantTerm()
    {
        // arrange

        // act
        var result = _expression.AddConstant(3).Evaluate();

        // assert
        AssertSingleTerm<ConstantTerm>(result, "3");
    }

    [TestMethod]
    public void AddDice_HasSingleDiceTerm()
    {
        // arrange

        // act
        var result = _expression.AddDice(20).Evaluate();

        // assert
        AssertSingleTerm<DiceTerm>(result, "1d20");
    }

    [TestMethod]
    public void AddDice_WithMultipeDice()
    {
        // arrange

        // act
        var result = _expression.AddDice(6, 3).Evaluate();

        // assert
        AssertSingleTerm<DiceTerm>(result, "3d6");
    }

    [TestMethod]
    public void AddDice_WithScalarMultiplierDice()
    {
        // arrange

        // act
        var result = _expression.AddDice(8, 2, 10).Evaluate();

        // assert
        AssertSingleTerm<DiceTerm>(result, "2d8x10");
    }

    [TestMethod]
    public void AddDice_WithChooseDice()
    {
        // arrange

        // act
        var result = _expression.AddDice(6, 4, choose: 3).Evaluate();

        // assert
        AssertSingleTerm<DiceTerm>(result, "4d6k3");
    }

    [TestMethod]
    public void AddDice_WithExplodingDice()
    {
        // arrange

        // act
        var result = _expression.AddDice(6, 4, exploding: 6).Evaluate();

        // assert
        AssertSingleTerm<DiceTerm>(result, "4d6!6");
    }

    [TestMethod]
    public void AddDice_WithChainedDiceTerms()
    {
        // arrange

        // act
        var result = _expression.AddDice(6, 4, choose: 3).AddDice(8).AddConstant(5).Evaluate();

        // assert
        Assert.HasCount(3, result);
        Assert.IsInstanceOfType<DiceTerm>(result[0]);
        Assert.IsInstanceOfType<DiceTerm>(result[1]);
        Assert.IsInstanceOfType<ConstantTerm>(result[2]);
        Assert.AreEqual("4d6k3+1d8+5", _expression.ToString());
    }

    [TestMethod]
    public void Dice_DiceWith1Side()
    {
        // arrange

        // act
        var result = _expression.AddDice(1).Evaluate();

        // assert
        AssertSingleTerm<DiceTerm>(result, "1d1");
    }

    [TestMethod]
    public void Clear_RemovesTerms()
    {
        // arrange
        _expression.AddDice(6, 4, choose: 3).AddDice(8).AddConstant(5);

        // act
        var result = _expression.Clear().Evaluate();

        // assert
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void AddFudgeDice_WithMultipleDice()
    {
        // arrange

        // act
        var result = _expression.AddFudgeDice(3, null).Evaluate();

        // assert
        AssertSingleTerm<FudgeDiceTerm>(result, "3f");
    }

    [TestMethod]
    public void Concat_WithAnotherExpression()
    {
        // arrange
        _expression.AddDice(6, 4, choose: 3);
        var otherExpression = DiceExpression.Create().AddDice(8).AddConstant(5);

        // act
        var result = _expression.Concat(otherExpression).Evaluate();

        // assert
        Assert.HasCount(3, result);
        Assert.IsInstanceOfType<DiceTerm>(result[0]);
        Assert.IsInstanceOfType<DiceTerm>(result[1]);
        Assert.IsInstanceOfType<ConstantTerm>(result[2]);
        Assert.AreEqual("4d6k3+1d8+5", _expression.ToString());
    }

    [TestMethod]
    public void Concat_WithEmptyExpression()
    {
        // arrange
        _expression.AddDice(6, 4, choose: 3);
        var otherExpression = DiceExpression.Create();

        // act
        var result = _expression.Concat(otherExpression).Evaluate();

        // assert
        AssertSingleTerm<DiceTerm>(result, "4d6k3");
    }

    private static void AssertSingleTerm<T>(IReadOnlyList<IExpressionTerm> result, string expected)
    {
        Assert.HasCount(1, result);
        var term = Assert.IsInstanceOfType<T>(result[0]);
        Assert.AreEqual(expected, term.ToString());
    }
}
