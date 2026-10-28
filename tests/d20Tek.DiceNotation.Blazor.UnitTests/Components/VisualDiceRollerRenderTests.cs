namespace d20Tek.DiceNotation.Blazor.UnitTests.Components;

using Bunit;
using d20Tek.DiceNotation.Blazor.Components;

[TestClass]
public sealed class VisualDiceRollerRenderTests
{
    [TestMethod]
    public void Render_WithNoParameters_ProducesContainerAndCanvas()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        var component = context.Render();

        // Assert
        var container = component.Find("div.d20tek-dice-roller");
        var canvas = component.Find("canvas.d20tek-dice-roller__canvas");
        Assert.IsNotNull(container);
        Assert.IsNotNull(canvas);
    }

    [TestMethod]
    public void Render_WithNoParameters_MarksCanvasAsAnImage()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        var component = context.Render();

        // Assert
        Assert.AreEqual("img", component.Find("canvas").AttributeValue("role"));
    }

    [TestMethod]
    public void Render_WithCssClass_AppendsItToTheContainer()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        var component = context.Render(p => p.Add(c => c.CssClass, "my-tray"));

        // Assert
        var classes = component.Find("div").AttributeValue("class");
        Assert.Contains("d20tek-dice-roller", classes);
        Assert.Contains("my-tray", classes);
    }

    [TestMethod]
    public void Render_WithoutCssClass_StillRendersTheBaseClass()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        var component = context.Render();

        // Assert
        Assert.Contains("d20tek-dice-roller", component.Find("div").AttributeValue("class"));
    }

    [TestMethod]
    public void Render_WithDefaultTray_SetsAspectRatioFromTraySize()
    {
        // Arrange
        var options = DiceRollerOptions.Default with { TraySize = new DiceTraySize(8f, 6f) };
        using var context = new VisualDiceRollerContext();

        // Act
        var component = context.Render(p => p.Add(c => c.Options, options));

        // Assert
        Assert.Contains("aspect-ratio: 8 / 6", component.Find("canvas").AttributeValue("style"));
    }

    [TestMethod]
    public void Render_WithFractionalTraySize_FormatsAspectRatioInvariantly()
    {
        // Arrange
        // A comma decimal separator would produce CSS the browser silently ignores.
        var options = DiceRollerOptions.Default with { TraySize = new DiceTraySize(7.5f, 4.25f) };
        using var context = new VisualDiceRollerContext();

        // Act
        var component = context.Render(p => p.Add(c => c.Options, options));

        // Assert
        var style = component.Find("canvas").AttributeValue("style");
        Assert.Contains("7.5 / 4.25", style);
        Assert.IsFalse(style.Contains(','), "Aspect ratio must not use a comma decimal separator.");
    }

    [TestMethod]
    public void Render_OnFirstRender_CreatesTheLoopOnce()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render();

        // Assert
        Assert.AreEqual(1, context.Factory.CreateCount);
    }

    [TestMethod]
    public void Render_OnFirstRender_InitializesTheCanvas()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        context.Render();

        // Assert
        Assert.AreEqual(1, context.Module.CountOf("initialize"));
    }

    [TestMethod]
    public void Render_WithRegisteredDefaults_PassesThoseSimulationSettingsToTheFactory()
    {
        // Arrange
        var settings = new DiceSimulationSettings { MaxDiceCount = 12 };
        var defaults = DiceRollerOptions.Default with { SimulationSettings = settings };
        using var context = new VisualDiceRollerContext(defaults);

        // Act
        context.Render();

        // Assert
        Assert.AreSame(settings, context.Factory.LastSettings);
    }

    [TestMethod]
    public void Render_WithComponentOptions_PrefersThemOverRegisteredDefaults()
    {
        // Arrange
        var defaults = DiceRollerOptions.Default with { TraySize = new DiceTraySize(10f, 10f) };
        var supplied = DiceRollerOptions.Default with { TraySize = new DiceTraySize(4f, 3f) };
        using var context = new VisualDiceRollerContext(defaults);

        // Act
        var component = context.Render(p => p.Add(c => c.Options, supplied));

        // Assert
        Assert.Contains("4 / 3", component.Find("canvas").AttributeValue("style"));
    }

    [TestMethod]
    public void Render_WithoutComponentOptions_UsesRegisteredDefaults()
    {
        // Arrange
        var defaults = DiceRollerOptions.Default with { TraySize = new DiceTraySize(9f, 2f) };
        using var context = new VisualDiceRollerContext(defaults);

        // Act
        var component = context.Render();

        // Assert
        Assert.Contains("9 / 2", component.Find("canvas").AttributeValue("style"));
    }

    [TestMethod]
    public void IsRolling_BeforeAnyResult_ReturnsFalse()
    {
        // Arrange
        using var context = new VisualDiceRollerContext();

        // Act
        var component = context.Render();

        // Assert
        Assert.IsFalse(component.Instance.IsRolling);
    }
}
