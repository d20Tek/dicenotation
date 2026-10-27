using System.Numerics;

namespace d20Tek.DiceNotation.Blazor.Services;

/// <summary>
/// Supplies the two-dimensional outline used to draw and simulate a die with a given
/// number of faces. Physical dice are solids, so these outlines are the recognizable
/// silhouette each solid presents rather than a projection of the solid itself.
/// </summary>
/// <remarks>
/// <para>
/// This is the single source of truth for die shape. The renderer traces the outline
/// returned here, and the physics simulation builds its bodies from the same outline, so
/// a die can never collide as one shape while being drawn as another.
/// </para>
/// <para>
/// Outlines are normalized to a unit bounding size and to approximately equal area. Equal
/// area matters more than equal width here: a hexagon inscribed in the same square as a
/// triangle looks far bigger, so without this correction a d20 would dwarf a d4 in the
/// same tray.
/// </para>
/// </remarks>
internal static class DieGeometry
{
    private const float TargetArea = 1f;

    // Outlines never change, so each side count is built once and shared. The dictionary
    // is only ever read after construction, which keeps it safe to share.
    private static readonly Dictionary<int, IReadOnlyList<Vector2>> _cache = [];
    private static readonly Lock _cacheLock = new();

    /// <summary>
    /// Gets the outline for a die with the given number of faces, expressed in units of
    /// the die's logical size and centered on the origin.
    /// </summary>
    /// <param name="sides">The number of faces on the die.</param>
    /// <returns>The convex outline, in counter-clockwise order.</returns>
    /// <remarks>
    /// Side counts without a conventional shape fall back to a regular polygon, so an
    /// unusual die still renders and simulates sensibly instead of failing.
    /// </remarks>
    public static IReadOnlyList<Vector2> GetOutline(int sides)
    {
        lock (_cacheLock)
        {
            if (_cache.TryGetValue(sides, out var cached))
            {
                return cached;
            }

            var outline = NormalizeArea(BuildOutline(sides));
            _cache[sides] = outline;
            return outline;
        }
    }

    /// <summary>
    /// Gets the corner rounding to apply to a die outline, as a fraction of the die's
    /// logical size.
    /// </summary>
    /// <param name="sides">The number of faces on the die.</param>
    /// <returns>The corner radius factor.</returns>
    /// <remarks>
    /// Shapes with sharp corners need a smaller radius, because rounding a narrow corner
    /// as heavily as a square corner would visibly erode the silhouette. A coin is the
    /// opposite case: its radius deliberately exceeds what any corner can consume, so the
    /// renderer's per-corner clamp rounds every corner as far as it will go and the
    /// octagon reads as a circle.
    /// </remarks>
    public static float GetCornerRadiusFactor(int sides) => sides switch
    {
        2 => 0.5f,
        4 => 0.08f,
        10 or 100 => 0.1f,
        _ => 0.14f,
    };

    private static IReadOnlyList<Vector2> BuildOutline(int sides) => sides switch
    {
        // A two-sided die is a coin. Aether accepts at most 8 polygon vertices, so the
        // circle is approximated by an octagon and the corner rounding does the rest of
        // the work of making it read as round.
        2 => BuildRegularPolygon(8, MathF.PI / 8f),

        // A tetrahedron reads as a triangle.
        4 => BuildRegularPolygon(3, MathF.PI / 2f),

        // A cube reads as a square, kept axis-aligned so it looks like a classic die.
        6 => BuildRegularPolygon(4, MathF.PI / 4f),

        // An octahedron reads as a vertical diamond.
        8 => BuildRegularPolygon(4, MathF.PI / 2f),

        // A pentagonal trapezohedron reads as a kite, which is what distinguishes a d10
        // from the other shapes at a glance.
        10 or 100 => BuildKite(),

        // A dodecahedron presents a pentagonal face.
        12 => BuildRegularPolygon(5, MathF.PI / 2f),

        // An icosahedron presents a triangular face, but a point-up triangle would be
        // indistinguishable from a d4, so the recognizable hexagonal silhouette is used.
        20 => BuildRegularPolygon(6, MathF.PI / 2f),

        // Anything else gets a regular polygon with a sensible vertex count. Aether
        // supports at most 8 polygon vertices, so the count is clamped to that.
        _ => BuildRegularPolygon(Math.Clamp(sides, 3, 8), MathF.PI / 2f),
    };

    private static Vector2[] BuildRegularPolygon(int count, float startAngle)
    {
        var points = new Vector2[count];
        var step = MathF.Tau / count;

        for (var i = 0; i < count; i++)
        {
            var angle = startAngle + (step * i);
            points[i] = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }

        return points;
    }

    private static Vector2[] BuildKite() =>
    [
        new(0f, 1f),
        new(-0.62f, 0.12f),
        new(0f, -1f),
        new(0.62f, 0.12f),
    ];

    /// <summary>
    /// Scales an outline so every die shape covers approximately the same area, then
    /// centers it on its own centroid so rotation looks balanced.
    /// </summary>
    private static IReadOnlyList<Vector2> NormalizeArea(IReadOnlyList<Vector2> points)
    {
        var centroid = GetCentroid(points);
        var centered = new Vector2[points.Count];
        for (var i = 0; i < points.Count; i++)
        {
            centered[i] = points[i] - centroid;
        }

        var area = GetArea(centered);

        // Area scales with the square of a linear scale factor, so the square root
        // converts the desired area ratio into the linear scale to apply.
        var scale = MathF.Sqrt(TargetArea / area);

        var result = new Vector2[centered.Length];
        for (var i = 0; i < centered.Length; i++)
        {
            result[i] = centered[i] * scale;
        }

        return result;
    }

    private static Vector2 GetCentroid(IReadOnlyList<Vector2> points)
    {
        var sum = Vector2.Zero;
        foreach (var point in points)
        {
            sum += point;
        }

        return sum / points.Count;
    }

    /// <summary>
    /// Calculates the area of a simple polygon using the shoelace formula.
    /// </summary>
    private static float GetArea(IReadOnlyList<Vector2> points)
    {
        var total = 0f;
        for (var i = 0; i < points.Count; i++)
        {
            var current = points[i];
            var next = points[(i + 1) % points.Count];
            total += (current.X * next.Y) - (next.X * current.Y);
        }

        return MathF.Abs(total) / 2f;
    }
}
