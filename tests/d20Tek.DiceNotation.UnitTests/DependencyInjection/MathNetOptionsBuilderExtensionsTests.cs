using d20Tek.DiceNotation.DependencyInjection;
using d20Tek.DiceNotation.DieRoller;
using Microsoft.Extensions.DependencyInjection;

namespace d20Tek.DiceNotation.UnitTests.DependencyInjection;

[TestClass]
public class MathNetOptionsBuilderExtensionsTests
{
    private const int _seed = 77;

    private static readonly ReplayToken _mathNetToken = new(DieRollerAlgorithm.MathNetMersenneTwister, _seed);

    [TestMethod]
    public void UseMathNetDieRoller_WithNullBuilder_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        static void Act() => MathNetOptionsBuilderExtensions.UseMathNetDieRoller(null!);
    }

    [TestMethod]
    public void UseSeededMathNetDieRoller_WithNullBuilder_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        static void Act() => MathNetOptionsBuilderExtensions.UseSeededMathNetDieRoller(null!, _seed);
    }

    [TestMethod]
    public void AddMathNetSeededDieRollerProvider_WithNullBuilder_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        static void Act() => MathNetOptionsBuilderExtensions.AddMathNetSeededDieRollerProvider(null!);
    }

    [TestMethod]
    public void UseMathNetDieRoller_ReturnsSameBuilder()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();

        // Act
        var result = builder.UseMathNetDieRoller();

        // Assert
        Assert.AreSame(builder, result);
    }

    [TestMethod]
    public void AddMathNetSeededDieRollerProvider_ReturnsSameBuilder()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();

        // Act
        var result = builder.AddMathNetSeededDieRollerProvider();

        // Assert
        Assert.AreSame(builder, result);
    }

    [TestMethod]
    public void AddMathNetSeededDieRollerProvider_RegistersMathNetProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(opt => opt.AddMathNetSeededDieRollerProvider());
        using var provider = services.BuildServiceProvider();

        // Assert
        var registered = provider.GetServices<ISeededDieRollerProvider>().Single();
        Assert.IsInstanceOfType<MathNetSeededDieRollerProvider>(registered);
    }

    [TestMethod]
    public void UseReplayDieRoller_WithMathNetTokenAndProvider_ResolvesSeededMathNetRoller()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDiceNotation(opt => opt.AddMathNetSeededDieRollerProvider().UseReplayDieRoller(_mathNetToken));
        using var provider = services.BuildServiceProvider();

        // Act
        var roller = provider.GetRequiredService<IDieRoller>();

        // Assert
        Assert.IsInstanceOfType<MathNetDieRoller>(roller);
        Assert.AreEqual(_mathNetToken, ((IReplayableDieRoller)roller).ReplayToken);
    }

    [TestMethod]
    public void UseReplayDieRoller_WithMathNetTokenAndNoProvider_ThrowsOnResolve()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDiceNotation(opt => opt.UseReplayDieRoller(_mathNetToken));
        using var provider = services.BuildServiceProvider();

        // Act & Assert
        [ExcludeFromCodeCoverage]
        void Act() => provider.GetRequiredService<IDieRoller>();

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);
    }
}
