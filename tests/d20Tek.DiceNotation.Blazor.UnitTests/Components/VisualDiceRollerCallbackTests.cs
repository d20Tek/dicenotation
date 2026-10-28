namespace d20Tek.DiceNotation.Blazor.UnitTests.Components;

using Bunit;
using Microsoft.JSInterop;

[TestClass]
public sealed class VisualDiceRollerCallbackTests
{
    [TestMethod]
    public async Task OnRollCompleted_WhenTheDiceSettle_IsInvokedOnce()
    {
        // Arrange
        var count = 0;
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(4, 2))
            .Add(c => c.OnRollCompleted, _ => count++));

        // Act
        await component.InvokeAsync(context.Factory.SettleAsync);

        // Assert
        Assert.AreEqual(1, count);
    }

    [TestMethod]
    public async Task OnRollCompleted_WhenTheDiceSettle_ReportsTheAuthoritativeTotal()
    {
        // Arrange
        DiceRollSummary? summary = null;
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(4, 2))
            .Add(c => c.OnRollCompleted, s => summary = s));

        // Act
        await component.InvokeAsync(context.Factory.SettleAsync);

        // Assert
        Assert.IsNotNull(summary);
        Assert.AreEqual(6, summary.Total);
    }

    [TestMethod]
    public async Task OnRollCompleted_WhenTheDiceSettle_ReportsEveryDieValue()
    {
        // Arrange
        DiceRollSummary? summary = null;
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(6, 1, 3))
            .Add(c => c.OnRollCompleted, s => summary = s));

        // Act
        await component.InvokeAsync(context.Factory.SettleAsync);

        // Assert
        Assert.IsNotNull(summary);
        CollectionAssert.AreEqual(new[] { 6, 1, 3 }, summary.Dice.Select(d => d.Value).ToArray());
    }

    [TestMethod]
    public async Task OnRollCompleted_BeforeTheDiceSettle_IsNotInvoked()
    {
        // Arrange
        var count = 0;
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(4))
            .Add(c => c.OnRollCompleted, [ExcludeFromCodeCoverage](_) => count++));

        // Act
        await component.InvokeAsync(() => context.Factory.InvokeFrameAsync());

        // Assert
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public void OnRollCompleted_WithAModifierOnlyResult_CompletesImmediately()
    {
        // Arrange
        // A constant expression has nothing to animate but still has a total, so it must
        // complete rather than appear to hang.
        DiceRollSummary? summary = null;
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render(p => p
            .Add(c => c.Result, VisualDiceRollerContext.ModifierOnlyResult(5))
            .Add(c => c.OnRollCompleted, s => summary = s));

        // Assert
        Assert.IsNotNull(summary);
        Assert.AreEqual(5, summary.Total);
    }

    [TestMethod]
    public void OnRollCompleted_WithAModifierOnlyResult_DoesNotStartTheSimulation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ModifierOnlyResult(3)));

        // Assert
        Assert.AreEqual(0, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public void OnRollCompleted_WithNoDelegate_DoesNotThrow()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ModifierOnlyResult(2)));

        // Assert
        Assert.AreEqual(0, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public void OnRollFailed_WithAFailedResult_ReportsTheFailure()
    {
        // Arrange
        Exception? failure = null;
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render(p => p
            .Add(c => c.Result, VisualDiceRollerContext.FailedResult())
            .Add(c => c.OnRollFailed, e => failure = e));

        // Assert
        Assert.IsInstanceOfType<ArgumentException>(failure);
    }

    [TestMethod]
    public void OnRollFailed_WithAFailedResult_DoesNotStartTheSimulation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.FailedResult()));

        // Assert
        Assert.AreEqual(0, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public void OnRollFailed_WhenTheModuleImportFails_ReportsTheFailure()
    {
        // Arrange
        Exception? failure = null;
        using var context = new VisualDiceRollerContext();
        context.Factory.JSRuntime.ImportException = new JSException("module missing");

        // Act
        context.Render(p => p.Add(c => c.OnRollFailed, e => failure = e));

        // Assert
        Assert.IsInstanceOfType<JSException>(failure);
    }

    [TestMethod]
    public void OnRollFailed_WhenTheLoopFactoryThrows_ReportsTheFailure()
    {
        // Arrange
        Exception? failure = null;
        using var context = new VisualDiceRollerContext();
        context.Factory.CreateException = new InvalidOperationException("no loop");

        // Act
        context.Render(p => p.Add(c => c.OnRollFailed, e => failure = e));

        // Assert
        Assert.IsInstanceOfType<InvalidOperationException>(failure);
    }

    [TestMethod]
    public void OnRollFailed_WithInvalidOptions_ReportsTheFailure()
    {
        // Arrange
        Exception? failure = null;
        var options = DiceRollerOptions.Default with { TraySize = new DiceTraySize(-1f, 6f) };
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render(p => p
            .Add(c => c.Options, options)
            .Add(c => c.OnRollFailed, e => failure = e));

        // Assert
        Assert.IsInstanceOfType<ArgumentOutOfRangeException>(failure);
    }

    [TestMethod]
    public void OnRollFailed_WhenStartingTheSimulationThrows_ReportsTheFailure()
    {
        // Arrange
        Exception? failure = null;
        using var context = new VisualDiceRollerContext();
        context.Simulation.StartRollException = new ArgumentException("too many dice");

        // Act
        context.Render(p => p
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(1, 2))
            .Add(c => c.OnRollFailed, e => failure = e));

        // Assert
        Assert.IsInstanceOfType<ArgumentException>(failure);
    }

    [TestMethod]
    public async Task OnRollFailed_WhenAFrameThrows_ReportsTheFailure()
    {
        // Arrange
        Exception? failure = null;
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(4))
            .Add(c => c.OnRollFailed, e => failure = e));
        context.Simulation.StepException = new InvalidOperationException("bad frame");

        // Act
        await component.InvokeAsync(() => context.Factory.InvokeFrameAsync());

        // Assert
        Assert.IsInstanceOfType<InvalidOperationException>(failure);
    }

    [TestMethod]
    public void OnRollFailed_WithNoDelegate_DoesNotThrow()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        context.Factory.JSRuntime.ImportException = new JSException("module missing");

        // Act
        var component = context.Render();

        // Assert
        // A visual failure with no handler must not tear down the component.
        Assert.IsNotNull(component.Find("canvas"));
    }

    [TestMethod]
    public void OnRollFailed_WhenInitializationFails_StillRendersTheCanvas()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        context.Factory.JSRuntime.ImportException = new JSException("module missing");

        // Act
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(5)));

        // Assert
        Assert.IsFalse(component.Instance.IsRolling);
    }
}
