using d20Tek.DiceNotation.DependencyInjection;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.UnitTests.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace d20Tek.DiceNotation.UnitTests.DependencyInjection;

public partial class DiceNotationOptionsBuilderTests
{
    private const int _replaySeed = 77;

    private static readonly ReplayToken _externalToken =
        new(FakeSeededDieRollerProvider.ExternalAlgorithm, _replaySeed);

    [TestMethod]
    public void AddSeededDieRollerProvider_ReturnsSameBuilder()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();

        // Act
        var result = builder.AddSeededDieRollerProvider(new FakeSeededDieRollerProvider());

        // Assert
        Assert.AreSame(builder, result);
    }

    [TestMethod]
    public void AddSeededDieRollerProvider_WithNullProvider_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = new DiceNotationOptionsBuilder();

        // Act & Assert
        [ExcludeFromCodeCoverage]
        void Act() => builder.AddSeededDieRollerProvider(null!);

        Assert.ThrowsExactly<ArgumentNullException>(Act);
    }

    [TestMethod]
    public void AddDiceNotation_WithoutSeededProviders_RegistersNoProviders()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation();

        // Assert
        Assert.IsEmpty(services.Where(d => d.ServiceType == typeof(ISeededDieRollerProvider)));
    }

    [TestMethod]
    public void AddSeededDieRollerProvider_RegistersProviderAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        var seededProvider = new FakeSeededDieRollerProvider();

        // Act
        services.AddDiceNotation(opt => opt.AddSeededDieRollerProvider(seededProvider));

        // Assert
        var descriptor = services.Single(d => d.ServiceType == typeof(ISeededDieRollerProvider));
        Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.AreSame(seededProvider, descriptor.ImplementationInstance);
    }

    [TestMethod]
    public void AddSeededDieRollerProvider_CalledTwice_RegistersBothProviders()
    {
        // Arrange
        var services = new ServiceCollection();
        var first = new FakeSeededDieRollerProvider();
        var second = new FakeSeededDieRollerProvider(DieRollerAlgorithm.Random);

        // Act
        services.AddDiceNotation(opt => opt.AddSeededDieRollerProvider(first).AddSeededDieRollerProvider(second));
        using var provider = services.BuildServiceProvider();
        var registered = provider.GetServices<ISeededDieRollerProvider>().ToList();

        // Assert
        Assert.AreSequenceEqual([first, second], registered);
    }

    [TestMethod]
    public void UseReplayDieRoller_WithBuilderProvider_CreatesRollerThroughProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var seededProvider = new FakeSeededDieRollerProvider();

        // Act
        services.AddDiceNotation(opt => opt.AddSeededDieRollerProvider(seededProvider).UseReplayDieRoller(_externalToken));
        using var provider = services.BuildServiceProvider();
        var roller = provider.GetRequiredService<IDieRoller>();

        // Assert
        Assert.IsInstanceOfType<RandomDieRoller>(roller);
        Assert.AreEqual(1, seededProvider.CreateCount);
    }

    [TestMethod]
    public void UseReplayDieRoller_WithDirectlyRegisteredProvider_CreatesRollerThroughProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var seededProvider = new FakeSeededDieRollerProvider();
        services.AddSingleton<ISeededDieRollerProvider>(seededProvider);

        // Act
        services.AddDiceNotation(opt => opt.UseReplayDieRoller(_externalToken));
        using var provider = services.BuildServiceProvider();
        var roller = provider.GetRequiredService<IDieRoller>();

        // Assert
        Assert.IsInstanceOfType<RandomDieRoller>(roller);
        Assert.AreEqual(1, seededProvider.CreateCount);
    }

    [TestMethod]
    public void UseReplayDieRoller_WithRollTracker_PassesTrackerToProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var seededProvider = new FakeSeededDieRollerProvider();
        services.AddDiceNotation(opt => opt
            .UseRollTracker()
            .AddSeededDieRollerProvider(seededProvider)
            .UseReplayDieRoller(_externalToken));
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        // Act
        scope.ServiceProvider.GetRequiredService<IDieRoller>();

        // Assert
        Assert.AreSame(scope.ServiceProvider.GetRequiredService<IAllowRollTrackerEntry>(), seededProvider.LastTracker);
    }

    [TestMethod]
    public void UseReplayDieRoller_WithUnregisteredAlgorithm_ThrowsOnResolve()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDiceNotation(opt => opt.UseReplayDieRoller(_externalToken));
        using var provider = services.BuildServiceProvider();

        // Act & Assert
        [ExcludeFromCodeCoverage]
        void Act() => provider.GetRequiredService<IDieRoller>();

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);
    }
}
