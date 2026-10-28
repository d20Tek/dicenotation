namespace d20Tek.DiceNotation.Blazor.UnitTests.Models;

[TestClass]
public class DiceThemeTests
{
    [TestMethod]
    public void Default_WhenAccessed_HasValidColors()
    {
        // Arrange
        // Act
        var theme = DiceTheme.Default;

        // Assert
        theme.Validate();
        Assert.AreEqual("Default", theme.Name);
    }

    [TestMethod]
    [DataRow("DarkFantasy")]
    [DataRow("Parchment")]
    public void BuiltInThemes_WhenValidated_DoNotThrow(string name)
    {
        // Arrange
        var theme = name == "DarkFantasy" ? DiceTheme.DarkFantasy : DiceTheme.Parchment;

        // Act
        theme.Validate();

        // Assert
        Assert.AreEqual(name, theme.Name);
    }

    [TestMethod]
    public void Validate_WithZeroCornerRadii_DoesNotThrow()
    {
        // Arrange
        var theme = DiceTheme.Default with { TrayCornerRadius = 0f, DieCornerRadius = 0f };

        // Act
        theme.Validate();

        // Assert
        Assert.AreEqual(0f, theme.DieCornerRadius);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow(null)]
    public void Validate_WithMissingTrayColor_ThrowsArgumentException(string? color)
    {
        // Arrange
        var theme = DiceTheme.Default with { TrayColor = color! };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => theme.Validate();
    }

    [TestMethod]
    public void Validate_WithMissingTrayBorderColor_ThrowsArgumentException()
    {
        // Arrange
        var theme = DiceTheme.Default with { TrayBorderColor = "" };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => theme.Validate();
    }

    [TestMethod]
    public void Validate_WithMissingDieColor_ThrowsArgumentException()
    {
        // Arrange
        var theme = DiceTheme.Default with { DieColor = "" };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => theme.Validate();
    }

    [TestMethod]
    public void Validate_WithMissingDieBorderColor_ThrowsArgumentException()
    {
        // Arrange
        var theme = DiceTheme.Default with { DieBorderColor = "" };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => theme.Validate();
    }

    [TestMethod]
    public void Validate_WithMissingDieTextColor_ThrowsArgumentException()
    {
        // Arrange
        var theme = DiceTheme.Default with { DieTextColor = "" };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => theme.Validate();
    }

    [TestMethod]
    public void Validate_WithMissingDieShadowColor_ThrowsArgumentException()
    {
        // Arrange
        var theme = DiceTheme.Default with { DieShadowColor = "" };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => theme.Validate();
    }

    [TestMethod]
    [DataRow(-0.1f)]
    [DataRow(float.NaN)]
    [DataRow(float.PositiveInfinity)]
    public void Validate_WithInvalidTrayCornerRadius_ThrowsArgumentOutOfRangeException(float radius)
    {
        // Arrange
        var theme = DiceTheme.Default with { TrayCornerRadius = radius };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => theme.Validate();
    }

    [TestMethod]
    public void Validate_WithNegativeDieCornerRadius_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var theme = DiceTheme.Default with { DieCornerRadius = -1f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => theme.Validate();
    }
}
