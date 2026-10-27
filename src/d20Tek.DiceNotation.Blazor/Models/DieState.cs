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
/// <param name="Energy">
/// How vigorously the die is moving, normalized to the range 0 to 1, where 0 is fully at
/// rest and 1 is moving as fast as a launch. The renderer uses this to imply height,
/// motion, and tumbling. It is a presentation hint only and never affects the result.
/// </param>
/// <param name="Sides">The number of faces on the die.</param>
/// <param name="VelocityX">
/// The horizontal velocity of the die, in logical units per second. The renderer uses
/// this to tumble a die forward over its direction of travel.
/// </param>
/// <param name="VelocityY">The vertical velocity of the die, in logical units per second.</param>
/// <param name="MinValue">
/// The lowest face value on the die. Standard dice start at one, but fudge dice run from
/// minus one to plus one, so the face range cannot be assumed from <paramref name="Sides"/>.
/// </param>
internal sealed record DieState(
    string Id,
    int Value,
    float X,
    float Y,
    float Rotation,
    float Width,
    float Height,
    float Energy = 0f,
    int Sides = 6,
    float VelocityX = 0f,
    float VelocityY = 0f,
    int MinValue = 1);
