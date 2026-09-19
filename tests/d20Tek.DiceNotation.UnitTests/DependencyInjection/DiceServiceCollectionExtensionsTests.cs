using d20Tek.DiceNotation.DieRoller;
using Microsoft.Extensions.DependencyInjection;

namespace d20Tek.DiceNotation.UnitTests.DependencyInjection;

[TestClass]
public class DiceServiceCollectionExtensionsTests
{
    [TestMethod]
    public void AddDiceNotation_WithNullServices_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceCollection services = null!;

        // Act & Assert
        [ExcludeFromCodeCoverage]
        void Act() => services.AddDiceNotation();

        Assert.ThrowsExactly<ArgumentNullException>(Act);
    }

    [TestMethod]
    public void AddDiceNotation_WithNoConfiguration_RegistersCoreServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation();
        using var provider = services.BuildServiceProvider();

        // Assert
        Assert.IsNotNull(provider.GetService<IDice>());
        Assert.IsNotNull(provider.GetService<IDiceConfiguration>());
        Assert.IsNotNull(provider.GetService<IDieRoller>());
    }

    [TestMethod]
    public void AddDiceNotation_WithNoConfiguration_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddDiceNotation();

        // Assert
        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddDiceNotation_WithDefaults_UsesRandomDieRoller()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation();
        using var provider = services.BuildServiceProvider();
        var roller = provider.GetRequiredService<IDieRoller>();

        // Assert
        Assert.IsInstanceOfType<RandomDieRoller>(roller);
    }

    [TestMethod]
    public void AddDiceNotation_WithDefaults_RegistersServicesAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation();

        // Assert
        var descriptor = services.Single(d => d.ServiceType == typeof(IDice));
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDiceNotation_WithSingletonLifetime_RegistersServicesAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(lifetime: ServiceLifetime.Singleton);

        // Assert
        Assert.IsTrue(services.All(d => d.Lifetime == ServiceLifetime.Singleton));
    }

    [TestMethod]
    public void AddDiceNotation_WithSingletonLifetime_ResolvesSameDiceInstance()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(lifetime: ServiceLifetime.Singleton);
        using var provider = services.BuildServiceProvider();
        var first = provider.GetRequiredService<IDice>();
        var second = provider.GetRequiredService<IDice>();

        // Assert
        Assert.AreSame(first, second);
    }

    [TestMethod]
    public void AddDiceNotation_WithConfiguredDieSides_AppliesToConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(opt => opt.WithDefaultDieSides(10));
        using var provider = services.BuildServiceProvider();
        var config = provider.GetRequiredService<IDiceConfiguration>();

        // Assert
        Assert.AreEqual(10, config.DefaultDieSides);
    }

    [TestMethod]
    public void AddDiceNotation_WithBoundedResult_AppliesToConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(opt => opt.WithBoundedResult(true, 5));
        using var provider = services.BuildServiceProvider();
        var config = provider.GetRequiredService<IDiceConfiguration>();

        // Assert
        Assert.IsTrue(config.HasBoundedResult);
        Assert.AreEqual(5, config.BoundedResultMinimum);
    }

    [TestMethod]
    public void AddDiceNotation_WithoutTracker_DoesNotRegisterTracker()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation();
        using var provider = services.BuildServiceProvider();

        // Assert
        Assert.IsNull(provider.GetService<IAllowRollTrackerEntry>());
        Assert.IsNull(provider.GetService<IDieRollTracker>());
    }

    [TestMethod]
    public void AddDiceNotation_WithRollTracker_RegistersDieRollTracker()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(opt => opt.UseRollTracker());
        using var provider = services.BuildServiceProvider();

        // Assert
        Assert.IsInstanceOfType<DieRollTracker>(provider.GetService<IDieRollTracker>());
        Assert.IsInstanceOfType<DieRollTracker>(provider.GetService<IAllowRollTrackerEntry>());
    }

    [TestMethod]
    public void AddDiceNotation_WithRollTracker_SharesTrackerInstanceWithRoller()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(opt => opt.UseRollTracker());
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var tracker = scope.ServiceProvider.GetRequiredService<IDieRollTracker>();
        var entry = scope.ServiceProvider.GetRequiredService<IAllowRollTrackerEntry>();

        // Assert
        Assert.AreSame(tracker, entry);
    }

    [TestMethod]
    public async Task AddDiceNotation_WithRollTracker_RecordsRollsFromResolvedDice()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDiceNotation(opt => opt.UseRandomDieRoller().UseRollTracker());
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var dice = scope.ServiceProvider.GetRequiredService<IDice>();
        var tracker = scope.ServiceProvider.GetRequiredService<IDieRollTracker>();

        // Act
        dice.Roll("3d6");
        var data = await tracker.GetTrackingDataAsync();

        // Assert
        Assert.HasCount(3, data);
    }

    [TestMethod]
    public void AddDiceNotation_WithAggregateRollTracker_RegistersAggregateTracker()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(opt => opt.UseAggregateRollTracker());
        using var provider = services.BuildServiceProvider();

        // Assert
        Assert.IsInstanceOfType<AggregateRollTracker>(provider.GetService<IAggregateRollTracker>());
        Assert.IsInstanceOfType<AggregateRollTracker>(provider.GetService<IAllowRollTrackerEntry>());
    }

    [TestMethod]
    public void AddDiceNotation_WithConstantDieRoller_RollsDeterministically()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(opt => opt.UseConstantDieRoller(3));
        using var provider = services.BuildServiceProvider();
        var dice = provider.GetRequiredService<IDice>();
        var result = dice.Roll("3d6");

        // Assert
        Assert.AreEqual(9, result.Value);
    }

    [TestMethod]
    public void AddDiceNotation_WithCryptoDieRoller_RegistersCryptoRoller()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(opt => opt.UseCryptoDieRoller());
        using var provider = services.BuildServiceProvider();

        // Assert
        Assert.IsInstanceOfType<CryptoDieRoller>(provider.GetRequiredService<IDieRoller>());
    }

    [TestMethod]
    public void AddDiceNotation_WithMathNetDieRoller_RegistersMathNetRoller()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(opt => opt.UseMathNetDieRoller());
        using var provider = services.BuildServiceProvider();

        // Assert
        Assert.IsInstanceOfType<MathNetDieRoller>(provider.GetRequiredService<IDieRoller>());
    }

    [TestMethod]
    public void AddDiceNotation_WithRandomDieRoller_RegistersRandomRoller()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDiceNotation(opt => opt.UseRandomDieRoller());
        using var provider = services.BuildServiceProvider();

        // Assert
        Assert.IsInstanceOfType<RandomDieRoller>(provider.GetRequiredService<IDieRoller>());
    }

    [TestMethod]
    public void AddDiceNotation_WithCustomDieRoller_RegistersCustomRoller()
    {
        // Arrange
        var services = new ServiceCollection();
        var custom = new ConstantDieRoller(6);

        // Act
        services.AddDiceNotation(opt => opt.UseDieRoller(_ => custom));
        using var provider = services.BuildServiceProvider();

        // Assert
        Assert.AreSame(custom, provider.GetRequiredService<IDieRoller>());
    }
}
