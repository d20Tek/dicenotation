using d20Tek.DiceNotation;
using d20Tek.DiceNotation.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering d20Tek.DiceNotation services with an
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class DiceServiceCollectionExtensions
{
    /// <summary>
    /// Registers the dice notation services, including <see cref="IDiceConfiguration"/>,
    /// <see cref="IDieRoller"/>, and <see cref="IDice"/>, with the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configure">
    /// An optional delegate used to configure dice behavior, the die roller, and roll tracking.
    /// </param>
    /// <param name="lifetime">
    /// The service lifetime used for the registered services. Defaults to
    /// <see cref="ServiceLifetime.Scoped"/>.
    /// </param>
    /// <returns>The same service collection so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddDiceNotation(
        this IServiceCollection services,
        Action<DiceNotationOptionsBuilder>? configure = null,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(services);

        var builder = new DiceNotationOptionsBuilder();
        configure?.Invoke(builder);

        builder.RegisterTracker(services, lifetime);

        services.Add(new ServiceDescriptor(typeof(IDieRoller), builder.BuildDieRoller, lifetime));
        services.Add(new ServiceDescriptor(typeof(IDiceConfiguration), builder.BuildConfiguration, lifetime));
        services.Add(new ServiceDescriptor(typeof(IDice), sp => new Dice(sp.GetRequiredService<IDiceConfiguration>()), lifetime));

        return services;
    }
}
