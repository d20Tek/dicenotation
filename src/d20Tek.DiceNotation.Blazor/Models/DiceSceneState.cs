namespace d20Tek.DiceNotation.Blazor.Models;

/// <summary>
/// Represents a snapshot of the entire visual scene at one moment in the simulation.
/// </summary>
/// <param name="Dice">The state of every die in the scene.</param>
/// <param name="ElapsedSeconds">The total simulated time of the current roll, in seconds.</param>
/// <param name="IsSettled">A value indicating whether every die has come to rest.</param>
public sealed record DiceSceneState(
    IReadOnlyList<DieState> Dice,
    float ElapsedSeconds,
    bool IsSettled)
{
    /// <summary>
    /// Gets an empty scene with no dice and no elapsed time.
    /// </summary>
    public static DiceSceneState Empty { get; } = new([], 0f, false);
}
