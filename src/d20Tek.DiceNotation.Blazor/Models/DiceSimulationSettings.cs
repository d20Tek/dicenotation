namespace d20Tek.DiceNotation.Blazor.Models;

/// <summary>
/// Provides the tunable physics values used by the dice simulation. All distances are
/// expressed in logical simulation units. The defaults were established by the dice tray
/// physics prototype and produce a settled scene within a few seconds.
/// </summary>
public sealed record DiceSimulationSettings
{
    /// <summary>Gets the thickness of the static tray walls.</summary>
    public float WallThickness { get; init; } = 0.5f;

    /// <summary>Gets the edge length of a square die.</summary>
    public float DieSize { get; init; } = 0.6f;

    /// <summary>Gets the density used for each dynamic die body.</summary>
    public float DieDensity { get; init; } = 1f;

    /// <summary>Gets the friction applied to dice and tray walls.</summary>
    public float Friction { get; init; } = 0.4f;

    /// <summary>Gets the restitution (bounciness) applied to dice and tray walls.</summary>
    public float Restitution { get; init; } = 0.35f;

    /// <summary>Gets the linear damping applied to each die.</summary>
    public float LinearDamping { get; init; } = 1.4f;

    /// <summary>Gets the angular damping applied to each die.</summary>
    public float AngularDamping { get; init; } = 1.8f;

    /// <summary>Gets the maximum initial linear speed applied when launching a die.</summary>
    public float MaxLaunchSpeed { get; init; } = 14f;

    /// <summary>Gets the maximum initial angular speed applied when launching a die.</summary>
    public float MaxLaunchSpin { get; init; } = 12f;

    /// <summary>Gets the fixed physics timestep, in seconds.</summary>
    public float FixedStep { get; init; } = 1f / 60f;

    /// <summary>
    /// Gets the maximum number of fixed physics steps performed during a single call to
    /// <c>Step</c>. This bounds catch-up work after a long pause, such as a backgrounded tab.
    /// </summary>
    public int MaxCatchUpSteps { get; init; } = 5;

    /// <summary>Gets the linear velocity at or below which a die is considered at rest.</summary>
    public float LinearSleepThreshold { get; init; } = 0.05f;

    /// <summary>Gets the angular velocity at or below which a die is considered at rest.</summary>
    public float AngularSleepThreshold { get; init; } = 0.1f;

    /// <summary>Gets how long every die must stay at rest before the scene is settled.</summary>
    public float SettleHoldSeconds { get; init; } = 0.35f;

    /// <summary>
    /// Gets the maximum duration of a roll, in seconds. When this elapses the simulation
    /// stops rather than leaving the component in a permanent rolling state.
    /// </summary>
    public float MaxDurationSeconds { get; init; } = 8f;

    /// <summary>Gets the maximum number of dice allowed in a single roll.</summary>
    public int MaxDiceCount { get; init; } = 50;

    /// <summary>
    /// Validates that every setting is within a usable range before it reaches the physics engine.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any setting is invalid.</exception>
    public void Validate()
    {
        EnsurePositive(WallThickness, nameof(WallThickness));
        EnsurePositive(DieSize, nameof(DieSize));
        EnsurePositive(DieDensity, nameof(DieDensity));
        EnsureNonNegative(Friction, nameof(Friction));
        EnsureNonNegative(Restitution, nameof(Restitution));
        EnsureNonNegative(LinearDamping, nameof(LinearDamping));
        EnsureNonNegative(AngularDamping, nameof(AngularDamping));
        EnsureNonNegative(MaxLaunchSpeed, nameof(MaxLaunchSpeed));
        EnsureNonNegative(MaxLaunchSpin, nameof(MaxLaunchSpin));
        EnsurePositive(FixedStep, nameof(FixedStep));
        EnsureNonNegative(LinearSleepThreshold, nameof(LinearSleepThreshold));
        EnsureNonNegative(AngularSleepThreshold, nameof(AngularSleepThreshold));
        EnsureNonNegative(SettleHoldSeconds, nameof(SettleHoldSeconds));
        EnsurePositive(MaxDurationSeconds, nameof(MaxDurationSeconds));

        ArgumentOutOfRangeException.ThrowIfLessThan(MaxCatchUpSteps, 1, nameof(MaxCatchUpSteps));
        ArgumentOutOfRangeException.ThrowIfLessThan(MaxDiceCount, 1, nameof(MaxDiceCount));
    }

    private static void EnsurePositive(float value, string name)
    {
        if (!float.IsFinite(value) || value <= 0f)
        {
            throw new ArgumentOutOfRangeException(name, value, $"{name} must be a finite, positive value.");
        }
    }

    private static void EnsureNonNegative(float value, string name)
    {
        if (!float.IsFinite(value) || value < 0f)
        {
            throw new ArgumentOutOfRangeException(name, value, $"{name} must be a finite, non-negative value.");
        }
    }
}
