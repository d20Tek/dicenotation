namespace d20Tek.DiceNotation.Blazor.UnitTests.Components;

using Bunit;

[TestClass]
public sealed class VisualDiceRollerAccessibilityTests
{
    [TestMethod]
    public void AriaLabel_BeforeAnyRoll_DescribesAnEmptyTray()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        var component = context.Render();

        // Assert
        Assert.AreEqual("Dice tray", component.Find("canvas").AttributeValue("aria-label"));
    }

    [TestMethod]
    public async Task AriaLabel_AfterTheDiceSettle_DescribesTheOutcome()
    {
        // Arrange
        // A canvas is opaque to assistive technology, so the result must be in the label.
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(5, 3)));

        // Act
        await component.InvokeAsync(context.Factory.SettleAsync);

        // Assert
        var label = component.Find("canvas").AttributeValue("aria-label");
        Assert.Contains("2 dice", label);
        Assert.Contains("5, 3", label);
        Assert.Contains("8", label);
    }

    [TestMethod]
    public async Task AriaLabel_AfterASecondRoll_ReflectsTheNewValues()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(1)));
        await component.InvokeAsync(context.Factory.SettleAsync);

        // Act
        component.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(6)));
        await component.InvokeAsync(context.Factory.SettleAsync);

        // Assert
        Assert.Contains("6", component.Find("canvas").AttributeValue("aria-label"));
    }

    [TestMethod]
    public void AriaLabel_WhenSuppliedExplicitly_OverridesTheGeneratedLabel()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        var component = context.Render(p => p.Add(c => c.AriaLabel, "Attack roll"));

        // Assert
        Assert.AreEqual("Attack roll", component.Find("canvas").AttributeValue("aria-label"));
    }

    [TestMethod]
    public async Task AriaLabel_WhenSuppliedExplicitly_IsKeptAfterTheDiceSettle()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p
            .Add(c => c.AriaLabel, "Attack roll")
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(4, 4)));

        // Act
        await component.InvokeAsync(context.Factory.SettleAsync);

        // Assert
        Assert.AreEqual("Attack roll", component.Find("canvas").AttributeValue("aria-label"));
    }

    [TestMethod]
    public void AriaLabel_WhenSuppliedAsEmpty_UsesThatEmptyValue()
    {
        // Arrange
        // An explicitly empty label is a deliberate choice, not a missing one.
        using var context = new VisualDiceRollerContext();

        // Act
        var component = context.Render(p => p.Add(c => c.AriaLabel, string.Empty));

        // Assert
        Assert.AreEqual(string.Empty, component.Find("canvas").AttributeValue("aria-label"));
    }

    [TestMethod]
    public async Task AriaLabel_WithASingleDie_DescribesThatDie()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(2)));

        // Act
        await component.InvokeAsync(context.Factory.SettleAsync);

        // Assert
        Assert.Contains("1 dice", component.Find("canvas").AttributeValue("aria-label"));
    }

    [TestMethod]
    public void ReducedMotion_WhenEnabled_StillRendersTheTray()
    {
        // Arrange
        var options = DiceRollerOptions.Default with { ReducedMotion = true };
        using var context = new VisualDiceRollerContext();

        // Act
        var component = context.Render(p => p
            .Add(c => c.Options, options)
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(3, 3)));

        // Assert
        Assert.IsNotNull(component.Find("canvas"));
    }

    [TestMethod]
    public void ReducedMotion_WhenEnabled_StillAnimatesTheDice()
    {
        // Arrange
        // Reduced motion removes effects, not the roll itself.
        var options = DiceRollerOptions.Default with { ReducedMotion = true };
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render(p => p
            .Add(c => c.Options, options)
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(3, 3)));

        // Assert
        Assert.AreEqual(1, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public async Task ReducedMotion_WhenEnabled_ProducesTheSameResult()
    {
        // Arrange
        DiceRollSummary? summary = null;
        var options = DiceRollerOptions.Default with { ReducedMotion = true };
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p
            .Add(c => c.Options, options)
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(6, 2))
            .Add(c => c.OnRollCompleted, s => summary = s));

        // Act
        await component.InvokeAsync(context.Factory.SettleAsync);

        // Assert
        Assert.IsNotNull(summary);
        Assert.AreEqual(8, summary.Total);
    }
}
