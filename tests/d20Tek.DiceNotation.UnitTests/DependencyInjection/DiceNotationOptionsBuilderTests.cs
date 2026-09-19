using d20Tek.DiceNotation.DependencyInjection;
using d20Tek.DiceNotation.DieRoller;
using Microsoft.Extensions.DependencyInjection;

namespace d20Tek.DiceNotation.UnitTests.DependencyInjection;

[TestClass]
public class DiceNotationOptionsBuilderTests
{
    [TestMethod]
    public void UseDieRoller_WithNullFactory_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();

        // Act & Assert
        [ExcludeFromCodeCoverage]
        void Act() => builder.UseDieRoller(null!);

        Assert.ThrowsExactly<ArgumentNullException>(Act);
    }

    [TestMethod]
    public void WithDefaultDieSides_ReturnsSameBuilder()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();

        // Act
        var result = builder.WithDefaultDieSides(8);

        // Assert
        Assert.AreSame(builder, result);
    }

    [TestMethod]
    public void WithBoundedResult_ReturnsSameBuilder()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();

        // Act
        var result = builder.WithBoundedResult(false);

        // Assert
        Assert.AreSame(builder, result);
    }

    [TestMethod]
    public void UseRandomDieRoller_ReturnsSameBuilder()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();

        // Act
        var result = builder.UseRandomDieRoller();

        // Assert
        Assert.AreSame(builder, result);
    }

    [TestMethod]
    public void UseConstantDieRoller_ReturnsSameBuilder()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();

        // Act
        var result = builder.UseConstantDieRoller();

        // Assert
        Assert.AreSame(builder, result);
    }

    [TestMethod]
    public void UseRollTracker_ReturnsSameBuilder()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();

        // Act
        var result = builder.UseRollTracker();

        // Assert
        Assert.AreSame(builder, result);
    }

    [TestMethod]
    public void UseCustomRollTracker_RegistersCustomTrackerType()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(opt => opt.UseRollTracker<DieRollTracker, IDieRollTracker>());
        using var provider = services.BuildServiceProvider();

        // Assert
        Assert.IsInstanceOfType<DieRollTracker>(provider.GetService<IDieRollTracker>());
    }

    [TestMethod]
    public void WithoutBuilderConfiguration_UsesDefaultDieSides()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation();
        using var provider = services.BuildServiceProvider();
        var config = provider.GetRequiredService<IDiceConfiguration>();

        // Assert
        Assert.AreEqual(6, config.DefaultDieSides);
    }
}
