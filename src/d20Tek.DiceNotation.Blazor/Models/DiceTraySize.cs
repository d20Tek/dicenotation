namespace d20Tek.DiceNotation.Blazor.Models;

/// <summary>
/// Represents the logical dimensions of the dice tray used by the simulation.
/// These are simulation units, not device pixels.
/// </summary>
/// <param name="Width">The width of the tray.</param>
/// <param name="Height">The height of the tray.</param>
public sealed record DiceTraySize(float Width, float Height)
{
    /// <summary>
    /// Gets the default tray size used when a consumer does not supply one.
    /// </summary>
    public static DiceTraySize Default { get; } = new(8f, 6f);

    /// <summary>
    /// Validates that the tray dimensions are finite and positive.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when either dimension is not a finite, positive number.
    /// </exception>
    public void Validate()
    {
        EnsurePositiveAndFinite(Width, nameof(Width));
        EnsurePositiveAndFinite(Height, nameof(Height));
    }

    private static void EnsurePositiveAndFinite(float value, string name)
    {
        if (!float.IsFinite(value) || value <= 0f)
        {
            throw new ArgumentOutOfRangeException(name, value, "Tray dimensions must be finite, positive values.");
        }
    }
}
