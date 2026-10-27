using d20Tek.DiceNotation.Blazor.Models;

namespace d20Tek.DiceNotation.Blazor.Services;

/// <summary>
/// Defines a physics simulation that animates dice in a tray. The simulation owns only
/// positions, rotations, and settling. It never produces or alters a die result.
/// </summary>
internal interface IDiceSimulation : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether every die has come to rest, or the roll reached
    /// its maximum duration.
    /// </summary>
    bool IsSettled { get; }

    /// <summary>Gets the total simulated time of the current roll, in seconds.</summary>
    float ElapsedSeconds { get; }

    /// <summary>
    /// Resets the simulation and launches the supplied dice into a tray of the given size.
    /// </summary>
    /// <param name="dice">The dice to animate, each carrying its precomputed result.</param>
    /// <param name="traySize">The logical dimensions of the tray.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="dice"/> or <paramref name="traySize"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the dice collection is empty, exceeds the configured maximum, or
    /// contains duplicate identifiers.
    /// </exception>
    void StartRoll(IReadOnlyList<VisualDie> dice, DiceTraySize traySize);

    /// <summary>
    /// Advances the simulation by the supplied elapsed time using a fixed internal timestep.
    /// The number of physics steps performed is bounded, so a large elapsed value cannot
    /// cause unbounded catch-up work.
    /// </summary>
    /// <param name="elapsedSeconds">The wall-clock time since the previous step, in seconds.</param>
    /// <returns>The number of fixed physics steps that were performed.</returns>
    int Step(float elapsedSeconds);

    /// <summary>Captures the current renderable state of the scene.</summary>
    /// <returns>A snapshot of every die, plus elapsed time and settling information.</returns>
    DiceSceneState GetSnapshot();
}
