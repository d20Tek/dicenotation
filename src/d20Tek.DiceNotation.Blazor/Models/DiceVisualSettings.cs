namespace d20Tek.DiceNotation.Blazor.Models;

/// <summary>
/// Provides the tunable presentation values used to turn a die's physical state into
/// drawing instructions. These values previously lived as constants inside the Canvas
/// renderer; keeping them in C# makes the resulting visuals testable and configurable.
/// </summary>
/// <remarks>
/// Nothing here can influence a die result. Every value affects appearance only.
/// </remarks>
public sealed record DiceVisualSettings
{
    /// <summary>
    /// Gets the energy above which a tumbling die shows churning faces. At or below this
    /// value the die always shows its real, precomputed result. The threshold is high
    /// enough that dice lock well before they come to rest, so a churned face can never
    /// be mistaken for the result.
    /// </summary>
    public float FaceLockEnergy { get; init; } = 0.18f;

    /// <summary>
    /// Gets the direction light arrives from, in canvas radians. Shading is counter-rotated
    /// by each die's rotation so the highlight stays fixed in the scene.
    /// </summary>
    public float LightAngle { get; init; } = -MathF.PI * 0.75f;

    /// <summary>
    /// Gets the additional scale applied to a die at full energy, which reads as height
    /// above the tray. A value of 0.18 draws a die in flight 18 percent larger.
    /// </summary>
    public float MaxLift { get; init; } = 0.18f;

    /// <summary>Gets the shadow blur applied to a die at rest, in logical units.</summary>
    public float BaseShadowBlur { get; init; } = 0.12f;

    /// <summary>Gets the additional shadow blur applied to a die at full energy.</summary>
    public float EnergyShadowBlur { get; init; } = 0.35f;

    /// <summary>Gets the shadow offset applied to a die at rest, in logical units.</summary>
    public float BaseShadowOffset { get; init; } = 0.05f;

    /// <summary>Gets the additional shadow offset applied to a die at full energy.</summary>
    public float EnergyShadowOffset { get; init; } = 0.3f;

    /// <summary>Gets the energy above which a motion smear is drawn behind a die.</summary>
    public float SmearEnergy { get; init; } = 0.25f;

    /// <summary>Gets the number of fading copies drawn in a motion smear.</summary>
    public int SmearSteps { get; init; } = 3;

    /// <summary>
    /// Gets the fraction of the die's larger dimension that a full-energy smear trails behind it.
    /// </summary>
    public float SmearReach { get; init; } = 0.5f;

    /// <summary>Gets the fraction of the die's smaller dimension used as bevel thickness.</summary>
    public float BevelThickness { get; init; } = 0.1f;

    /// <summary>Gets a default instance.</summary>
    public static DiceVisualSettings Default { get; } = new();

    /// <summary>
    /// Validates that every setting is within a usable range.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any setting is invalid.</exception>
    public void Validate()
    {
        EnsureFraction(FaceLockEnergy, nameof(FaceLockEnergy));
        EnsureFraction(SmearEnergy, nameof(SmearEnergy));
        EnsureFinite(LightAngle, nameof(LightAngle));
        EnsureNonNegative(MaxLift, nameof(MaxLift));
        EnsureNonNegative(BaseShadowBlur, nameof(BaseShadowBlur));
        EnsureNonNegative(EnergyShadowBlur, nameof(EnergyShadowBlur));
        EnsureNonNegative(BaseShadowOffset, nameof(BaseShadowOffset));
        EnsureNonNegative(EnergyShadowOffset, nameof(EnergyShadowOffset));
        EnsureNonNegative(SmearReach, nameof(SmearReach));
        EnsureFraction(BevelThickness, nameof(BevelThickness));

        ArgumentOutOfRangeException.ThrowIfLessThan(SmearSteps, 1, nameof(SmearSteps));
    }

    private static void EnsureFinite(float value, string name)
    {
        if (!float.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(name, value, $"{name} must be a finite value.");
        }
    }

    private static void EnsureNonNegative(float value, string name)
    {
        if (!float.IsFinite(value) || value < 0f)
        {
            throw new ArgumentOutOfRangeException(name, value, $"{name} must be a finite, non-negative value.");
        }
    }

    private static void EnsureFraction(float value, string name)
    {
        if (!float.IsFinite(value) || value < 0f || value > 1f)
        {
            throw new ArgumentOutOfRangeException(name, value, $"{name} must be between 0 and 1.");
        }
    }
}
