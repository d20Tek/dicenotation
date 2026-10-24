using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;

namespace d20Tek.DiceNotation.Blazor.UnitTests.Fakes;

/// <summary>
/// Records the JavaScript calls made by the renderer so tests can assert on the
/// interop boundary without a browser.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class FakeJSRuntime : IJSRuntime
{
    private readonly FakeJSObjectReference _module = new();

    public List<string> Invocations { get; } = [];

    public FakeJSObjectReference Module => _module;

    public Exception? ImportException { get; set; }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        Invocations.Add(identifier);

        if (identifier == "import")
        {
            if (ImportException is not null)
            {
                throw ImportException;
            }

            return ValueTask.FromResult((TValue)(object)_module);
        }

        return ValueTask.FromResult(default(TValue)!);
    }

    public ValueTask<TValue> InvokeAsync<TValue>(
        string identifier,
        CancellationToken cancellationToken,
        object?[]? args) =>
        InvokeAsync<TValue>(identifier, args);
}

/// <summary>
/// Records calls made against the imported Canvas module.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class FakeJSObjectReference : IJSObjectReference
{
    private int _nextHandle = 1;

    public List<(string Identifier, object?[]? Args)> Calls { get; } = [];

    public bool IsDisposed { get; private set; }

    public Exception? InvokeException { get; set; }

    /// <summary>Gets the exceptions thrown only for specific module functions.</summary>
    public Dictionary<string, Exception> InvokeExceptions { get; } = [];

    /// <summary>Gets the identifiers of every module call, in order.</summary>
    public IEnumerable<string> Identifiers => Calls.Select(c => c.Identifier);

    /// <summary>Gets the number of times a module function was called.</summary>
    public int CountOf(string identifier) => Calls.Count(c => c.Identifier == identifier);

    /// <summary>Gets the arguments of the first call to a module function.</summary>
    public object?[]? ArgsOf(string identifier) =>
        Calls.FirstOrDefault(c => c.Identifier == identifier).Args;

    /// <summary>
    /// Gets the frame callback registered by the most recent <c>startLoop</c> call, which
    /// stands in for the browser animation scheduler.
    /// </summary>
    public DotNetObjectReference<T>? FrameCallbackOf<T>() where T : class =>
        Calls.LastOrDefault(c => c.Identifier == "startLoop").Args?
            .OfType<DotNetObjectReference<T>>()
            .FirstOrDefault();

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        Calls.Add((identifier, args));

        if (InvokeException is not null)
        {
            throw InvokeException;
        }

        if (InvokeExceptions.TryGetValue(identifier, out var scoped))
        {
            throw scoped;
        }

        if (identifier == "initialize" && typeof(TValue) == typeof(int))
        {
            return ValueTask.FromResult((TValue)(object)_nextHandle++);
        }

        // Void interop calls are represented by IJSVoidResult.
        if (typeof(TValue) == typeof(IJSVoidResult))
        {
            return ValueTask.FromResult(default(TValue)!);
        }

        return ValueTask.FromResult(default(TValue)!);
    }

    public ValueTask<TValue> InvokeAsync<TValue>(
        string identifier,
        CancellationToken cancellationToken,
        object?[]? args) =>
        InvokeAsync<TValue>(identifier, args);

    public ValueTask DisposeAsync()
    {
        IsDisposed = true;
        return ValueTask.CompletedTask;
    }
}
