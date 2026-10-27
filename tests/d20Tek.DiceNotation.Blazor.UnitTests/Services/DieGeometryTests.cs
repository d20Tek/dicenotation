using System.Numerics;

namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public class DieGeometryTests
{
    [TestMethod]
    [DataRow(4, 3)]
    [DataRow(6, 4)]
    [DataRow(8, 4)]
    [DataRow(10, 4)]
    [DataRow(100, 4)]
    [DataRow(12, 5)]
    [DataRow(20, 6)]
    public void GetOutline_WithStandardDieType_ReturnsExpectedVertexCount(int sides, int expectedCount)
    {
        // Arrange
        // Act
        var result = DieGeometry.GetOutline(sides);

        // Assert
        Assert.HasCount(expectedCount, result);
    }

    [TestMethod]
    [DataRow(3)]
    [DataRow(5)]
    [DataRow(7)]
    [DataRow(30)]
    public void GetOutline_WithUnconventionalDieType_ReturnsUsablePolygon(int sides)
    {
        // Arrange
        // Act
        var result = DieGeometry.GetOutline(sides);

        // Assert
        Assert.IsGreaterThanOrEqualTo(3, result.Count);
        Assert.IsLessThanOrEqualTo(8, result.Count);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(0)]
    public void GetOutline_WithTooFewSides_ClampsToTriangle(int sides)
    {
        // Arrange
        // Act
        var result = DieGeometry.GetOutline(sides);

        // Assert
        Assert.HasCount(3, result);
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(4)]
    [DataRow(6)]
    [DataRow(8)]
    [DataRow(10)]
    [DataRow(12)]
    [DataRow(20)]
    [DataRow(100)]
    public void GetOutline_WithStandardDieType_ReturnsConvexPolygon(int sides)
    {
        // Arrange
        var outline = DieGeometry.GetOutline(sides);

        // Act
        var isConvex = IsConvex(outline);

        // Assert
        Assert.IsTrue(isConvex, $"The outline for d{sides} must be convex to be usable as a physics body.");
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(4)]
    [DataRow(6)]
    [DataRow(8)]
    [DataRow(10)]
    [DataRow(12)]
    [DataRow(20)]
    public void GetOutline_WithStandardDieType_IsCenteredOnOrigin(int sides)
    {
        // Arrange
        var outline = DieGeometry.GetOutline(sides);

        // Act
        var centroid = outline.Aggregate(Vector2.Zero, (sum, point) => sum + point) / outline.Count;

        // Assert
        Assert.IsLessThan(0.001f, Math.Abs(centroid.X));
        Assert.IsLessThan(0.001f, Math.Abs(centroid.Y));
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(4)]
    [DataRow(6)]
    [DataRow(8)]
    [DataRow(10)]
    [DataRow(12)]
    [DataRow(20)]
    public void GetOutline_WithStandardDieType_NormalizesAreaToOne(int sides)
    {
        // Arrange
        var outline = DieGeometry.GetOutline(sides);

        // Act
        var area = GetArea(outline);

        // Assert
        Assert.IsLessThan(0.01f, Math.Abs(area - 1f), $"d{sides} has area {area}, which would look out of scale.");
    }

    [TestMethod]
    public void GetOutline_WhenCalledTwice_ReturnsCachedInstance()
    {
        // Arrange
        var first = DieGeometry.GetOutline(20);

        // Act
        var second = DieGeometry.GetOutline(20);

        // Assert
        Assert.AreSame(first, second);
    }

    [TestMethod]
    public void GetOutline_ForDifferentDieTypes_ReturnsDistinctShapes()
    {
        // Arrange
        var d6 = DieGeometry.GetOutline(6);

        // Act
        var d8 = DieGeometry.GetOutline(8);

        // Assert
        // A d6 and d8 both have four vertices, so they must differ by orientation.
        Assert.HasCount(d6.Count, d8);
        Assert.IsTrue(d6.Where((p, i) => p != d8[i]).Any());
    }

    [TestMethod]
    [DataRow(4, 0.08f)]
    [DataRow(10, 0.1f)]
    [DataRow(100, 0.1f)]
    [DataRow(6, 0.14f)]
    [DataRow(20, 0.14f)]
    public void GetCornerRadiusFactor_WithDieType_ReturnsShapeAppropriateRadius(int sides, float expected)
    {
        // Arrange
        // Act
        var result = DieGeometry.GetCornerRadiusFactor(sides);

        // Assert
        Assert.AreEqual(expected, result, 0.0001f);
    }

    private static bool IsConvex(IReadOnlyList<Vector2> points)
    {
        var sign = 0;
        for (var i = 0; i < points.Count; i++)
        {
            var a = points[i];
            var b = points[(i + 1) % points.Count];
            var c = points[(i + 2) % points.Count];

            var cross = ((b.X - a.X) * (c.Y - b.Y)) - ((b.Y - a.Y) * (c.X - b.X));
            if (Math.Abs(cross) < 0.0001f) continue;

            var current = cross > 0 ? 1 : -1;
            if (sign == 0)
            {
                sign = current;
            }
            else if (sign != current)
            {
                return false;
            }
        }

        return true;
    }

    private static float GetArea(IReadOnlyList<Vector2> points)
    {
        var total = 0f;
        for (var i = 0; i < points.Count; i++)
        {
            var current = points[i];
            var next = points[(i + 1) % points.Count];
            total += (current.X * next.Y) - (next.X * current.Y);
        }

        return Math.Abs(total) / 2f;
    }
}
