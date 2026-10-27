namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public class DieVisualCalculatorTravelDirectionTests
{
    private static DieState CreateDie(float velocityX, float velocityY) =>
        new("d1", 4, 1f, 1f, 0f, 0.6f, 0.6f, 0.5f, 6, velocityX, velocityY);

    [TestMethod]
    public void CalculateTravelDirection_WithFiniteVelocity_ReturnsNormalizedVector()
    {
        // Arrange
        var die = CreateDie(3f, 4f);

        // Act
        var (x, y) = DieVisualCalculator.CalculateTravelDirection(die);

        // Assert
        Assert.AreEqual(0.6f, x, 0.0001f);
        Assert.AreEqual(0.8f, y, 0.0001f);
        Assert.AreEqual(1f, MathF.Sqrt((x * x) + (y * y)), 0.0001f);
    }

    [TestMethod]
    public void CalculateTravelDirection_WithNegativeVelocity_PreservesDirectionSigns()
    {
        // Arrange
        var die = CreateDie(-3f, -4f);

        // Act
        var (x, y) = DieVisualCalculator.CalculateTravelDirection(die);

        // Assert
        Assert.AreEqual(-0.6f, x, 0.0001f);
        Assert.AreEqual(-0.8f, y, 0.0001f);
    }

    [TestMethod]
    public void CalculateTravelDirection_WithZeroVelocity_ReturnsZeroVector()
    {
        // Arrange
        var die = CreateDie(0f, 0f);

        // Act
        var (x, y) = DieVisualCalculator.CalculateTravelDirection(die);

        // Assert
        Assert.AreEqual(0f, x);
        Assert.AreEqual(0f, y);
    }

    [TestMethod]
    public void CalculateTravelDirection_WithSpeedAtOrBelowThreshold_ReturnsZeroVector()
    {
        // Arrange
        var die = CreateDie(0.0001f, 0f);

        // Act
        var (x, y) = DieVisualCalculator.CalculateTravelDirection(die);

        // Assert
        Assert.AreEqual(0f, x);
        Assert.AreEqual(0f, y);
    }

    [TestMethod]
    public void CalculateTravelDirection_WithSpeedJustAboveThreshold_ReturnsNormalizedVector()
    {
        // Arrange
        var die = CreateDie(0.001f, 0f);

        // Act
        var (x, y) = DieVisualCalculator.CalculateTravelDirection(die);

        // Assert
        Assert.AreEqual(1f, x, 0.0001f);
        Assert.AreEqual(0f, y, 0.0001f);
    }

    [TestMethod]
    [DataRow(float.NaN)]
    [DataRow(float.PositiveInfinity)]
    [DataRow(float.NegativeInfinity)]
    public void CalculateTravelDirection_WithNonFiniteHorizontalVelocity_IgnoresThatAxis(float velocityX)
    {
        // Arrange
        var die = CreateDie(velocityX, 2f);

        // Act
        var (x, y) = DieVisualCalculator.CalculateTravelDirection(die);

        // Assert
        Assert.AreEqual(0f, x, 0.0001f);
        Assert.AreEqual(1f, y, 0.0001f);
    }

    [TestMethod]
    [DataRow(float.NaN)]
    [DataRow(float.PositiveInfinity)]
    [DataRow(float.NegativeInfinity)]
    public void CalculateTravelDirection_WithNonFiniteVerticalVelocity_IgnoresThatAxis(float velocityY)
    {
        // Arrange
        var die = CreateDie(-2f, velocityY);

        // Act
        var (x, y) = DieVisualCalculator.CalculateTravelDirection(die);

        // Assert
        Assert.AreEqual(-1f, x, 0.0001f);
        Assert.AreEqual(0f, y, 0.0001f);
    }

    [TestMethod]
    public void CalculateTravelDirection_WithBothVelocitiesNonFinite_ReturnsZeroVector()
    {
        // Arrange
        var die = CreateDie(float.NaN, float.PositiveInfinity);

        // Act
        var (x, y) = DieVisualCalculator.CalculateTravelDirection(die);

        // Assert
        Assert.AreEqual(0f, x);
        Assert.AreEqual(0f, y);
    }

    [TestMethod]
    public void CalculateTravelDirection_WithDefaultDieState_ReturnsZeroVector()
    {
        // Arrange
        var die = new DieState("d1", 4, 1f, 1f, 0f, 0.6f, 0.6f);

        // Act
        var (x, y) = DieVisualCalculator.CalculateTravelDirection(die);

        // Assert
        Assert.AreEqual(0f, x);
        Assert.AreEqual(0f, y);
    }

    public TestContext TestContext { get; set; }
}
