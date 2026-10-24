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
internal sealed class FakeJSObjectReference : IJSObjectReference
{
    private int _nextHandle = 1;

    public List<(string Identifier, object?[]? Args)> Calls { get; } = [];

    public bool IsDisposed { get; private set; }

    public Exception? InvokeException { get; set; }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        Calls.Add((identifier, args));

        if (InvokeException is not null)
        {
            throw InvokeException;
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
