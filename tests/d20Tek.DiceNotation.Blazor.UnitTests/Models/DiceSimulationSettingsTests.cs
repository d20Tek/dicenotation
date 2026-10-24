namespace d20Tek.DiceNotation.Blazor.UnitTests.Models;

[TestClass]
public class DiceSimulationSettingsTests
{
    [TestMethod]
    public void Validate_WithDefaults_DoesNotThrow()
    {
        // Arrange
        var settings = new DiceSimulationSettings();

        // Act
        settings.Validate();

        // Assert
        Assert.IsGreaterThan(0f, settings.FixedStep);
        Assert.IsGreaterThanOrEqualTo(1, settings.MaxCatchUpSteps);
        Assert.IsGreaterThanOrEqualTo(1, settings.MaxDiceCount);
    }

    [TestMethod]
    public void Validate_WithCustomValidValues_DoesNotThrow()
    {
        // Arrange
        var settings = new DiceSimulationSettings
        {
            Friction = 0f,
            Restitution = 0f,
            LinearDamping = 0f,
            AngularDamping = 0f,
            MaxLaunchSpeed = 0f,
            MaxLaunchSpin = 0f,
            LinearSleepThreshold = 0f,
            AngularSleepThreshold = 0f,
            SettleHoldSeconds = 0f,
            MaxCatchUpSteps = 1,
            MaxDiceCount = 1,
        };

        // Act
        settings.Validate();

        // Assert
        Assert.AreEqual(0f, settings.Friction);
        Assert.AreEqual(1, settings.MaxCatchUpSteps);
    }

    [TestMethod]
    [DataRow(0f)]
    [DataRow(-0.5f)]
    [DataRow(float.NaN)]
    public void Validate_WithNonPositiveWallThickness_ThrowsArgumentOutOfRangeException(float value)
    {
        // Arrange
        var settings = new DiceSimulationSettings { WallThickness = value };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNonPositiveDieSize_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { DieSize = 0f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNonPositiveDieDensity_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { DieDensity = -1f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNegativeFriction_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { Friction = -0.1f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNegativeRestitution_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { Restitution = -0.1f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNegativeLinearDamping_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { LinearDamping = -1f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNegativeAngularDamping_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { AngularDamping = -1f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNegativeMaxLaunchSpeed_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { MaxLaunchSpeed = -1f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNegativeMaxLaunchSpin_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { MaxLaunchSpin = -1f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNonPositiveFixedStep_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { FixedStep = 0f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNegativeLinearSleepThreshold_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { LinearSleepThreshold = -1f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNegativeAngularSleepThreshold_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { AngularSleepThreshold = -1f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNegativeSettleHoldSeconds_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { SettleHoldSeconds = -0.1f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithNonPositiveMaxDurationSeconds_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { MaxDurationSeconds = 0f };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithMaxCatchUpStepsBelowOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { MaxCatchUpSteps = 0 };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }

    [TestMethod]
    public void Validate_WithMaxDiceCountBelowOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new DiceSimulationSettings { MaxDiceCount = 0 };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => settings.Validate();
    }
}
