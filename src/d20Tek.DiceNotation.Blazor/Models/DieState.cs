namespace d20Tek.DiceNotation.Blazor.Models;

/// <summary>
/// Represents the renderable state of a single die at one moment in the simulation.
/// Coordinates are expressed in logical simulation units rather than device pixels.
/// </summary>
/// <param name="Id">The stable identifier of the die.</param>
/// <param name="Value">The fixed result displayed on the die.</param>
/// <param name="X">The horizontal position of the die's center.</param>
/// <param name="Y">The vertical position of the die's center.</param>
/// <param name="Rotation">The rotation of the die, in radians.</param>
/// <param name="Width">The width of the die.</param>
/// <param name="Height">The height of the die.</param>
public sealed record DieState(
    string Id,
    int Value,
    float X,
    float Y,
    float Rotation,
    float Width,
    float Height);
