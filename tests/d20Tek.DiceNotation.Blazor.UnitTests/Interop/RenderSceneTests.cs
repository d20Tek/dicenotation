using d20Tek.DiceNotation.Blazor.Interop;

namespace d20Tek.DiceNotation.Blazor.UnitTests.Interop;

[TestClass]
public class RenderSceneTests
{
    private static DiceSceneState CreateScene(bool isSettled = false) =>
        new(
            [
                new DieState("die-0", 3, 1.5f, -2f, 0.75f, 0.6f, 0.6f),
                new DieState("die-1", 6, -1f, 2.25f, -1.2f, 0.6f, 0.6f),
            ],
            1.5f,
            isSettled);

    [TestMethod]
    public void FromSnapshot_WithScene_MapsEveryDie()
    {
        // Arrange
        var scene = CreateScene();

        // Act
        var result = RenderScene.FromSnapshot(scene, DiceTraySize.Default);

        // Assert
        Assert.HasCount(2, result.Dice);
        Assert.AreEqual("die-0", result.Dice[0].Id);
        Assert.AreEqual("die-1", result.Dice[1].Id);
    }

    [TestMethod]
    public void FromSnapshot_WithScene_PreservesDieValuesAndGeometry()
    {
        // Arrange
        var scene = CreateScene();

        // Act
        var result = RenderScene.FromSnapshot(scene, DiceTraySize.Default);

        // Assert
        var die = result.Dice[0];
        Assert.AreEqual(3, die.Value);
        Assert.AreEqual(1.5f, die.X);
        Assert.AreEqual(-2f, die.Y);
        Assert.AreEqual(0.75f, die.Rotation);
        Assert.AreEqual(0.6f, die.Width);
        Assert.AreEqual(0.6f, die.Height);
    }

    [TestMethod]
    public void FromSnapshot_WithTraySize_CopiesTrayDimensions()
    {
        // Arrange
        var scene = CreateScene();
        var tray = new DiceTraySize(12f, 9f);

        // Act
        var result = RenderScene.FromSnapshot(scene, tray);

        // Assert
        Assert.AreEqual(12f, result.Tray.Width);
        Assert.AreEqual(9f, result.Tray.Height);
    }

    [TestMethod]
    public void FromSnapshot_WithoutTheme_UsesDefaultTheme()
    {
        // Arrange
        var scene = CreateScene();

        // Act
        var result = RenderScene.FromSnapshot(scene, DiceTraySize.Default);

        // Assert
        Assert.AreSame(DiceTheme.Default, result.Theme);
    }

    [TestMethod]
    public void FromSnapshot_WithTheme_UsesSuppliedTheme()
    {
        // Arrange
        var scene = CreateScene();

        // Act
        var result = RenderScene.FromSnapshot(scene, DiceTraySize.Default, DiceTheme.DarkFantasy);

        // Assert
        Assert.AreSame(DiceTheme.DarkFantasy, result.Theme);
    }

    [TestMethod]
    public void FromSnapshot_WithSettledScene_PropagatesSettledFlag()
    {
        // Arrange
        var scene = CreateScene(isSettled: true);

        // Act
        var result = RenderScene.FromSnapshot(scene, DiceTraySize.Default);

        // Assert
        Assert.IsTrue(result.IsSettled);
    }

    [TestMethod]
    public void FromSnapshot_WithEmptyScene_ProducesNoDice()
    {
        // Arrange
        // Act
        var result = RenderScene.FromSnapshot(DiceSceneState.Empty, DiceTraySize.Default);

        // Assert
        Assert.IsEmpty(result.Dice);
    }

    [TestMethod]
    public void FromSnapshot_WithNullScene_ThrowsArgumentNullException()
    {
        // Arrange
        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => RenderScene.FromSnapshot(null!, DiceTraySize.Default);
    }

    [TestMethod]
    public void FromSnapshot_WithNullTraySize_ThrowsArgumentNullException()
    {
        // Arrange
        var scene = CreateScene();

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => RenderScene.FromSnapshot(scene, null!);
    }

    [TestMethod]
    public void FromSnapshot_WithInvalidTraySize_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var scene = CreateScene();

        // Act and Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => RenderScene.FromSnapshot(scene, new DiceTraySize(-1f, 5f));
    }

    [TestMethod]
    public void FromSnapshot_WithInvalidTheme_ThrowsArgumentException()
    {
        // Arrange
        var scene = CreateScene();
        var theme = DiceTheme.Default with { DieColor = "" };

        // Act and Assert
        Assert.ThrowsExactly<ArgumentException>(Act);

        [ExcludeFromCodeCoverage]
        void Act() => RenderScene.FromSnapshot(scene, DiceTraySize.Default, theme);
    }
}
