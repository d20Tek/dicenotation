namespace d20Tek.DiceNotation.Blazor.UnitTests.Models;

[TestClass]
public class DiceRollerOptionsTests
{
    [TestMethod]
    public void Default_WhenAccessed_ReturnsPopulatedSettings()
    {
        // Arrange
        // Act
        var result = DiceRollerOptions.Default;

        // Assert
        Assert.IsNotNull(result.TraySize);
        Assert.IsNotNull(result.Theme);
        Assert.IsNotNull(result.VisualSettings);
        Assert.IsNotNull(result.SimulationSettings);
        Assert.IsFalse(result.ReducedMotion);
    }

    [TestMethod]
    public void ResolveTheme_WithReducedMotion_DisablesMotionEffects()
    {
        // Arrange
        var options = new DiceRollerOptions { ReducedMotion = true };

        // Act
        var result = options.ResolveTheme();

        // Assert
        Assert.IsFalse(result.UseMotionEffects);
    }

    [TestMethod]
    public void ResolveTheme_WithoutReducedMotion_ReturnsSameTheme()
    {
        // Arrange
        var theme = DiceTheme.Default;
        var options = new DiceRollerOptions { Theme = theme, ReducedMotion = false };

        // Act
        var result = options.ResolveTheme();

        // Assert
        Assert.AreSame(theme, result);
    }

    [TestMethod]
    public void Validate_WithDefaultSettings_DoesNotThrow()
    {
        // Arrange
        var options = DiceRollerOptions.Default;

        // Act
        options.Validate();

        // Assert
        Assert.IsNotNull(options);
    }

    [TestMethod]
    public void Validate_WithNullTraySize_ThrowsArgumentNullException()
    {
        // Arrange
        var options = new DiceRollerOptions { TraySize = null! };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => options.Validate();
    }

    [TestMethod]
    public void Validate_WithNullTheme_ThrowsArgumentNullException()
    {
        // Arrange
        var options = new DiceRollerOptions { Theme = null! };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => options.Validate();
    }

    [TestMethod]
    public void Validate_WithNullVisualSettings_ThrowsArgumentNullException()
    {
        // Arrange
        var options = new DiceRollerOptions { VisualSettings = null! };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => options.Validate();
    }

    [TestMethod]
    public void Validate_WithNullSimulationSettings_ThrowsArgumentNullException()
    {
        // Arrange
        var options = new DiceRollerOptions { SimulationSettings = null! };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => options.Validate();
    }

    [TestMethod]
    public void Validate_WithInvalidTraySize_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = new DiceRollerOptions { TraySize = new DiceTraySize(0f, 5f) };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => options.Validate();
    }
}
