using d20Tek.DiceNotation.Blazor.Models;

namespace d20Tek.DiceNotation.Blazor.DependencyInjection;

/// <summary>
/// Builds the default <see cref="DiceRollerOptions"/> used by components that do not
/// supply their own options.
/// </summary>
/// <remarks>
/// <see cref="DiceRollerOptions"/> is an immutable record, so this mutable builder gives
/// registration code a simple assignment style without exposing settable state to the
/// components that later read the options.
/// </remarks>
public sealed class DiceRollerOptionsBuilder
{
    /// <summary>Gets or sets the logical dimensions of the dice tray.</summary>
    public DiceTraySize TraySize { get; set; } = DiceTraySize.Default;

    /// <summary>Gets or sets the theme used to draw the tray and dice.</summary>
    public DiceTheme Theme { get; set; } = DiceTheme.Default;

    /// <summary>Gets or sets the presentation values used when drawing dice.</summary>
    public DiceVisualSettings VisualSettings { get; set; } = DiceVisualSettings.Default;

    /// <summary>Gets or sets the physics values used by the dice simulation.</summary>
    public DiceSimulationSettings SimulationSettings { get; set; } = new();

    /// <summary>Gets or sets a value indicating whether motion effects are disabled.</summary>
    public bool ReducedMotion { get; set; }

    /// <summary>Creates the validated options described by this builder.</summary>
    /// <returns>The options to register as the application-wide defaults.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a settings object is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a theme color value is missing.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any setting is out of range.</exception>
    public DiceRollerOptions Build()
    {
        var options = new DiceRollerOptions
        {
            TraySize = TraySize,
            Theme = Theme,
            VisualSettings = VisualSettings,
            SimulationSettings = SimulationSettings,
            ReducedMotion = ReducedMotion,
        };

        // Failing during registration is far easier to diagnose than failing later inside
        // a component's first render.
        options.Validate();

        return options;
    }
}
