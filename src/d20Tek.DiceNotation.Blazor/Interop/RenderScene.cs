using System.Text.Json.Serialization;
using d20Tek.DiceNotation.Blazor.Models;

namespace d20Tek.DiceNotation.Blazor.Interop;

/// <summary>
/// Represents the compact, serializable payload sent to the Canvas renderer for a single
/// frame. The whole scene travels in one batched interop call rather than one call per die.
/// </summary>
/// <param name="Tray">The logical dimensions of the tray.</param>
/// <param name="Dice">The state of every die to draw.</param>
/// <param name="Theme">The resolved style data used to draw the scene.</param>
/// <param name="IsSettled">A value indicating whether the scene has come to rest.</param>
public sealed record RenderScene(
    [property: JsonPropertyName("tray")] RenderTray Tray,
    [property: JsonPropertyName("dice")] IReadOnlyList<RenderDie> Dice,
    [property: JsonPropertyName("theme")] DiceTheme Theme,
    [property: JsonPropertyName("isSettled")] bool IsSettled)
{
    /// <summary>
    /// Creates a render payload from a scene snapshot.
    /// </summary>
    /// <param name="scene">The scene snapshot produced by the simulation.</param>
    /// <param name="traySize">The logical dimensions of the tray.</param>
    /// <param name="theme">The theme to draw with, or <see langword="null"/> for the default.</param>
    /// <returns>A payload ready to send through a single interop call.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="scene"/> or <paramref name="traySize"/> is <see langword="null"/>.
    /// </exception>
    public static RenderScene FromSnapshot(
        DiceSceneState scene,
        DiceTraySize traySize,
        DiceTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(scene);
        ArgumentNullException.ThrowIfNull(traySize);

        var resolvedTheme = theme ?? DiceTheme.Default;
        resolvedTheme.Validate();
        traySize.Validate();

        var dice = new RenderDie[scene.Dice.Count];
        for (var i = 0; i < scene.Dice.Count; i++)
        {
            var die = scene.Dice[i];
            dice[i] = new RenderDie(die.Id, die.Value, die.X, die.Y, die.Rotation, die.Width, die.Height);
        }

        return new RenderScene(
            new RenderTray(traySize.Width, traySize.Height),
            dice,
            resolvedTheme,
            scene.IsSettled);
    }
}

/// <summary>
/// Represents the tray dimensions within a render payload.
/// </summary>
/// <param name="Width">The width of the tray, in logical units.</param>
/// <param name="Height">The height of the tray, in logical units.</param>
public sealed record RenderTray(
    [property: JsonPropertyName("width")] float Width,
    [property: JsonPropertyName("height")] float Height);

/// <summary>
/// Represents a single die within a render payload.
/// </summary>
/// <param name="Id">The stable identifier of the die.</param>
/// <param name="Value">The fixed result drawn on the die.</param>
/// <param name="X">The horizontal position of the die's center, in logical units.</param>
/// <param name="Y">The vertical position of the die's center, in logical units.</param>
/// <param name="Rotation">The rotation of the die, in radians.</param>
/// <param name="Width">The width of the die, in logical units.</param>
/// <param name="Height">The height of the die, in logical units.</param>
public sealed record RenderDie(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("value")] int Value,
    [property: JsonPropertyName("x")] float X,
    [property: JsonPropertyName("y")] float Y,
    [property: JsonPropertyName("rotation")] float Rotation,
    [property: JsonPropertyName("width")] float Width,
    [property: JsonPropertyName("height")] float Height);
