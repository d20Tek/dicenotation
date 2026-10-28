namespace d20Tek.DiceNotation.Blazor.UnitTests.Components;

using Bunit;
using d20Tek.DiceNotation.Blazor.Components;
using Microsoft.JSInterop;

[TestClass]
public sealed class VisualDiceRollerLifecycleTests
{
    private static async Task DisposeAsync(IRenderedComponent<VisualDiceRoller> component)
    {
        var instance = component.Instance;
        await component.InvokeAsync(() => instance.DisposeAsync().AsTask());
    }

    [TestMethod]
    public async Task DisposeAsync_AfterRendering_DisposesTheSimulation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));

        // Act
        await DisposeAsync(component);

        // Assert
        Assert.IsTrue(context.Simulation.IsDisposed);
    }

    [TestMethod]
    public async Task DisposeAsync_AfterRendering_ReleasesTheModule()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));

        // Act
        await DisposeAsync(component);

        // Assert
        Assert.IsTrue(context.Module.IsDisposed);
    }

    [TestMethod]
    public async Task DisposeAsync_WhileAnimating_StopsTheSchedulerBeforeReleasingTheModule()
    {
        // Arrange
        // A queued frame must not be able to run against disposed .NET state.
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));

        // Act
        await DisposeAsync(component);

        // Assert
        var identifiers = context.Module.Identifiers.ToList();
        var stopIndex = identifiers.LastIndexOf("stopLoop");
        var disposeIndex = identifiers.LastIndexOf("dispose");
        Assert.IsGreaterThanOrEqualTo(0, stopIndex);
        Assert.IsLessThan(disposeIndex, stopIndex);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenCalledTwice_DoesNotThrow()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));
        await DisposeAsync(component);

        // Act
        await DisposeAsync(component);

        // Assert
        Assert.IsTrue(context.Simulation.IsDisposed);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenNoAnimationEverStarted_DoesNotThrow()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render();

        // Act
        await DisposeAsync(component);

        // Assert
        Assert.IsTrue(context.Simulation.IsDisposed);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenTheLoopWasNeverCreated_DoesNotThrow()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        context.Factory.CreateException = new InvalidOperationException("no loop");
        var component = context.Render();

        // Act
        await DisposeAsync(component);

        // Assert
        Assert.IsFalse(context.Simulation.IsDisposed);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenTheBrowserIsDisconnected_DoesNotThrow()
    {
        // Arrange
        // Disposal commonly races with circuit teardown and navigation.
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));
        context.Module.InvokeException = new JSDisconnectedException("circuit gone");

        // Act
        await DisposeAsync(component);

        // Assert
        Assert.IsTrue(context.Simulation.IsDisposed);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenTheModuleThrowsJSException_DoesNotThrow()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));
        context.Module.InvokeException = new JSException("interop failed");

        // Act
        await DisposeAsync(component);

        // Assert
        Assert.IsTrue(context.Simulation.IsDisposed);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenReleasingTheModuleThrows_DoesNotThrow()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));
        context.Module.DisposeException = new JSDisconnectedException("circuit gone");

        // Act
        await DisposeAsync(component);

        // Assert
        Assert.IsTrue(context.Simulation.IsDisposed);
    }

    [TestMethod]
    public async Task DisposeAsync_AfterRendering_ReleasesTheFrameCallback()
    {
        // Arrange
        // Releasing the object reference is what stops the browser from reaching
        // disposed .NET state through a queued frame.
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));

        // Act
        await DisposeAsync(component);

        // Assert
        Assert.IsFalse(context.Factory.IsFrameCallbackReachable);
    }

    [TestMethod]
    public async Task Frame_AfterDisposal_DoesNotStepTheSimulation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));
        await DisposeAsync(component);
        var before = context.Simulation.Steps.Count;

        // Act
        await context.Factory.InvokeFrameAsync();

        // Assert
        Assert.HasCount(before, context.Simulation.Steps);
    }

    [TestMethod]
    public async Task OnRollCompleted_AfterDisposal_IsNotInvoked()
    {
        // Arrange
        var count = 0;
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(4))
            .Add(c => c.OnRollCompleted, [ExcludeFromCodeCoverage](_) => count++));
        await DisposeAsync(component);

        // Act
        await context.Factory.SettleAsync();

        // Assert
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public async Task StopAsync_AfterDisposal_DoesNotThrow()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));
        var instance = component.Instance;
        await DisposeAsync(component);

        // Act
        await instance.StopAsync(CancellationToken.None);

        // Assert
        Assert.IsFalse(instance.IsRolling);
    }

    [TestMethod]
    public async Task ResizeAsync_AfterDisposal_DoesNotThrow()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));
        var instance = component.Instance;
        await DisposeAsync(component);
        var before = context.Module.CountOf("resize");

        // Act
        await instance.ResizeAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(before, context.Module.CountOf("resize"));
    }

    [TestMethod]
    public void Render_WithTwoComponents_GivesEachItsOwnLoop()
    {
        // Arrange
        // Several rollers must be able to coexist on one page.
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(1)));
        context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(2)));

        // Assert
        Assert.AreEqual(2, context.Factory.CreateCount);
    }

    [TestMethod]
    public void Render_WithTwoComponents_InitializesEachCanvas()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render();
        context.Render();

        // Assert
        Assert.AreEqual(2, context.Module.CountOf("initialize"));
    }
}
