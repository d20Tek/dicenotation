using d20Tek.DiceNotation.Blazor.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace d20Tek.DiceNotation.Blazor.UnitTests.Interop;

[TestClass]
public class CanvasRendererTests
{
    private static DiceSceneState CreateScene() =>
        new([new DieState("die-0", 4, 0f, 0f, 0f, 0.6f, 0.6f)], 0.5f, false);

    [TestMethod]
    public void Constructor_WithNullJsRuntime_ThrowsArgumentNullException()
    {
        // Arrange
        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        static void Act() => _ = new CanvasRenderer(null!);
    }

    [TestMethod]
    public void IsInitialized_BeforeInitializeAsync_ReturnsFalse()
    {
        // Arrange
        var runtime = new FakeJSRuntime();

        // Act
        var renderer = new CanvasRenderer(runtime);

        // Assert
        Assert.IsFalse(renderer.IsInitialized);
    }

    [TestMethod]
    public async Task InitializeAsync_WhenCalled_ImportsModuleAndInitializesCanvas()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        await using var renderer = new CanvasRenderer(runtime);

        // Act
        await renderer.InitializeAsync(default, CancellationToken.None);

        // Assert
        Assert.Contains("import", runtime.Invocations);
        Assert.AreEqual("initialize", runtime.Module.Calls[0].Identifier);
        Assert.IsTrue(renderer.IsInitialized);
    }

    [TestMethod]
    public async Task InitializeAsync_CalledTwice_ImportsModuleOnlyOnce()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        await using var renderer = new CanvasRenderer(runtime);

        // Act
        await renderer.InitializeAsync(default, CancellationToken.None);
        await renderer.InitializeAsync(default, CancellationToken.None);

        // Assert
        Assert.ContainsSingle(i => i == "import", runtime.Invocations);
        Assert.ContainsSingle(c => c.Identifier == "initialize", runtime.Module.Calls);
    }

    [TestMethod]
    public async Task RenderAsync_WhenInitialized_SendsSingleBatchedCall()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        await using var renderer = new CanvasRenderer(runtime);
        await renderer.InitializeAsync(default, CancellationToken.None);

        // Act
        await renderer.RenderAsync(CreateScene(), DiceTraySize.Default, null, null, CancellationToken.None);

        // Assert
        var renderCalls = runtime.Module.Calls.Where(c => c.Identifier == "render").ToList();
        Assert.HasCount(1, renderCalls);
        Assert.IsInstanceOfType<RenderScene>(renderCalls[0].Args![1]);
    }

    [TestMethod]
    public async Task RenderAsync_WithMultipleDice_StillSendsOneCall()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        await using var renderer = new CanvasRenderer(runtime);
        await renderer.InitializeAsync(default, CancellationToken.None);
        var scene = new DiceSceneState(
            [.. Enumerable.Range(0, 12).Select(i => new DieState($"die-{i}", 1, 0f, 0f, 0f, 0.6f, 0.6f))],
            1f,
            true);

        // Act
        await renderer.RenderAsync(scene, DiceTraySize.Default, null, null, CancellationToken.None);

        // Assert
        Assert.ContainsSingle(c => c.Identifier == "render", runtime.Module.Calls);
    }

    [TestMethod]
    public async Task RenderAsync_BeforeInitialize_DoesNothing()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        await using var renderer = new CanvasRenderer(runtime);

        // Act
        await renderer.RenderAsync(CreateScene(), DiceTraySize.Default, null, null, CancellationToken.None);

        // Assert
        Assert.IsEmpty(runtime.Module.Calls);
    }

    [TestMethod]
    public async Task RenderAsync_WithNullScene_ThrowsArgumentNullException()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        await using var renderer = new CanvasRenderer(runtime);

        // Act and Assert
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        async Task Act() => await renderer.RenderAsync(null!, DiceTraySize.Default, null, null, CancellationToken.None);
    }

    [TestMethod]
    public async Task RenderAsync_WithNullTraySize_ThrowsArgumentNullException()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        await using var renderer = new CanvasRenderer(runtime);

        // Act and Assert
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        async Task Act() => await renderer.RenderAsync(CreateScene(), null!, null, null, CancellationToken.None);
    }

    [TestMethod]
    public async Task ResizeAsync_WhenInitialized_InvokesResize()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        await using var renderer = new CanvasRenderer(runtime);
        await renderer.InitializeAsync(default, CancellationToken.None);

        // Act
        await renderer.ResizeAsync(CancellationToken.None);

        // Assert
        Assert.ContainsSingle(c => c.Identifier == "resize", runtime.Module.Calls);
    }

    [TestMethod]
    public async Task ResizeAsync_BeforeInitialize_DoesNothing()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        await using var renderer = new CanvasRenderer(runtime);

        // Act
        await renderer.ResizeAsync(CancellationToken.None);

        // Assert
        Assert.IsEmpty(runtime.Module.Calls);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenInitialized_DisposesCanvasAndModule()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);
        await renderer.InitializeAsync(default, CancellationToken.None);

        // Act
        await renderer.DisposeAsync();

        // Assert
        Assert.ContainsSingle(c => c.Identifier == "dispose", runtime.Module.Calls);
        Assert.IsTrue(runtime.Module.IsDisposed);
        Assert.IsFalse(renderer.IsInitialized);
    }

    [TestMethod]
    public async Task DisposeAsync_BeforeInitialize_DoesNotThrow()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);

        // Act
        await renderer.DisposeAsync();

        // Assert
        Assert.IsFalse(runtime.Module.IsDisposed);
    }

    [TestMethod]
    public async Task DisposeAsync_CalledTwice_DisposesModuleOnce()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);
        await renderer.InitializeAsync(default, CancellationToken.None);

        // Act
        await renderer.DisposeAsync();
        await renderer.DisposeAsync();

        // Assert
        Assert.ContainsSingle(c => c.Identifier == "dispose", runtime.Module.Calls);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenBrowserDisconnected_SwallowsException()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);
        await renderer.InitializeAsync(default, CancellationToken.None);
        runtime.Module.InvokeException = new JSDisconnectedException("circuit gone");

        // Act
        await renderer.DisposeAsync();

        // Assert
        Assert.IsFalse(renderer.IsInitialized);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenModuleAlreadyDisposed_SwallowsException()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);
        await renderer.InitializeAsync(default, CancellationToken.None);
        runtime.Module.InvokeException = new ObjectDisposedException("module");

        // Act
        await renderer.DisposeAsync();

        // Assert
        Assert.IsFalse(renderer.IsInitialized);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenOperationCanceled_SwallowsException()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);
        await renderer.InitializeAsync(default, CancellationToken.None);
        runtime.Module.InvokeException = new OperationCanceledException();

        // Act
        await renderer.DisposeAsync();

        // Assert
        Assert.IsFalse(renderer.IsInitialized);
    }

    [TestMethod]
    public async Task InitializeAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);
        await renderer.DisposeAsync();

        // Act and Assert
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(Act);

        [ExcludeFromCodeCoverage]
        async Task Act() => await renderer.InitializeAsync(default, CancellationToken.None);
    }

    [TestMethod]
    public async Task RenderAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);
        await renderer.DisposeAsync();

        // Act and Assert
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(Act);

        [ExcludeFromCodeCoverage]
        async Task Act() => await renderer.RenderAsync(CreateScene(), DiceTraySize.Default, null, null, CancellationToken.None);
    }

    [TestMethod]
    public async Task ResizeAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);
        await renderer.DisposeAsync();

        // Act and Assert
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(Act);

        [ExcludeFromCodeCoverage]
        async Task Act() => await renderer.ResizeAsync(CancellationToken.None);
    }
}
