namespace d20Tek.DiceNotation.Blazor.UnitTests.Components;

using Bunit;
using Microsoft.JSInterop;

[TestClass]
public sealed class VisualDiceRollerApiTests
{
    [TestMethod]
    public async Task ReplayAsync_WithACurrentResult_StartsAnotherAnimation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(3)));

        // Act
        await component.InvokeAsync(() => component.Instance.ReplayAsync(CancellationToken.None));

        // Assert
        Assert.AreEqual(2, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public async Task ReplayAsync_WithACurrentResult_ReusesTheSameValues()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(2, 6)));

        // Act
        await component.InvokeAsync(() => component.Instance.ReplayAsync(CancellationToken.None));

        // Assert
        Assert.AreSequenceEqual([2, 6], [.. context.Simulation.LastDice.Select(d => d.Value)]);
    }

    [TestMethod]
    public async Task ReplayAsync_WithNoResult_DoesNothing()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render();

        // Act
        await component.InvokeAsync(() => component.Instance.ReplayAsync(CancellationToken.None));

        // Assert
        Assert.AreEqual(0, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public async Task ReplayAsync_WhenDisabled_DoesNothing()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p
            .Add(c => c.Disabled, true)
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));

        // Act
        await component.InvokeAsync(() => component.Instance.ReplayAsync(CancellationToken.None));

        // Assert
        Assert.AreEqual(0, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public async Task ReplayAsync_AfterDisposal_DoesNothing()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));
        var instance = component.Instance;
        await component.InvokeAsync(() => instance.DisposeAsync().AsTask());

        // Act
        await component.InvokeAsync(() => instance.ReplayAsync(CancellationToken.None));

        // Assert
        Assert.AreEqual(1, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public async Task ReplayAsync_WithCancellation_DoesNotThrow()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(5)));
        using var cts = new CancellationTokenSource();

        // Act
        await component.InvokeAsync(() => component.Instance.ReplayAsync(cts.Token));

        // Assert
        Assert.AreEqual(2, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public async Task StopAsync_WhileAnimating_StopsTheScheduler()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(3)));

        // Act
        await component.InvokeAsync(() => component.Instance.StopAsync(CancellationToken.None));

        // Assert
        Assert.IsFalse(component.Instance.IsRolling);
    }

    [TestMethod]
    public async Task StopAsync_WhileAnimating_CallsStopLoopOnTheModule()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(3)));

        // Act
        await component.InvokeAsync(() => component.Instance.StopAsync(CancellationToken.None));

        // Assert
        Assert.IsGreaterThanOrEqualTo(1, context.Module.CountOf("stopLoop"));
    }

    [TestMethod]
    public async Task StopAsync_WhenNotAnimating_DoesNotThrow()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render();

        // Act
        await component.InvokeAsync(() => component.Instance.StopAsync(CancellationToken.None));

        // Assert
        Assert.IsFalse(component.Instance.IsRolling);
    }

    [TestMethod]
    public async Task StopAsync_WhileAnimating_DoesNotRaiseCompletion()
    {
        // Arrange
        var count = 0;
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(3))
            .Add(c => c.OnRollCompleted, [ExcludeFromCodeCoverage](_) => count++));

        // Act
        await component.InvokeAsync(() => component.Instance.StopAsync(CancellationToken.None));

        // Assert
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public async Task ResizeAsync_AfterInitialization_ResizesTheCanvas()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));

        // Act
        await component.InvokeAsync(() => component.Instance.ResizeAsync(CancellationToken.None));

        // Assert
        Assert.AreEqual(1, context.Module.CountOf("resize"));
    }

    [TestMethod]
    public async Task ResizeAsync_AfterInitialization_RedrawsTheCurrentScene()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));
        var before = context.Module.CountOf("render");

        // Act
        await component.InvokeAsync(() => component.Instance.ResizeAsync(CancellationToken.None));

        // Assert
        Assert.IsGreaterThan(before, context.Module.CountOf("render"));
    }

    [TestMethod]
    public async Task ResizeAsync_WhenNoAnimationHasStarted_DoesNotThrow()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render();

        // Act
        await component.InvokeAsync(() => component.Instance.ResizeAsync(CancellationToken.None));

        // Assert
        Assert.AreEqual(1, context.Module.CountOf("resize"));
    }

    [TestMethod]
    public async Task ResizeAsync_WhenTheLoopWasNeverCreated_DoesNothing()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        context.Factory.CreateException = new InvalidOperationException("no loop");
        var component = context.Render();

        // Act
        await component.InvokeAsync(() => component.Instance.ResizeAsync(CancellationToken.None));

        // Assert
        Assert.AreEqual(0, context.Module.CountOf("resize"));
    }

    [TestMethod]
    public async Task ReplayAsync_WhenTheLoopWasNeverCreated_DoesNotStartAnAnimation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        context.Factory.CreateException = new InvalidOperationException("no loop");
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(3)));

        // Act
        await component.InvokeAsync(() => component.Instance.ReplayAsync(CancellationToken.None));

        // Assert
        Assert.AreEqual(0, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public async Task ReplayAsync_WhenTheCanvasNeverInitialized_DoesNotStartAnAnimation()
    {
        // Arrange
        // The loop exists here because the factory succeeded, but the module import failed,
        // so replaying would push dice at a canvas that was never set up.
        using var context = new VisualDiceRollerContext();
        context.Factory.JSRuntime.ImportException = new JSException("module missing");
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(3)));

        // Act
        await component.InvokeAsync(() => component.Instance.ReplayAsync(CancellationToken.None));

        // Assert
        Assert.IsNotNull(context.Factory.Loop);
        Assert.AreEqual(0, context.Simulation.StartRollCount);
    }
}
