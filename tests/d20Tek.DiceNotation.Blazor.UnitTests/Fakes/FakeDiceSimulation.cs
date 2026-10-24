namespace d20Tek.DiceNotation.Blazor.UnitTests.Fakes;

/// <summary>
/// A simulation stand-in that lets tests control settling and failures deterministically,
/// without running real physics.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class FakeDiceSimulation : IDiceSimulation
{
    public bool IsSettled { get; set; }

    public float ElapsedSeconds { get; set; }

    public bool IsDisposed { get; private set; }

    public int StartRollCount { get; private set; }

    public List<float> Steps { get; } = [];

    public int SnapshotCount { get; private set; }

    public IReadOnlyList<VisualDie> LastDice { get; private set; } = [];

    public DiceTraySize? LastTraySize { get; private set; }

    /// <summary>Gets or sets the number of steps after which the scene reports settled.</summary>
    public int SettleAfterSteps { get; set; } = int.MaxValue;

    /// <summary>Gets or sets an exception thrown by <see cref="Step"/>.</summary>
    public Exception? StepException { get; set; }

    public void StartRoll(IReadOnlyList<VisualDie> dice, DiceTraySize traySize)
    {
        StartRollCount++;
        LastDice = dice;
        LastTraySize = traySize;
        IsSettled = false;
        Steps.Clear();
    }

    public int Step(float elapsedSeconds)
    {
        if (StepException is not null)
        {
            throw StepException;
        }

        Steps.Add(elapsedSeconds);
        ElapsedSeconds += elapsedSeconds;

        if (Steps.Count >= SettleAfterSteps)
        {
            IsSettled = true;
        }

        return 1;
    }

    public DiceSceneState GetSnapshot()
    {
        SnapshotCount++;
        var dice = LastDice
            .Select(d => new DieState(d.Id, d.Value, 0f, 0f, 0f, 0.6f, 0.6f))
            .ToArray();

        return new DiceSceneState(dice, ElapsedSeconds, IsSettled);
    }

    public void Dispose() => IsDisposed = true;
}
