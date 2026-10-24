namespace d20Tek.DiceNotation.Blazor.Models;

/// <summary>
/// Represents resolved style data used by the Canvas renderer. All values are CSS color
/// strings so the renderer can apply them directly without any styling logic of its own.
/// </summary>
public sealed record DiceTheme
{
    /// <summary>Gets the name of the theme.</summary>
    public string Name { get; init; } = "Default";

    /// <summary>Gets the fill color of the dice tray.</summary>
    public string TrayColor { get; init; } = "#1f2933";

    /// <summary>Gets the color of the tray border.</summary>
    public string TrayBorderColor { get; init; } = "#3e4c59";

    /// <summary>Gets the corner radius of the tray, in logical units.</summary>
    public float TrayCornerRadius { get; init; } = 0.4f;

    /// <summary>Gets the fill color of each die.</summary>
    public string DieColor { get; init; } = "#f5f7fa";

    /// <summary>Gets the border color of each die.</summary>
    public string DieBorderColor { get; init; } = "#cbd2d9";

    /// <summary>Gets the color used to draw the value on each die.</summary>
    public string DieTextColor { get; init; } = "#1f2933";

    /// <summary>Gets the color of the shadow cast by each die.</summary>
    public string DieShadowColor { get; init; } = "rgba(0, 0, 0, 0.35)";

    /// <summary>Gets the corner radius of each die, in logical units.</summary>
    public float DieCornerRadius { get; init; } = 0.1f;

    /// <summary>Gets the default light theme.</summary>
    public static DiceTheme Default { get; } = new();

    /// <summary>Gets a high-contrast dark theme.</summary>
    public static DiceTheme DarkFantasy { get; } = new()
    {
        Name = "DarkFantasy",
        TrayColor = "#14110f",
        TrayBorderColor = "#7f5539",
        DieColor = "#2b2118",
        DieBorderColor = "#b08968",
        DieTextColor = "#f0e6d2",
        DieShadowColor = "rgba(0, 0, 0, 0.55)",
    };

    /// <summary>Gets a bright theme suited to light application backgrounds.</summary>
    public static DiceTheme Parchment { get; } = new()
    {
        Name = "Parchment",
        TrayColor = "#efe3c8",
        TrayBorderColor = "#a98b5d",
        DieColor = "#fffaf0",
        DieBorderColor = "#a98b5d",
        DieTextColor = "#44372a",
        DieShadowColor = "rgba(68, 55, 42, 0.3)",
    };

    /// <summary>
    /// Validates that the theme contains usable color values and non-negative radii.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when a color value is missing.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a corner radius is invalid.</exception>
    public void Validate()
    {
        EnsureColor(TrayColor, nameof(TrayColor));
        EnsureColor(TrayBorderColor, nameof(TrayBorderColor));
        EnsureColor(DieColor, nameof(DieColor));
        EnsureColor(DieBorderColor, nameof(DieBorderColor));
        EnsureColor(DieTextColor, nameof(DieTextColor));
        EnsureColor(DieShadowColor, nameof(DieShadowColor));
        EnsureRadius(TrayCornerRadius, nameof(TrayCornerRadius));
        EnsureRadius(DieCornerRadius, nameof(DieCornerRadius));
    }

    private static void EnsureColor(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{name} must be a non-empty color value.", name);
        }
    }

    private static void EnsureRadius(float value, string name)
    {
        if (!float.IsFinite(value) || value < 0f)
        {
            throw new ArgumentOutOfRangeException(name, value, $"{name} must be a finite, non-negative value.");
        }
    }
}
