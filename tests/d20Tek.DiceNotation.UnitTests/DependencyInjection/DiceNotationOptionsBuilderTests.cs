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

    [TestMethod]
    public void UseSeededDieRoller_ReturnsSameBuilder()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();

        // Act
        var result = builder.UseSeededDieRoller(42);

        // Assert
        Assert.AreSame(builder, result);
    }

    [TestMethod]
    public void UseSeededMathNetDieRoller_ReturnsSameBuilder()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();

        // Act
        var result = builder.UseSeededMathNetDieRoller(42);

        // Assert
        Assert.AreSame(builder, result);
    }

    [TestMethod]
    public void UseReplayDieRoller_ReturnsSameBuilder()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();
        var token = new ReplayToken(DieRollerAlgorithm.Random, 42);

        // Act
        var result = builder.UseReplayDieRoller(token);

        // Assert
        Assert.AreSame(builder, result);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void UseReplayDieRoller_WithNullToken_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => builder.UseReplayDieRoller(null!));
    }

    [TestMethod]
    public void UseSeededDieRoller_RegistersSeededRandomRoller()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(opt => opt.UseSeededDieRoller(42));
        using var provider = services.BuildServiceProvider();
        var roller = provider.GetRequiredService<IDieRoller>();

        // Assert
        Assert.IsInstanceOfType<RandomDieRoller>(roller);
        Assert.AreEqual(42, ((IReplayableDieRoller)roller).ReplayToken!.Seed);
    }

    [TestMethod]
    public void UseSeededMathNetDieRoller_RegistersSeededMathNetRoller()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(opt => opt.UseSeededMathNetDieRoller(42));
        using var provider = services.BuildServiceProvider();
        var roller = provider.GetRequiredService<IDieRoller>();

        // Assert
        Assert.IsInstanceOfType<MathNetDieRoller>(roller);
        Assert.AreEqual(42, ((IReplayableDieRoller)roller).ReplayToken!.Seed);
    }

    [TestMethod]
    public void UseReplayDieRoller_RegistersRollerFromToken()
    {
        // Arrange
        var services = new ServiceCollection();
        var token = new ReplayToken(DieRollerAlgorithm.MathNetMersenneTwister, 77);

        // Act
        services.AddDiceNotation(opt => opt.UseReplayDieRoller(token));
        using var provider = services.BuildServiceProvider();
        var roller = provider.GetRequiredService<IDieRoller>();

        // Assert
        Assert.IsInstanceOfType<MathNetDieRoller>(roller);
        Assert.AreEqual(token, ((IReplayableDieRoller)roller).ReplayToken);
    }
}
