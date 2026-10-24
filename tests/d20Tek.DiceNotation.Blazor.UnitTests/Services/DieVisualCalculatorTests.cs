namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public sealed class DieVisualCalculatorTests
{
    private static DieState Die(
        float energy = 0f,
        int value = 4,
        int sides = 6,
        float rotation = 0f) =>
        new("die-0", value, 0f, 0f, rotation, 0.6f, 0.6f, energy, sides);

    [TestMethod]
    public void Constructor_WithNullSettings_UsesDefaults()
    {
        // Arrange & Act
        var calculator = new DieVisualCalculator();
        var state = calculator.Resolve(Die(), 0f, true, true);

        // Assert
        Assert.AreEqual(1f, state.Lift);
    }

    [TestMethod]
    public void Constructor_WithInvalidSettings_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceVisualSettings { SmearSteps = 0 };

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => _ = new DieVisualCalculator(settings);
    }

    [TestMethod]
    public void Resolve_WithNullDie_ThrowsArgumentNullException()
    {
        // Arrange
        var calculator = new DieVisualCalculator();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => calculator.Resolve(null!, 0f, false, true);
    }

    [TestMethod]
    public void Resolve_WhenSettled_ShowsTrueValueAndNoMotion()
    {
        // Arrange
        var calculator = new DieVisualCalculator();

        // Act
        // A settled scene forces energy to zero even when the body still reports motion.
        var state = calculator.Resolve(Die(energy: 1f, value: 5, rotation: 9f), 2f, true, true);

        // Assert
        Assert.AreEqual(5, state.ShownValue);
        Assert.AreEqual(1f, state.Lift);
        Assert.AreEqual(0f, state.SmearStrength);
        Assert.AreEqual(0f, state.SmearReach);
    }

    [TestMethod]
    public void Resolve_WhenMotionEffectsDisabled_ShowsTrueValueAndNoMotion()
    {
        // Arrange
        var calculator = new DieVisualCalculator();

        // Act
        var state = calculator.Resolve(Die(energy: 1f, value: 2, rotation: 9f), 2f, false, false);

        // Assert
        Assert.AreEqual(2, state.ShownValue);
        Assert.AreEqual(1f, state.Lift);
        Assert.AreEqual(0f, state.SmearStrength);
    }

    [TestMethod]
    public void Resolve_AtFullEnergy_AppliesMaximumLift()
    {
        // Arrange
        var settings = new DiceVisualSettings();
        var calculator = new DieVisualCalculator(settings);

        // Act
        var state = calculator.Resolve(Die(energy: 1f), 1f, false, true);

        // Assert
        Assert.AreEqual(1f + settings.MaxLift, state.Lift, 0.0001f);
    }

    [TestMethod]
    public void Resolve_AtRest_AppliesBaseShadowOnly()
    {
        // Arrange
        var settings = new DiceVisualSettings();
        var calculator = new DieVisualCalculator(settings);

        // Act
        var state = calculator.Resolve(Die(energy: 0f), 1f, false, true);

        // Assert
        Assert.AreEqual(settings.BaseShadowBlur, state.ShadowBlur, 0.0001f);
        Assert.AreEqual(settings.BaseShadowOffset, state.ShadowOffset, 0.0001f);
    }

    [TestMethod]
    public void Resolve_AtFullEnergy_AppliesFullShadow()
    {
        // Arrange
        var settings = new DiceVisualSettings();
        var calculator = new DieVisualCalculator(settings);

        // Act
        var state = calculator.Resolve(Die(energy: 1f), 1f, false, true);

        // Assert
        Assert.AreEqual(settings.BaseShadowBlur + settings.EnergyShadowBlur, state.ShadowBlur, 0.0001f);
        Assert.AreEqual(settings.BaseShadowOffset + settings.EnergyShadowOffset, state.ShadowOffset, 0.0001f);
    }

    [TestMethod]
    public void Resolve_BelowSmearThreshold_ProducesNoSmear()
    {
        // Arrange
        var settings = new DiceVisualSettings();
        var calculator = new DieVisualCalculator(settings);

        // Act
        var state = calculator.Resolve(Die(energy: settings.SmearEnergy), 1f, false, true);

        // Assert
        Assert.AreEqual(0f, state.SmearStrength);
    }

    [TestMethod]
    public void Resolve_AboveSmearThreshold_ProducesSmear()
    {
        // Arrange
        var settings = new DiceVisualSettings();
        var calculator = new DieVisualCalculator(settings);

        // Act
        var state = calculator.Resolve(Die(energy: 0.9f), 1f, false, true);

        // Assert
        Assert.AreEqual(0.9f, state.SmearStrength, 0.0001f);
        Assert.AreEqual(0.6f * 0.9f * settings.SmearReach, state.SmearReach, 0.0001f);
    }

    [TestMethod]
    public void Resolve_AtOrBelowFaceLockEnergy_ShowsTrueValue()
    {
        // Arrange
        var settings = new DiceVisualSettings();
        var calculator = new DieVisualCalculator(settings);

        // Act
        var state = calculator.Resolve(
            Die(energy: settings.FaceLockEnergy, value: 3, rotation: 7.5f), 1f, false, true);

        // Assert
        Assert.AreEqual(3, state.ShownValue);
    }

    [TestMethod]
    public void Resolve_WithNonFiniteEnergy_TreatsDieAsAtRest()
    {
        // Arrange
        var calculator = new DieVisualCalculator();

        // Act
        var state = calculator.Resolve(Die(energy: float.NaN, value: 6), 1f, false, true);

        // Assert
        Assert.AreEqual(6, state.ShownValue);
        Assert.AreEqual(1f, state.Lift);
    }

    [TestMethod]
    public void Resolve_WithEnergyAboveOne_ClampsToFullEffect()
    {
        // Arrange
        var settings = new DiceVisualSettings();
        var calculator = new DieVisualCalculator(settings);

        // Act
        var state = calculator.Resolve(Die(energy: 5f), 1f, false, true);

        // Assert
        Assert.AreEqual(1f + settings.MaxLift, state.Lift, 0.0001f);
    }

    [TestMethod]
    public void Resolve_WhenSettled_ReturnsTrueValue()
    {
        // Arrange
        var calculator = new DieVisualCalculator();

        // Act
        var state = calculator.Resolve(Die(energy: 0.9f, value: 3), 1f, true, true);

        // Assert
        Assert.AreEqual(3, state.ShownValue);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(6)]
    [DataRow(8)]
    [DataRow(20)]
    [DataRow(100)]
    public void ChurnFace_ForAnySideCount_StaysWithinFaceRange(int sides)
    {
        // Arrange
        // The churn must respect the die's own face count, not its rolled value.
        var results = new List<int>();

        // Act
        for (var i = 0; i < 400; i++)
        {
            results.Add(DieVisualCalculator.ChurnFace(Die(sides: sides, rotation: i * 0.37f), 0.8f, 1f));
        }

        // Assert
        foreach (var result in results)
        {
            Assert.IsGreaterThanOrEqualTo(1, result);
            Assert.IsLessThanOrEqualTo(sides, result);
        }
    }

    [TestMethod]
    public void ChurnFace_ForTwentySidedDie_ReachesFacesAboveSix()
    {
        // Arrange
        // A d20 that rolled a low number must still churn through its high faces.
        var seen = new HashSet<int>();

        // Act
        for (var i = 0; i < 400; i++)
        {
            seen.Add(DieVisualCalculator.ChurnFace(Die(value: 2, sides: 20, rotation: i * 0.41f), 0.8f, 1f));
        }

        // Assert
        Assert.IsTrue(seen.Any(v => v > 6), "Expected a d20 to churn past face 6.");
    }

    [TestMethod]
    public void ChurnFace_WithSingleSidedDie_AlwaysReturnsOne()
    {
        // Arrange & Act
        var result = DieVisualCalculator.ChurnFace(Die(sides: 1, rotation: 5f), 0.8f, 1f);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void ChurnFace_WithZeroSides_ReturnsOne()
    {
        // Arrange & Act
        // A malformed side count must not divide by zero.
        var result = DieVisualCalculator.ChurnFace(Die(sides: 0, rotation: 5f), 0.8f, 1f);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void ChurnFace_WithNonFiniteRotation_FallsBackToTrueValue()
    {
        // Arrange & Act
        var result = DieVisualCalculator.ChurnFace(Die(value: 5, rotation: float.PositiveInfinity), 0.8f, 1f);

        // Assert
        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public void ChurnFace_WithSameInputs_IsDeterministic()
    {
        // Arrange
        var die = Die(sides: 20, rotation: 3.75f);

        // Act
        var first = DieVisualCalculator.ChurnFace(die, 0.8f, 1f);
        var second = DieVisualCalculator.ChurnFace(die, 0.8f, 1f);

        // Assert
        Assert.AreEqual(first, second);
    }
}
