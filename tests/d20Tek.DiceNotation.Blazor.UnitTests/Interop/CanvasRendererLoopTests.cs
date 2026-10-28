using d20Tek.DiceNotation.Blazor.Interop;
using Microsoft.JSInterop;

namespace d20Tek.DiceNotation.Blazor.UnitTests.Interop;

[TestClass]
public class CanvasRendererLoopTests
{
    private static async Task<(FakeJSRuntime Runtime, CanvasRenderer Renderer)> CreateInitializedAsync()
    {
        var runtime = new FakeJSRuntime();
        var renderer = new CanvasRenderer(runtime);
        await renderer.InitializeAsync(default, CancellationToken.None);

        return (runtime, renderer);
    }

    [TestMethod]
    public async Task StartLoopAsync_WhenInitialized_StartsSchedulerWithCallbackAndClamp()
    {
        // Arrange
        var (runtime, renderer) = await CreateInitializedAsync();
        await using var _ = renderer;
        var callback = new object();

        // Act
        await renderer.StartLoopAsync(callback, 0.1f, CancellationToken.None);

        // Assert
        var args = runtime.Module.ArgsOf("startLoop");
        Assert.AreEqual(1, runtime.Module.CountOf("startLoop"));
        Assert.IsNotNull(args);
        Assert.AreSame(callback, args[1]);
        Assert.AreEqual(0.1f, args[2]);
    }

    [TestMethod]
    public async Task StartLoopAsync_WithNullCallback_ThrowsArgumentNullException()
    {
        // Arrange
        var (_, renderer) = await CreateInitializedAsync();
        await using var disposable = renderer;

        // Act and Assert
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(Act);

        [ExcludeFromCodeCoverage]
        async Task Act() => await renderer.StartLoopAsync(null!, 0.1f, CancellationToken.None);
    }

    [TestMethod]
    public async Task StartLoopAsync_BeforeInitialize_DoesNothing()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        await using var renderer = new CanvasRenderer(runtime);

        // Act
        await renderer.StartLoopAsync(new object(), 0.1f, CancellationToken.None);

        // Assert
        Assert.AreEqual(0, runtime.Module.CountOf("startLoop"));
    }

    [TestMethod]
    public async Task StartLoopAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var (_, renderer) = await CreateInitializedAsync();
        await renderer.DisposeAsync();

        // Act and Assert
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(Act);

        [ExcludeFromCodeCoverage]
        async Task Act() => await renderer.StartLoopAsync(new object(), 0.1f, CancellationToken.None);
    }

    [TestMethod]
    public async Task StartLoopAsync_CalledTwice_SendsTwoStartCalls()
    {
        // Arrange
        var (runtime, renderer) = await CreateInitializedAsync();
        await using var _ = renderer;

        // Act
        await renderer.StartLoopAsync(new object(), 0.1f, CancellationToken.None);
        await renderer.StartLoopAsync(new object(), 0.1f, CancellationToken.None);

        // Assert
        // The JavaScript module cancels the previous registration, so repeated starts
        // are safe and must not be suppressed on the C# side.
        Assert.AreEqual(2, runtime.Module.CountOf("startLoop"));
    }

    [TestMethod]
    public async Task StopLoopAsync_WhenInitialized_StopsScheduler()
    {
        // Arrange
        var (runtime, renderer) = await CreateInitializedAsync();
        await using var _ = renderer;
        await renderer.StartLoopAsync(new object(), 0.1f, CancellationToken.None);

        // Act
        await renderer.StopLoopAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(1, runtime.Module.CountOf("stopLoop"));
    }

    [TestMethod]
    public async Task StopLoopAsync_BeforeInitialize_DoesNothing()
    {
        // Arrange
        var runtime = new FakeJSRuntime();
        await using var renderer = new CanvasRenderer(runtime);

        // Act
        await renderer.StopLoopAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(0, runtime.Module.CountOf("stopLoop"));
    }

    [TestMethod]
    public async Task StopLoopAsync_WhenNoLoopRunning_DoesNotThrow()
    {
        // Arrange
        var (runtime, renderer) = await CreateInitializedAsync();
        await using var _ = renderer;

        // Act
        await renderer.StopLoopAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(1, runtime.Module.CountOf("stopLoop"));
    }

    [TestMethod]
    public async Task StopLoopAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var (_, renderer) = await CreateInitializedAsync();
        await renderer.DisposeAsync();

        // Act and Assert
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(Act);

        [ExcludeFromCodeCoverage]
        async Task Act() => await renderer.StopLoopAsync(CancellationToken.None);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenLoopRunning_StopsLoopBeforeDisposingCanvas()
    {
        // Arrange
        var (runtime, renderer) = await CreateInitializedAsync();
        await renderer.StartLoopAsync(new object(), 0.1f, CancellationToken.None);

        // Act
        await renderer.DisposeAsync();

        // Assert
        var identifiers = runtime.Module.Identifiers.ToList();
        Assert.IsLessThan(identifiers.IndexOf("dispose"), identifiers.IndexOf("stopLoop"));
        Assert.IsTrue(runtime.Module.IsDisposed);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenStopLoopFails_StillCompletesTeardown()
    {
        // Arrange
        var (runtime, renderer) = await CreateInitializedAsync();
        runtime.Module.InvokeExceptions["stopLoop"] = new JSDisconnectedException("gone");

        // Act
        await renderer.DisposeAsync();

        // Assert
        Assert.IsFalse(renderer.IsInitialized);
    }
}
