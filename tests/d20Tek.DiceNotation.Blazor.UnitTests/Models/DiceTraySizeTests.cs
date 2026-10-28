namespace d20Tek.DiceNotation.Blazor.UnitTests.Models;

[TestClass]
public class DiceTraySizeTests
{
    [TestMethod]
    public void Default_WhenAccessed_ReturnsPositiveDimensions()
    {
        // Arrange
        // Act
        var result = DiceTraySize.Default;

        // Assert
        Assert.IsGreaterThan(0f, result.Width);
        Assert.IsGreaterThan(0f, result.Height);
    }

    [TestMethod]
    public void Validate_WithPositiveDimensions_DoesNotThrow()
    {
        // Arrange
        var size = new DiceTraySize(10f, 7f);

        // Act
        size.Validate();

        // Assert
        Assert.AreEqual(10f, size.Width);
        Assert.AreEqual(7f, size.Height);
    }

    [TestMethod]
    [DataRow(0f, 5f)]
    [DataRow(-1f, 5f)]
    [DataRow(5f, 0f)]
    [DataRow(5f, -1f)]
    [DataRow(float.NaN, 5f)]
    [DataRow(5f, float.PositiveInfinity)]
    public void Validate_WithInvalidDimensions_ThrowsArgumentOutOfRangeException(float width, float height)
    {
        // Arrange
        var size = new DiceTraySize(width, height);

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => size.Validate();
    }

    [TestMethod]
    public void Equals_WithSameDimensions_ReturnsTrue()
    {
        // Arrange
        var first = new DiceTraySize(8f, 6f);
        var second = new DiceTraySize(8f, 6f);

        // Act
        var result = first == second;

        // Assert
        Assert.IsTrue(result);
    }
}
