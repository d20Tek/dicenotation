namespace d20Tek.DiceNotation.Blazor.Models;

/// <summary>
/// Provides the configuration used by the <c>VisualDiceRoller</c> component. This groups
/// the tray, theme, presentation, and physics settings into a single object so the
/// component's parameter list stays small and can evolve without breaking consumers.
/// </summary>
/// <remarks>
/// Nothing in this type can influence a die result. Results are determined by the
/// notation library before an animation begins, so every value here affects appearance,
/// timing, or physics only.
/// </remarks>
public sealed record DiceRollerOptions
{
    /// <summary>Gets the logical dimensions of the dice tray.</summary>
    public DiceTraySize TraySize { get; init; } = DiceTraySize.Default;

    /// <summary>Gets the theme used to draw the tray and dice.</summary>
    public DiceTheme Theme { get; init; } = DiceTheme.Default;

    /// <summary>Gets the presentation values used to turn physical state into drawing instructions.</summary>
    public DiceVisualSettings VisualSettings { get; init; } = DiceVisualSettings.Default;

    /// <summary>Gets the physics values used by the dice simulation.</summary>
    public DiceSimulationSettings SimulationSettings { get; init; } = new();

    /// <summary>
    /// Gets a value indicating whether motion effects are disabled, which suits users who
    /// prefer reduced motion. When this is <see langword="true"/> the dice are drawn
    /// without face churn, implied height, or motion smear.
    /// </summary>
    /// <remarks>
    /// This is an explicit option rather than an automatic media-query check, because an
    /// application usually already knows its own accessibility preferences and can apply
    /// them consistently across its UI.
    /// </remarks>
    public bool ReducedMotion { get; init; }

    /// <summary>Gets a default instance.</summary>
    public static DiceRollerOptions Default { get; } = new();

    /// <summary>
    /// Gets the theme with the reduced-motion preference applied, which is the theme the
    /// renderer should actually draw with.
    /// </summary>
    /// <returns>The theme to draw with.</returns>
    public DiceTheme ResolveTheme() =>
        ReducedMotion ? Theme with { UseMotionEffects = false } : Theme;

    /// <summary>
    /// Validates every contained settings object so an invalid configuration is reported
    /// before any simulation or interop work begins.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when a contained settings object is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a theme color value is missing.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any setting is out of range.</exception>
    public void Validate()
    {
        ArgumentNullException.ThrowIfNull(TraySize);
        ArgumentNullException.ThrowIfNull(Theme);
        ArgumentNullException.ThrowIfNull(VisualSettings);
        ArgumentNullException.ThrowIfNull(SimulationSettings);

        TraySize.Validate();
        Theme.Validate();
        VisualSettings.Validate();
        SimulationSettings.Validate();
    }
}
