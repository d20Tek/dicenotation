using d20Tek.DiceNotation.Blazor.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace d20Tek.DiceNotation.Blazor.UnitTests.DependencyInjection;

[TestClass]
public class DiceRollerServiceCollectionExtensionsTests
{
    [TestMethod]
    public void AddDiceNotationBlazor_WithNullServices_ThrowsArgumentNullException()
    {
        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        static void Act() => _ = ((IServiceCollection)null!).AddDiceNotationBlazor();
    }

    [TestMethod]
    public void AddDiceNotationBlazor_WithoutConfiguration_RegistersCoreDiceServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotationBlazor();

        // Assert
        using var provider = services.BuildServiceProvider();
        Assert.IsNotNull(provider.GetService<IDice>());
    }

    [TestMethod]
    public void AddDiceNotationBlazor_WithoutConfiguration_RegistersDefaultOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotationBlazor();

        // Assert
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<DiceRollerOptions>();
        Assert.IsFalse(options.ReducedMotion);
        Assert.AreEqual(DiceTraySize.Default, options.TraySize);
    }

    [TestMethod]
    public void AddDiceNotationBlazor_WithRollerConfiguration_AppliesConfiguredValues()
    {
        // Arrange
        var services = new ServiceCollection();
        var traySize = new DiceTraySize(12f, 9f);

        // Act
        services.AddDiceNotationBlazor(roller =>
        {
            roller.TraySize = traySize;
            roller.ReducedMotion = true;
        });

        // Assert
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<DiceRollerOptions>();
        Assert.AreEqual(traySize, options.TraySize);
        Assert.IsTrue(options.ReducedMotion);
    }

    [TestMethod]
    public void AddDiceNotationBlazor_WithDiceConfiguration_AppliesConfiguredRoller()
    {
        // Arrange
        var services = new ServiceCollection();
        var roller = new SequenceDieRoller([3]);

        // Act
        services.AddDiceNotationBlazor(configureDice: dice => dice.UseDieRoller(_ => roller));

        // Assert
        using var provider = services.BuildServiceProvider();
        Assert.AreSame(roller, provider.GetRequiredService<IDieRoller>());
    }

    [TestMethod]
    public void AddDiceNotationBlazor_WithInvalidRollerConfiguration_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => services.AddDiceNotationBlazor(r => r.TraySize = new DiceTraySize(0f, 5f));
    }

    [TestMethod]
    public void AddDiceNotationBlazor_WhenRegistered_ResolvesAnimationLoopFactory()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IJSRuntime>(new FakeJSRuntime());

        // Act
        services.AddDiceNotationBlazor();

        // Assert
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        Assert.IsNotNull(scope.ServiceProvider.GetService<IDiceAnimationLoopFactory>());
    }

    [TestMethod]
    public void AddDiceNotationBlazor_WhenCalled_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddDiceNotationBlazor();

        // Assert
        Assert.AreSame(services, result);
    }
}

[TestClass]
public class DiceRollerOptionsBuilderTests
{
    [TestMethod]
    public void Build_WithDefaults_ReturnsDefaultEquivalentOptions()
    {
        // Arrange
        var builder = new DiceRollerOptionsBuilder();

        // Act
        var result = builder.Build();

        // Assert
        Assert.AreEqual(DiceTraySize.Default, result.TraySize);
        Assert.AreEqual(DiceVisualSettings.Default, result.VisualSettings);
        Assert.IsFalse(result.ReducedMotion);
    }

    [TestMethod]
    public void Build_WithAllValuesSet_CopiesEveryValue()
    {
        // Arrange
        var traySize = new DiceTraySize(14f, 10f);
        var theme = DiceTheme.Default;
        var visuals = DiceVisualSettings.Default;
        var simulation = new DiceSimulationSettings();
        var builder = new DiceRollerOptionsBuilder
        {
            TraySize = traySize,
            Theme = theme,
            VisualSettings = visuals,
            SimulationSettings = simulation,
            ReducedMotion = true,
        };

        // Act
        var result = builder.Build();

        // Assert
        Assert.AreEqual(traySize, result.TraySize);
        Assert.AreSame(theme, result.Theme);
        Assert.AreSame(visuals, result.VisualSettings);
        Assert.AreSame(simulation, result.SimulationSettings);
        Assert.IsTrue(result.ReducedMotion);
    }

    [TestMethod]
    public void Build_WithNullTheme_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = new DiceRollerOptionsBuilder { Theme = null! };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => builder.Build();
    }
}
