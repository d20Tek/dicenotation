using d20Tek.DiceNotation.Blazor.Models;

namespace d20Tek.DiceNotation.Blazor.Services;

/// <summary>
/// Turns a die's physical state into fully resolved drawing instructions. This type holds
/// the presentation policy that the Canvas renderer used to decide for itself.
/// </summary>
/// <remarks>
/// Every method is a pure function of its arguments. Nothing here is stateful, so the
/// visuals cannot drift out of step with the simulation, and nothing here can change a
/// die result: churned faces are transient and always resolve to the real value as a die
/// slows down.
/// </remarks>
public sealed class DieVisualCalculator
{
    private readonly DiceVisualSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="DieVisualCalculator"/> class.
    /// </summary>
    /// <param name="settings">The presentation settings, or <see langword="null"/> for the defaults.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any setting is invalid.</exception>
    public DieVisualCalculator(DiceVisualSettings? settings = null)
    {
        _settings = settings ?? DiceVisualSettings.Default;
        _settings.Validate();
    }

    /// <summary>
    /// Resolves the drawing instructions for a single die.
    /// </summary>
    /// <param name="die">The die state produced by the simulation.</param>
    /// <param name="elapsedSeconds">The elapsed time of the roll, used to drive the tumble.</param>
    /// <param name="isSettled">Whether the scene has come to rest.</param>
    /// <param name="useMotionEffects">Whether motion effects are enabled for this scene.</param>
    /// <returns>The resolved visual state for the die.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="die"/> is <see langword="null"/>.</exception>
    public DieVisualState Resolve(
        DieState die,
        float elapsedSeconds,
        bool isSettled,
        bool useMotionEffects)
    {
        ArgumentNullException.ThrowIfNull(die);

        // A settled scene always draws true values at rest, regardless of reported energy.
        var energy = useMotionEffects && !isSettled ? Clamp01(die.Energy) : 0f;
        var smearStrength = energy > _settings.SmearEnergy ? energy : 0f;
        var (dirX, dirY) = CalculateTravelDirection(die);

        return new DieVisualState(
            ResolveShownValue(die, energy, elapsedSeconds),
            1f + (energy * _settings.MaxLift),
            _settings.BaseShadowBlur + (energy * _settings.EnergyShadowBlur),
            _settings.BaseShadowOffset + (energy * _settings.EnergyShadowOffset),
            smearStrength,
            MathF.Max(die.Width, die.Height) * energy * _settings.SmearReach,
            dirX,
            dirY);
    }

    /// <summary>
    /// Normalizes a die's velocity into a unit direction used to trail its motion smear.
    /// </summary>
    /// <param name="die">The die state.</param>
    /// <returns>A unit vector, or zero when the die has no meaningful direction.</returns>
    internal static (float X, float Y) CalculateTravelDirection(DieState die)
    {
        var vx = float.IsFinite(die.VelocityX) ? die.VelocityX : 0f;
        var vy = float.IsFinite(die.VelocityY) ? die.VelocityY : 0f;
        var speed = MathF.Sqrt((vx * vx) + (vy * vy));

        return speed > 0.0001f ? (vx / speed, vy / speed) : (0f, 0f);
    }

    /// <summary>
    /// Determines the face to draw. A fast-moving die churns through plausible faces for
    /// its own side count; as it slows it locks to the fixed, precomputed result.
    /// </summary>
    /// <param name="die">The die state.</param>
    /// <param name="energy">The die's clamped energy.</param>
    /// <param name="elapsedSeconds">The elapsed time of the roll.</param>
    /// <returns>The face value to display.</returns>
    internal int ResolveShownValue(DieState die, float energy, float elapsedSeconds)
    {
        if (energy <= _settings.FaceLockEnergy)
        {
            return die.Value;
        }

        return ChurnFace(die, energy, elapsedSeconds);
    }

    /// <summary>
    /// Produces a transient face for a tumbling die, derived from the die's own rotation
    /// so it is deterministic and needs no per-frame state.
    /// </summary>
    /// <param name="die">The die state.</param>
    /// <param name="energy">The die's clamped energy.</param>
    /// <param name="elapsedSeconds">The elapsed time of the roll.</param>
    /// <returns>A face value between 1 and the die's side count.</returns>
    internal static int ChurnFace(DieState die, float energy, float elapsedSeconds)
    {
        var faces = Math.Max(1, die.Sides);
        if (faces == 1)
        {
            return 1;
        }

        var spin = MathF.Abs(die.Rotation) * (2f + (energy * 8f));
        if (!float.IsFinite(spin))
        {
            return die.Value;
        }

        return ((int)MathF.Floor(spin) % faces) + 1;
    }

    private static float Clamp01(float value) =>
        float.IsFinite(value) ? Math.Clamp(value, 0f, 1f) : 0f;
}
