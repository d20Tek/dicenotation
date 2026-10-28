using d20Tek.DiceNotation.Blazor.DependencyInjection;
using d20Tek.DiceNotation.Blazor.Services;
using d20Tek.DiceNotation.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering the visual dice roller services with an
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class DiceRollerServiceCollectionExtensions
{
    /// <summary>
    /// Registers everything the <c>VisualDiceRoller</c> component needs, including the
    /// core dice notation services, so a consumer only has to make one call.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configureRoller">
    /// An optional delegate used to configure the default tray, theme, presentation,
    /// physics, and reduced-motion settings shared by components that do not supply their
    /// own options.
    /// </param>
    /// <param name="configureDice">
    /// An optional delegate used to configure the core dice notation services.
    /// </param>
    /// <returns>The same service collection so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the configured roller options contain an invalid value.
    /// </exception>
    public static IServiceCollection AddDiceNotationBlazor(
        this IServiceCollection services,
        Action<DiceRollerOptionsBuilder>? configureRoller = null,
        Action<DiceNotationOptionsBuilder>? configureDice = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddDiceNotation(configureDice);

        var builder = new DiceRollerOptionsBuilder();
        configureRoller?.Invoke(builder);

        var options = builder.Build();
        services.AddSingleton(options);

        // The factory only depends on IJSRuntime, which is scoped in Blazor Server, so the
        // factory itself is scoped. Each component still receives its own loop instance.
        services.AddScoped<IDiceAnimationLoopFactory, DiceAnimationLoopFactory>();

        return services;
    }
}
