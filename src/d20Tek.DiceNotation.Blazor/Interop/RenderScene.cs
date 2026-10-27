using System.Text.Json.Serialization;
using d20Tek.DiceNotation.Blazor.Models;
using d20Tek.DiceNotation.Blazor.Services;

namespace d20Tek.DiceNotation.Blazor.Interop;

/// <summary>
/// Represents the compact, serializable payload sent to the Canvas renderer for a single
/// frame. The whole scene travels in one batched interop call rather than one call per die.
/// </summary>
/// <param name="Tray">The logical dimensions of the tray.</param>
/// <param name="Dice">The state of every die to draw.</param>
/// <param name="Theme">The resolved style data used to draw the scene.</param>
/// <param name="IsSettled">A value indicating whether the scene has come to rest.</param>
/// <param name="Visuals">The resolved renderer tuning values used to draw the scene.</param>
internal sealed record RenderScene(
    [property: JsonPropertyName("tray")] RenderTray Tray,
    [property: JsonPropertyName("dice")] IReadOnlyList<RenderDie> Dice,
    [property: JsonPropertyName("theme")] DiceTheme Theme,
    [property: JsonPropertyName("isSettled")] bool IsSettled,
    [property: JsonPropertyName("visuals")] RenderVisuals? Visuals = null)
{
    /// <summary>
    /// Creates a render payload from a scene snapshot.
    /// </summary>
    /// <param name="scene">The scene snapshot produced by the simulation.</param>
    /// <param name="traySize">The logical dimensions of the tray.</param>
    /// <param name="theme">The theme to draw with, or <see langword="null"/> for the default.</param>
    /// <param name="visualSettings">
    /// The presentation tuning used to resolve each die's drawing instructions, or
    /// <see langword="null"/> for the defaults.
    /// </param>
    /// <returns>A payload ready to send through a single interop call.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="scene"/> or <paramref name="traySize"/> is <see langword="null"/>.
    /// </exception>
    public static RenderScene FromSnapshot(
        DiceSceneState scene,
        DiceTraySize traySize,
        DiceTheme? theme = null,
        DiceVisualSettings? visualSettings = null)
    {
        ArgumentNullException.ThrowIfNull(scene);
        ArgumentNullException.ThrowIfNull(traySize);

        var resolvedTheme = theme ?? DiceTheme.Default;
        resolvedTheme.Validate();
        traySize.Validate();

        // Presentation policy is resolved here so the Canvas renderer only has to draw.
        var calculator = new DieVisualCalculator(visualSettings);
        var dice = new RenderDie[scene.Dice.Count];
        for (var i = 0; i < scene.Dice.Count; i++)
        {
            var die = scene.Dice[i];
            var visual = calculator.Resolve(
                die,
                scene.ElapsedSeconds,
                scene.IsSettled,
                resolvedTheme.UseMotionEffects);

            dice[i] = new RenderDie(
                die.Id,
                die.Value,
                die.X,
                die.Y,
                die.Rotation,
                die.Width,
                die.Height,
                die.Energy,
                die.Sides,
                visual.ShownValue,
                visual.Lift,
                visual.ShadowBlur,
                visual.ShadowOffset,
                visual.SmearStrength,
                visual.SmearReach,
                visual.SmearDirectionX,
                visual.SmearDirectionY);
        }

        return new RenderScene(
            new RenderTray(traySize.Width, traySize.Height),
            dice,
            resolvedTheme,
            scene.IsSettled,
            RenderVisuals.FromSettings(visualSettings ?? DiceVisualSettings.Default));
    }
}

/// <summary>
/// Represents the renderer tuning values within a render payload. These describe how to
/// draw, not what to decide, so the Canvas renderer holds no tuning constants of its own.
/// </summary>
/// <param name="LightAngle">The direction light arrives from, in canvas radians.</param>
/// <param name="BevelThickness">The fraction of the die's smaller dimension used as bevel thickness.</param>
/// <param name="SmearSteps">The number of fading copies drawn in a motion smear.</param>
internal sealed record RenderVisuals(
    [property: JsonPropertyName("lightAngle")] float LightAngle,
    [property: JsonPropertyName("bevelThickness")] float BevelThickness,
    [property: JsonPropertyName("smearSteps")] int SmearSteps)
{
    /// <summary>
    /// Creates a renderer tuning payload from presentation settings.
    /// </summary>
    /// <param name="settings">The presentation settings to project.</param>
    /// <returns>The values the Canvas renderer needs to draw the scene.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="settings"/> is <see langword="null"/>.
    /// </exception>
    public static RenderVisuals FromSettings(DiceVisualSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return new RenderVisuals(
            settings.LightAngle,
            settings.BevelThickness,
            settings.SmearSteps);
    }
}

/// <summary>
/// Represents the tray dimensions within a render payload.
/// </summary>
/// <param name="Width">The width of the tray, in logical units.</param>
/// <param name="Height">The height of the tray, in logical units.</param>
internal sealed record RenderTray(
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
/// <param name="Energy">
/// How vigorously the die is moving, normalized to the range 0 to 1. The renderer uses
/// this for motion effects only; it never influences the displayed result.
/// </param>
/// <param name="Sides">The number of faces on the die.</param>
/// <param name="ShownValue">The face to draw, with any transient churn already resolved.</param>
/// <param name="Lift">The scale multiplier that reads as height above the tray.</param>
/// <param name="ShadowBlur">The shadow blur radius, in logical units.</param>
/// <param name="ShadowOffset">The vertical shadow offset, in logical units.</param>
/// <param name="SmearStrength">How strongly to draw the motion trail, from 0 to 1.</param>
/// <param name="SmearReach">How far the motion trail extends behind the die.</param>
/// <param name="SmearDirectionX">The normalized horizontal direction the die is travelling.</param>
/// <param name="SmearDirectionY">The normalized vertical direction the die is travelling.</param>
internal sealed record RenderDie(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("value")] int Value,
    [property: JsonPropertyName("x")] float X,
    [property: JsonPropertyName("y")] float Y,
    [property: JsonPropertyName("rotation")] float Rotation,
    [property: JsonPropertyName("width")] float Width,
    [property: JsonPropertyName("height")] float Height,
    [property: JsonPropertyName("energy")] float Energy = 0f,
    [property: JsonPropertyName("sides")] int Sides = 6,
    [property: JsonPropertyName("shownValue")] int ShownValue = 0,
    [property: JsonPropertyName("lift")] float Lift = 1f,
    [property: JsonPropertyName("shadowBlur")] float ShadowBlur = 0f,
    [property: JsonPropertyName("shadowOffset")] float ShadowOffset = 0f,
    [property: JsonPropertyName("smearStrength")] float SmearStrength = 0f,
    [property: JsonPropertyName("smearReach")] float SmearReach = 0f,
    [property: JsonPropertyName("smearDirectionX")] float SmearDirectionX = 0f,
    [property: JsonPropertyName("smearDirectionY")] float SmearDirectionY = 0f);
