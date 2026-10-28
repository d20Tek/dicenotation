namespace d20Tek.DiceNotation.Blazor.UnitTests.Models;

[TestClass]
public class DiceThemeMotionTests
{
    [TestMethod]
    public void Default_ForMotionStyling_EnablesEffectsWithColors()
    {
        // Arrange
        // Act
        var theme = DiceTheme.Default;

        // Assert
        Assert.IsTrue(theme.UseMotionEffects);
        Assert.IsFalse(string.IsNullOrWhiteSpace(theme.DieBevelLightColor));
        Assert.IsFalse(string.IsNullOrWhiteSpace(theme.DieBevelDarkColor));
        Assert.IsFalse(string.IsNullOrWhiteSpace(theme.DieMotionColor));
    }

    [DataRow("DarkFantasy")]
    [DataRow("Parchment")]
    [TestMethod]
    public void BuiltInThemes_WithMotionStyling_AreValid(string name)
    {
        // Arrange
        var theme = name == "DarkFantasy" ? DiceTheme.DarkFantasy : DiceTheme.Parchment;

        // Act
        theme.Validate();

        // Assert
        Assert.AreEqual(name, theme.Name);
        Assert.IsTrue(theme.UseMotionEffects);
    }

    [TestMethod]
    public void Validate_WithReducedMotionTheme_Succeeds()
    {
        // Arrange
        var theme = DiceTheme.Default with { UseMotionEffects = false };

        // Act
        theme.Validate();

        // Assert
        Assert.IsFalse(theme.UseMotionEffects);
    }

    [DataRow("DieBevelLightColor")]
    [DataRow("DieBevelDarkColor")]
    [DataRow("DieMotionColor")]
    [TestMethod]
    public void Validate_WithMissingMotionColor_ThrowsArgumentException(string property)
    {
        // Arrange
        var theme = property switch
        {
            "DieBevelLightColor" => DiceTheme.Default with { DieBevelLightColor = " " },
            "DieBevelDarkColor" => DiceTheme.Default with { DieBevelDarkColor = " " },
            _ => DiceTheme.Default with { DieMotionColor = " " },
        };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => theme.Validate();
    }
}
