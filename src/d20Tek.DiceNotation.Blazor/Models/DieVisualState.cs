namespace d20Tek.DiceNotation.Blazor.Models;

/// <summary>
/// Represents the fully resolved drawing instructions for a single die in one frame.
/// Every presentation decision has already been made, so the Canvas renderer only has
/// to apply these values rather than interpret physical state.
/// </summary>
/// <param name="ShownValue">The face to draw, with any transient churn already resolved.</param>
/// <param name="Lift">The scale multiplier that reads as height above the tray.</param>
/// <param name="ShadowBlur">The shadow blur radius, in logical units.</param>
/// <param name="ShadowOffset">The vertical shadow offset, in logical units.</param>
/// <param name="SmearStrength">
/// How strongly to draw the motion trail, from 0 for none to 1 for the full effect.
/// </param>
/// <param name="SmearReach">How far the motion trail extends behind the die, in logical units.</param>
/// <param name="SmearDirectionX">The normalized horizontal direction the die is travelling.</param>
/// <param name="SmearDirectionY">The normalized vertical direction the die is travelling.</param>
internal readonly record struct DieVisualState(
    int ShownValue,
    float Lift,
    float ShadowBlur,
    float ShadowOffset,
    float SmearStrength,
    float SmearReach,
    float SmearDirectionX = 0f,
    float SmearDirectionY = 0f);
