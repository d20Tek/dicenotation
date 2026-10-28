namespace d20Tek.DiceNotation.Blazor.UnitTests.Models;

[TestClass]
public sealed class DiceVisualSettingsTests
{
    [TestMethod]
    public void Validate_WithDefaults_DoesNotThrow()
    {
        // Arrange
        var settings = new DiceVisualSettings();

        // Act
        settings.Validate();

        // Assert
        Assert.AreEqual(0.18f, settings.FaceLockEnergy);
        Assert.AreEqual(0.25f, settings.SmearEnergy);
        Assert.AreEqual(3, settings.SmearSteps);
        Assert.IsTrue(settings.LightAngle < 0f);
    }

    [TestMethod]
    public void Default_WhenRead_ReturnsValidatedInstance()
    {
        // Arrange & Act
        var settings = DiceVisualSettings.Default;

        // Assert
        settings.Validate();
        Assert.AreEqual(new DiceVisualSettings(), settings);
    }

    [TestMethod]
    [DataRow(-0.1f)]
    [DataRow(1.1f)]
    [DataRow(float.NaN)]
    public void Validate_WithFaceLockEnergyOutOfRange_ThrowsArgumentOutOfRangeException(float value)
    {
        // Arrange
        var settings = new DiceVisualSettings { FaceLockEnergy = value };

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    [DataRow(-0.5f)]
    [DataRow(1.5f)]
    public void Validate_WithSmearEnergyOutOfRange_ThrowsArgumentOutOfRangeException(float value)
    {
        // Arrange
        var settings = new DiceVisualSettings { SmearEnergy = value };

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    [DataRow(-0.2f)]
    [DataRow(2f)]
    public void Validate_WithBevelThicknessOutOfRange_ThrowsArgumentOutOfRangeException(float value)
    {
        // Arrange
        var settings = new DiceVisualSettings { BevelThickness = value };

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNonFiniteLightAngle_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceVisualSettings { LightAngle = float.PositiveInfinity };

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNegativeMaxLift_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceVisualSettings { MaxLift = -1f };

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNegativeShadowValues_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var blur = new DiceVisualSettings { BaseShadowBlur = -0.1f };
        var energyBlur = new DiceVisualSettings { EnergyShadowBlur = -0.1f };
        var offset = new DiceVisualSettings { BaseShadowOffset = -0.1f };
        var energyOffset = new DiceVisualSettings { EnergyShadowOffset = -0.1f };

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(ActBlur);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(ActEnergyBlur);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(ActOffset);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(ActEnergyOffset);

        [ExcludeFromCodeCoverage]
        void ActBlur() => blur.Validate();

        [ExcludeFromCodeCoverage]
        void ActEnergyBlur() => energyBlur.Validate();

        [ExcludeFromCodeCoverage]
        void ActOffset() => offset.Validate();

        [ExcludeFromCodeCoverage]
        void ActEnergyOffset() => energyOffset.Validate();
    }

    [TestMethod]
    public void Validate_WithNegativeSmearReach_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceVisualSettings { SmearReach = -0.3f };

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-2)]
    public void Validate_WithSmearStepsBelowOne_ThrowsArgumentOutOfRangeException(int value)
    {
        // Arrange
        var settings = new DiceVisualSettings { SmearSteps = value };

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithBoundaryFractions_DoesNotThrow()
    {
        // Arrange
        // Zero and one are both inclusive bounds for fraction settings.
        var low = new DiceVisualSettings { FaceLockEnergy = 0f, BevelThickness = 0f };
        var high = new DiceVisualSettings { FaceLockEnergy = 1f, BevelThickness = 1f };

        // Act
        low.Validate();
        high.Validate();

        // Assert
        Assert.AreEqual(0f, low.FaceLockEnergy);
        Assert.AreEqual(1f, high.FaceLockEnergy);
    }
}
