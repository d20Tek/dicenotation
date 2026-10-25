namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public sealed class DieVisualCalculatorFaceRangeTests
{
    private static DieState Die(
        float energy = 0f,
        int value = 4,
        int sides = 6,
        float rotation = 0f,
        int minValue = 1) =>
        new("die-0", value, 0f, 0f, rotation, 0.6f, 0.6f, energy, sides, 0f, 0f, minValue);

    [TestMethod]
    public void ChurnFace_ForFudgeDie_StaysWithinNegativeOneToPositiveOne()
    {
        // Arrange
        // A fudge die is three-sided but reads as -1, 0, and +1.
        var results = new List<int>();

        // Act
        for (var i = 0; i < 400; i++)
        {
            results.Add(DieVisualCalculator.ChurnFace(
                Die(sides: 3, minValue: -1, rotation: i * 0.37f),
                0.8f,
                1f));
        }

        // Assert
        foreach (var result in results)
        {
            Assert.IsGreaterThanOrEqualTo(-1, result);
            Assert.IsLessThanOrEqualTo(1, result);
        }
    }

    [TestMethod]
    public void ChurnFace_ForFudgeDie_ReachesEveryFace()
    {
        // Arrange
        var seen = new HashSet<int>();

        // Act
        for (var i = 0; i < 400; i++)
        {
            seen.Add(DieVisualCalculator.ChurnFace(
                Die(sides: 3, minValue: -1, rotation: i * 0.41f),
                0.8f,
                1f));
        }

        // Assert
        Assert.AreSequenceEqual([-1, 0, 1], [.. seen], SequenceOrder.InAnyOrder);
    }

    [TestMethod]
    public void ChurnFace_ForStandardDie_StillStartsAtOne()
    {
        // Arrange
        // The default minimum must leave existing die behavior unchanged.
        var seen = new HashSet<int>();

        // Act
        for (var i = 0; i < 400; i++)
        {
            seen.Add(DieVisualCalculator.ChurnFace(Die(sides: 6, rotation: i * 0.37f), 0.8f, 1f));
        }

        // Assert
        Assert.AreEqual(1, seen.Min());
        Assert.AreEqual(6, seen.Max());
    }

    [TestMethod]
    public void ChurnFace_WithSingleFaceAndCustomMinimum_ReturnsThatMinimum()
    {
        // Arrange & Act
        var result = DieVisualCalculator.ChurnFace(Die(sides: 1, minValue: -1, rotation: 5f), 0.8f, 1f);

        // Assert
        Assert.AreEqual(-1, result);
    }

    [TestMethod]
    public void ChurnFace_WithZeroSidesAndCustomMinimum_ReturnsThatMinimum()
    {
        // Arrange & Act
        var result = DieVisualCalculator.ChurnFace(Die(sides: 0, minValue: -1, rotation: 5f), 0.8f, 1f);

        // Assert
        Assert.AreEqual(-1, result);
    }

    [TestMethod]
    public void ChurnFace_ForFudgeDieWithNonFiniteRotation_FallsBackToTrueValue()
    {
        // Arrange & Act
        var result = DieVisualCalculator.ChurnFace(
            Die(value: 0, sides: 3, minValue: -1, rotation: float.PositiveInfinity),
            0.8f,
            1f);

        // Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Resolve_ForTumblingFudgeDie_ShowsAFudgeFace()
    {
        // Arrange
        var calculator = new DieVisualCalculator();
        var die = Die(energy: 0.9f, value: 1, sides: 3, rotation: 2.5f, minValue: -1);

        // Act
        var state = calculator.Resolve(die, 1f, false, true);

        // Assert
        Assert.IsGreaterThanOrEqualTo(-1, state.ShownValue);
        Assert.IsLessThanOrEqualTo(1, state.ShownValue);
    }

    [TestMethod]
    public void Resolve_ForSettledFudgeDie_ShowsTheTrueValue()
    {
        // Arrange
        var calculator = new DieVisualCalculator();
        var die = Die(energy: 0.9f, value: -1, sides: 3, rotation: 2.5f, minValue: -1);

        // Act
        var state = calculator.Resolve(die, 1f, true, true);

        // Assert
        Assert.AreEqual(-1, state.ShownValue);
    }

    [TestMethod]
    public void DieState_WithoutExplicitMinValue_DefaultsToOne()
    {
        // Arrange & Act
        var die = new DieState("die-0", 3, 0f, 0f, 0f, 0.6f, 0.6f);

        // Assert
        Assert.AreEqual(1, die.MinValue);
    }

    [TestMethod]
    public void VisualDie_WithoutExplicitOptions_DefaultsToKeptAndMinimumOne()
    {
        // Arrange & Act
        var die = new VisualDie("die-0", "d6", 4);

        // Assert
        Assert.AreEqual(1, die.MinValue);
        Assert.AreEqual(DiceNotation.Results.DieRollRole.Kept, die.Roles);
    }
}
