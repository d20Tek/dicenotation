namespace D20Tek.DiceNotation.PhysicsSpike;

/// <summary>
/// Tunable values for the dice tray physics spike. All dimensions are in logical
/// simulation units (not device pixels).
/// </summary>
internal sealed record SpikeSettings
{
    public float TrayWidth { get; init; } = 8f;

    public float TrayHeight { get; init; } = 6f;

    public float WallThickness { get; init; } = 0.5f;

    public float DieSize { get; init; } = 0.6f;

    public float DieDensity { get; init; } = 1f;

    public float Friction { get; init; } = 0.4f;

    public float Restitution { get; init; } = 0.35f;

    public float LinearDamping { get; init; } = 0.9f;

    public float AngularDamping { get; init; } = 1.2f;

    public float MaxLaunchSpeed { get; init; } = 14f;

    public float MaxLaunchSpin { get; init; } = 12f;

    public float FixedStep { get; init; } = 1f / 60f;

    public float LinearSleepThreshold { get; init; } = 0.05f;

    public float AngularSleepThreshold { get; init; } = 0.1f;

    public float SettleHoldSeconds { get; init; } = 0.35f;

    public float MaxDurationSeconds { get; init; } = 8f;
}
