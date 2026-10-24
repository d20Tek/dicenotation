namespace d20Tek.DiceNotation.Blazor.UnitTests.Fakes;

/// <summary>
/// Builds <see cref="VisualDie"/> collections for simulation tests.
/// </summary>
internal static class VisualDiceFactory
{
    public static IReadOnlyList<VisualDie> Create(int count, int value = 4) =>
        [.. Enumerable.Range(0, count).Select(i => new VisualDie($"die-{i}", "d6", value))];

    public static IReadOnlyList<VisualDie> CreateFromValues(params int[] values) =>
        [.. values.Select((v, i) => new VisualDie($"die-{i}", "d6", v))];
}
