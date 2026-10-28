namespace d20Tek.DiceNotation.Blazor.UnitTests.Components;

using Bunit;

[TestClass]
public sealed class VisualDiceRollerAnimationTests
{
    [TestMethod]
    public void Render_WithResultSuppliedUpFront_StartsTheAnimation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var result = VisualDiceRollerContext.ResultWith(3, 5);

        // Act
        context.Render(p => p.Add(c => c.Result, result));

        // Assert
        // A result assigned before the canvas existed must still animate once.
        Assert.AreEqual(1, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public void Render_WithResultSuppliedUpFront_PassesEveryDieToTheSimulation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var result = VisualDiceRollerContext.ResultWith(3, 5, 1);

        // Act
        context.Render(p => p.Add(c => c.Result, result));

        // Assert
        Assert.AreSequenceEqual([3, 5, 1], [.. context.Simulation.LastDice.Select(d => d.Value)]);
    }

    [TestMethod]
    public void Render_WithResultSuppliedUpFront_StartsTheBrowserScheduler()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));

        // Assert
        Assert.AreEqual(1, context.Module.CountOf("startLoop"));
    }

    [TestMethod]
    public void Render_WithNoResult_DoesNotStartAnAnimation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render();

        // Assert
        Assert.AreEqual(0, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public void SetParameters_WithNewResult_StartsAnotherAnimation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(2)));

        // Act
        component.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(6)));

        // Assert
        Assert.AreEqual(2, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public void SetParameters_WithEqualButDistinctResult_StartsAnotherAnimation()
    {
        // Arrange
        // Rolling the same total twice must still animate, which is why the component
        // compares results by reference rather than by value.
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(3, 4)));

        // Act
        component.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(3, 4)));

        // Assert
        Assert.AreEqual(2, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public void SetParameters_WithTheSameResultInstance_DoesNotRestartTheAnimation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var result = VisualDiceRollerContext.ResultWith(5);
        var component = context.Render(p => p.Add(c => c.Result, result));

        // Act
        component.Render(p => p.Add(c => c.Result, result));

        // Assert
        Assert.AreEqual(1, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public void SetParameters_WithAnUnrelatedParameterChange_DoesNotRestartTheAnimation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var result = VisualDiceRollerContext.ResultWith(5);
        var component = context.Render(p => p.Add(c => c.Result, result));

        // Act
        component.Render(p => p
            .Add(c => c.Result, result)
            .Add(c => c.CssClass, "changed"));

        // Assert
        Assert.AreEqual(1, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public void SetParameters_WithNullResult_DoesNotStartAnAnimation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render();

        // Act
        component.Render(p => p.Add(c => c.Result, null));

        // Assert
        Assert.AreEqual(0, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public void Render_WhenDisabled_DoesNotStartAnAnimation()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render(p => p
            .Add(c => c.Disabled, true)
            .Add(c => c.Result, VisualDiceRollerContext.ResultWith(4)));

        // Assert
        Assert.AreEqual(0, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public void SetParameters_WhenDisabled_DoesNotStartNewAnimations()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Disabled, true));

        // Act
        component.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(1)));

        // Assert
        Assert.AreEqual(0, context.Simulation.StartRollCount);
    }

    [TestMethod]
    public void IsRolling_WhileAnimating_ReturnsTrue()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(2, 2)));

        // Assert
        Assert.IsTrue(component.Instance.IsRolling);
    }

    [TestMethod]
    public async Task IsRolling_AfterTheDiceSettle_ReturnsFalse()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(2)));

        // Act
        await component.InvokeAsync(context.Factory.SettleAsync);

        // Assert
        Assert.IsFalse(component.Instance.IsRolling);
    }

    [TestMethod]
    public async Task Frame_WhileAnimating_RendersTheScene()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();
        var component = context.Render(p => p.Add(c => c.Result, VisualDiceRollerContext.ResultWith(3)));
        var before = context.Module.CountOf("render");

        // Act
        await component.InvokeAsync(() => context.Factory.InvokeFrameAsync());

        // Assert
        Assert.IsGreaterThan(before, context.Module.CountOf("render"));
    }

    [TestMethod]
    public async Task Frame_WithManyDice_SendsOneRenderCallPerFrame()
    {
        // Arrange
        // Interop traffic must stay proportional to frames, not to dice.
        using var context = new VisualDiceRollerContext();
        var result = VisualDiceRollerContext.ResultWith(1, 2, 3, 4, 5, 6, 1, 2);
        var component = context.Render(p => p.Add(c => c.Result, result));
        var before = context.Module.CountOf("render");

        // Act
        await component.InvokeAsync(() => context.Factory.InvokeFrameAsync());

        // Assert
        Assert.AreEqual(before + 1, context.Module.CountOf("render"));
    }
}
