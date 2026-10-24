using d20Tek.DiceNotation.Blazor.Interop;

namespace d20Tek.DiceNotation.Blazor.UnitTests.Interop;

[TestClass]
public class RenderSceneEnergyTests
{
    private static DiceSceneState CreateScene(params float[] energies) =>
        new(
            [.. energies.Select((e, i) => new DieState($"die-{i}", i + 1, 0f, 0f, 0f, 0.6f, 0.6f, e))],
            0.5f,
            false);

    [TestMethod]
    public void FromSnapshot_WithMovingDice_PreservesEnergyPerDie()
    {
        // Arrange
        var scene = CreateScene(0f, 0.25f, 1f);

        // Act
        var payload = RenderScene.FromSnapshot(scene, DiceTraySize.Default);

        // Assert
        Assert.AreEqual(0f, payload.Dice[0].Energy);
        Assert.AreEqual(0.25f, payload.Dice[1].Energy);
        Assert.AreEqual(1f, payload.Dice[2].Energy);
    }

    [TestMethod]
    public void FromSnapshot_WithDefaultDieState_DefaultsEnergyToZero()
    {
        // Arrange
        var scene = new DiceSceneState(
            [new DieState("die-0", 4, 0f, 0f, 0f, 0.6f, 0.6f)],
            0.5f,
            true);

        // Act
        var payload = RenderScene.FromSnapshot(scene, DiceTraySize.Default);

        // Assert
        Assert.AreEqual(0f, payload.Dice[0].Energy);
    }

    [TestMethod]
    public void FromSnapshot_WhenEnergyVaries_DoesNotAlterDieValues()
    {
        // Arrange
        // Energy is a presentation hint only and must never disturb the results.
        var scene = CreateScene(0.9f, 0.1f);

        // Act
        var payload = RenderScene.FromSnapshot(scene, DiceTraySize.Default);

        // Assert
        Assert.AreEqual(1, payload.Dice[0].Value);
        Assert.AreEqual(2, payload.Dice[1].Value);
    }
}
