using d20Tek.DiceNotation.Blazor.Interop;

namespace d20Tek.DiceNotation.Blazor.UnitTests.Interop;

[TestClass]
public class RenderSceneGeometryTests
{
    private static DiceSceneState CreateScene(int sides, float size = 0.6f) =>
        new([new DieState("die-0", 3, 0f, 0f, 0f, size, size, 0f, sides)], 1f, false);

    [TestMethod]
    [DataRow(4, 3)]
    [DataRow(6, 4)]
    [DataRow(8, 4)]
    [DataRow(12, 5)]
    [DataRow(20, 6)]
    public void FromSnapshot_ForDieType_EmitsOneCoordinatePairPerVertex(int sides, int expectedVertices)
    {
        // Arrange
        var scene = CreateScene(sides);

        // Act
        var result = RenderScene.FromSnapshot(scene, DiceTraySize.Default);

        // Assert
        Assert.IsNotNull(result.Dice[0].Vertices);
        Assert.HasCount(expectedVertices * 2, result.Dice[0].Vertices!);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void FromSnapshot_WithDie_ScalesVerticesByDieSize()
    {
        // Arrange
        var small = RenderScene.FromSnapshot(CreateScene(6, 0.5f), DiceTraySize.Default);

        // Act
        var large = RenderScene.FromSnapshot(CreateScene(6, 1f), DiceTraySize.Default);

        // Assert
        var smallReach = small.Dice[0].Vertices!.Max(Math.Abs);
        var largeReach = large.Dice[0].Vertices!.Max(Math.Abs);
        Assert.AreEqual(smallReach * 2f, largeReach, 0.0001f);
    }

    [TestMethod]
    public void FromSnapshot_ForDifferentDieTypes_EmitsDifferentGeometry()
    {
        // Arrange
        var d6 = RenderScene.FromSnapshot(CreateScene(6), DiceTraySize.Default);

        // Act
        var d20 = RenderScene.FromSnapshot(CreateScene(20), DiceTraySize.Default);

        // Assert
        Assert.AreNotEqual(d6.Dice[0].Vertices!.Count, d20.Dice[0].Vertices!.Count);
    }

    [TestMethod]
    public void FromSnapshot_WithDie_EmitsPositiveCornerRadius()
    {
        // Arrange
        var scene = CreateScene(6);

        // Act
        var result = RenderScene.FromSnapshot(scene, DiceTraySize.Default);

        // Assert
        Assert.IsGreaterThan(0f, result.Dice[0].CornerRadius);
    }

    [TestMethod]
    public void FromSnapshot_ForSharpCorneredDie_UsesSmallerCornerRadius()
    {
        // Arrange
        var d6 = RenderScene.FromSnapshot(CreateScene(6), DiceTraySize.Default);

        // Act
        var d4 = RenderScene.FromSnapshot(CreateScene(4), DiceTraySize.Default);

        // Assert
        Assert.IsLessThan(d6.Dice[0].CornerRadius, d4.Dice[0].CornerRadius);
    }

    [TestMethod]
    public void FromSnapshot_WithUnusualDieType_StillEmitsGeometry()
    {
        // Arrange
        var scene = CreateScene(7);

        // Act
        var result = RenderScene.FromSnapshot(scene, DiceTraySize.Default);

        // Assert
        Assert.IsNotNull(result.Dice[0].Vertices);
        Assert.IsGreaterThanOrEqualTo(6, result.Dice[0].Vertices!.Count);
    }
}
