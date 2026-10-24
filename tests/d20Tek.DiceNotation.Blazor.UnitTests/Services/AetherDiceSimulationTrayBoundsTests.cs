namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public class AetherDiceSimulationTrayBoundsTests
{
    private const float MaxX = 4.6f;
    private const float MaxY = 3.6f;

    [TestMethod]
    public void IsInsideTray_WithPositionAtOrigin_ReturnsTrue()
    {
        // Arrange
        // Act
        var result = AetherDiceSimulation.IsInsideTray(0f, 0f, MaxX, MaxY);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow(2f, 1f, DisplayName = "Positive X and positive Y")]
    [DataRow(-2f, 1f, DisplayName = "Negative X and positive Y")]
    [DataRow(2f, -1f, DisplayName = "Positive X and negative Y")]
    [DataRow(-2f, -1f, DisplayName = "Negative X and negative Y")]
    public void IsInsideTray_WithPositionWellInsideBounds_ReturnsTrue(float x, float y)
    {
        // Arrange
        // Act
        var result = AetherDiceSimulation.IsInsideTray(x, y, MaxX, MaxY);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow(MaxX, 0f, DisplayName = "X exactly at positive bound")]
    [DataRow(-MaxX, 0f, DisplayName = "X exactly at negative bound")]
    [DataRow(0f, MaxY, DisplayName = "Y exactly at positive bound")]
    [DataRow(0f, -MaxY, DisplayName = "Y exactly at negative bound")]
    [DataRow(MaxX, MaxY, DisplayName = "Both axes exactly at bound")]
    [DataRow(-MaxX, -MaxY, DisplayName = "Both axes exactly at negative bound")]
    public void IsInsideTray_WithPositionExactlyOnBound_ReturnsTrue(float x, float y)
    {
        // Arrange
        // Act
        var result = AetherDiceSimulation.IsInsideTray(x, y, MaxX, MaxY);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow(MaxX + 0.01f, 0f, DisplayName = "X just beyond positive bound")]
    [DataRow(-MaxX - 0.01f, 0f, DisplayName = "X just beyond negative bound")]
    public void IsInsideTray_WithXOutsideBoundAndYInside_ReturnsFalse(float x, float y)
    {
        // Arrange
        // Act
        var result = AetherDiceSimulation.IsInsideTray(x, y, MaxX, MaxY);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow(0f, MaxY + 0.01f, DisplayName = "Y just beyond positive bound")]
    [DataRow(0f, -MaxY - 0.01f, DisplayName = "Y just beyond negative bound")]
    public void IsInsideTray_WithXInsideBoundAndYOutside_ReturnsFalse(float x, float y)
    {
        // Arrange
        // Act
        var result = AetherDiceSimulation.IsInsideTray(x, y, MaxX, MaxY);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow(100f, 100f, DisplayName = "Both axes positive and far outside")]
    [DataRow(-100f, -100f, DisplayName = "Both axes negative and far outside")]
    public void IsInsideTray_WithBothAxesOutsideBound_ReturnsFalse(float x, float y)
    {
        // Arrange
        // Act
        var result = AetherDiceSimulation.IsInsideTray(x, y, MaxX, MaxY);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow(float.NaN, 0f, DisplayName = "X is NaN")]
    [DataRow(0f, float.NaN, DisplayName = "Y is NaN")]
    [DataRow(float.PositiveInfinity, 0f, DisplayName = "X is positive infinity")]
    [DataRow(0f, float.NegativeInfinity, DisplayName = "Y is negative infinity")]
    public void IsInsideTray_WithNonFinitePosition_ReturnsFalse(float x, float y)
    {
        // Arrange
        // Act
        var result = AetherDiceSimulation.IsInsideTray(x, y, MaxX, MaxY);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void AllDiceInsideTray_WithNoDice_ReturnsTrue()
    {
        // Arrange
        using var simulation = new AetherDiceSimulation(null, new Random(11));

        // Act
        var result = simulation.AllDiceInsideTray();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void AllDiceInsideTray_AfterStartRollWithCustomTray_UsesThatTraySize()
    {
        // Arrange
        var simulation = new AetherDiceSimulation(null, new Random(12));
        simulation.StartRoll(VisualDiceFactory.Create(4), new DiceTraySize(20f, 14f));

        // Act
        var result = simulation.AllDiceInsideTray();

        // Assert
        Assert.IsTrue(result);
        simulation.Dispose();
    }
}
