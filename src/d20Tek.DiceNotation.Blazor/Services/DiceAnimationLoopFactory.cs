using d20Tek.DiceNotation.Blazor.Interop;
using d20Tek.DiceNotation.Blazor.Models;
using Microsoft.JSInterop;

namespace d20Tek.DiceNotation.Blazor.Services;

/// <summary>
/// Creates the animation loop used by one component instance. The loop owns mutable
/// animation state, so each component must receive its own instance rather than sharing
/// a registered singleton.
/// </summary>
internal interface IDiceAnimationLoopFactory
{
    /// <summary>Creates a loop and its renderer for a single component instance.</summary>
    /// <param name="settings">The physics settings the loop and simulation share.</param>
    /// <returns>The renderer to initialize and the loop that drives it.</returns>
    (CanvasRenderer Renderer, DiceAnimationLoop Loop) Create(DiceSimulationSettings settings);
}

/// <summary>
/// Creates animation loops backed by the Aether physics simulation and the Canvas
/// JavaScript module.
/// </summary>
internal sealed class DiceAnimationLoopFactory : IDiceAnimationLoopFactory
{
    private readonly IJSRuntime _jsRuntime;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiceAnimationLoopFactory"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime used by each created renderer.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="jsRuntime"/> is <see langword="null"/>.
    /// </exception>
    public DiceAnimationLoopFactory(IJSRuntime jsRuntime)
    {
        ArgumentNullException.ThrowIfNull(jsRuntime);
        _jsRuntime = jsRuntime;
    }

    /// <inheritdoc />
    public (CanvasRenderer Renderer, DiceAnimationLoop Loop) Create(DiceSimulationSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var renderer = new CanvasRenderer(_jsRuntime);
        var simulation = new AetherDiceSimulation(settings);

        return (renderer, new DiceAnimationLoop(simulation, renderer, settings));
    }
}
