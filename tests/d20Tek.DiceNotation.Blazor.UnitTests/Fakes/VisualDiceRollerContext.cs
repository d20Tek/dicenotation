namespace d20Tek.DiceNotation.Blazor.UnitTests.Fakes;

using Bunit;
using d20Tek.DiceNotation.Blazor.Components;
using d20Tek.DiceNotation.Results;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Shared setup for <see cref="VisualDiceRoller"/> component tests. Each context registers
/// a controllable loop factory so rendering, framing, and settling are deterministic.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class VisualDiceRollerContext : IDisposable
{
    private readonly BunitContext _context = new();

    public VisualDiceRollerContext(DiceRollerOptions? defaultOptions = null)
    {
        _context.Services.AddSingleton<IDiceAnimationLoopFactory>(Factory);
        _context.Services.AddSingleton(defaultOptions ?? DiceRollerOptions.Default);
    }

    public FakeDiceAnimationLoopFactory Factory { get; } = new();

    public FakeDiceSimulation Simulation => Factory.Simulation;

    public FakeJSObjectReference Module => Factory.Module;

    public BunitContext Context => _context;

    /// <summary>Renders the component with the supplied parameters.</summary>
    public IRenderedComponent<VisualDiceRoller> Render(
        Action<ComponentParameterCollectionBuilder<VisualDiceRoller>>? parameters = null) =>
        _context.Render<VisualDiceRoller>(builder => parameters?.Invoke(builder));

    /// <summary>Creates a completed result containing the supplied d6 values.</summary>
    public static DiceResult ResultWith(params int[] values) =>
        new()
        {
            DiceExpression = $"{values.Length}d6",
            Results = [.. values.Select(v => new TermResult(1, v, "DiceTerm.d6"))],
            Value = values.Sum(),
        };

    /// <summary>Creates a result that has no dice terms, only a constant total.</summary>
    public static DiceResult ModifierOnlyResult(int total) =>
        new() { DiceExpression = total.ToString(), Results = [], Value = total };

    /// <summary>Creates a result representing a failed roll.</summary>
    public static DiceResult FailedResult() =>
        new() { DiceExpression = "bad", Results = [], Value = 0, Error = "invalid notation" };

    public void Dispose() => _context.Dispose();
}
