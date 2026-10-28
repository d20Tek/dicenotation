namespace D20Tek.DiceNotation.PhysicsSpike;

/// <summary>
/// Renderable state of a single die at one moment in the simulation.
/// </summary>
internal readonly record struct DieSnapshot(
    string Id,
    int Value,
    float X,
    float Y,
    float Rotation);

/// <summary>
/// Snapshot of the whole spike scene at one moment.
/// </summary>
internal readonly record struct SceneSnapshot(
    IReadOnlyList<DieSnapshot> Dice,
    float ElapsedSeconds,
    bool IsSettled);
