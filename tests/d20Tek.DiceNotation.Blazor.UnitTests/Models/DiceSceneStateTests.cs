namespace d20Tek.DiceNotation.Blazor.UnitTests.Models;

[TestClass]
public class DiceSceneStateTests
{
    [TestMethod]
    public void Empty_WhenAccessed_ReturnsNoDiceAndUnsettledScene()
    {
        // Arrange
        // Act
        var result = DiceSceneState.Empty;

        // Assert
        Assert.IsEmpty(result.Dice);
        Assert.AreEqual(0f, result.ElapsedSeconds);
        Assert.IsFalse(result.IsSettled);
    }

    [TestMethod]
    public void Constructor_WithDiceStates_RetainsSuppliedValues()
    {
        // Arrange
        var dice = new[] { new DieState("die-0", 5, 1f, 2f, 0.5f, 0.6f, 0.6f) };

        // Act
        var result = new DiceSceneState(dice, 1.25f, true);

        // Assert
        Assert.HasCount(1, result.Dice);
        Assert.AreEqual("die-0", result.Dice[0].Id);
        Assert.AreEqual(5, result.Dice[0].Value);
        Assert.AreEqual(1.25f, result.ElapsedSeconds);
        Assert.IsTrue(result.IsSettled);
    }
}
