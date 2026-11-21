using d20Tek.DiceNotation.DieRoller;
using Microsoft.Extensions.DependencyInjection;

namespace d20Tek.DiceNotation.AotSmoke;

/// <summary>
/// Runs the library's public API end to end in a Native AOT binary, so features that compile cleanly
/// but fail at runtime without reflection (such as JSON serialization) still fail the CI job.
/// </summary>
internal static class Program
{
    private const int SuccessExitCode = 0;
    private const int FailureExitCode = 1;
    private const int ConstantRoll = 2;
    private const string Notation = "4d6k3+(1+1)d8+5";
    private const int NotationTotal = 15;
    private const int ExpressionTotal = 11;
    private const int BatchCount = 10;
    private const int D20 = 20;
    private const int Seed = 42;
    private const int ReplayLength = 5;

    private static async Task<int> Main()
    {
        SmokeCheck[] checks =
        [
            new("Roll notation", Sync(RollsNotation)),
            new("Roll fluent expression", Sync(RollsExpression)),
            new("Roll many", Sync(RollsMany)),
            new("Random rollers", Sync(RandomRollersStayInRange)),
            new("Die roll tracker JSON", DieRollTrackerRoundTripsJsonAsync),
            new("Aggregate tracker JSON", Sync(AggregateTrackerRoundTripsJson)),
            new("Seeded replay", Sync(ReplayTokensReproduceRolls)),
            new("Dependency injection", Sync(ResolvesFromServiceProvider)),
        ];

        var failures = 0;
        foreach (var check in checks)
        {
            var passed = await RunCheckAsync(check);
            failures += passed ? 0 : 1;
        }

        Console.WriteLine($"{checks.Length - failures} of {checks.Length} AOT smoke checks passed.");
        return failures == 0 ? SuccessExitCode : FailureExitCode;
    }

    /// <summary>Wraps a synchronous check so it runs like the asynchronous ones.</summary>
    private static Func<Task<bool>> Sync(Func<bool> check) => () => Task.FromResult(check());

    /// <summary>Runs one check, reporting an exception as a failure, and prints its outcome.</summary>
    private static async Task<bool> RunCheckAsync(SmokeCheck check)
    {
        bool passed;
        try
        {
            passed = await check.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  {check.Name}: {ex.GetType().Name}: {ex.Message}");
            passed = false;
        }

        Console.WriteLine($"{(passed ? "PASS" : "FAIL")} {check.Name}");
        return passed;
    }

    /// <summary>Parses and evaluates notation with grouping and keep-highest through the lexer and parser.</summary>
    private static bool RollsNotation() =>
        new Dice().Roll(Notation, new ConstantDieRoller(ConstantRoll)) is { HasError: false, Value: NotationTotal };

    /// <summary>Rolls an expression built with the fluent API.</summary>
    private static bool RollsExpression()
    {
        var expression = DiceExpression.Create().AddDice(6, 4, choose: 3).AddConstant(5);

        return new Dice().Roll(expression, new ConstantDieRoller(ConstantRoll)).Value == ExpressionTotal;
    }

    /// <summary>Rolls the same notation as a batch.</summary>
    private static bool RollsMany()
    {
        var results = new Dice().RollMany("d20", BatchCount);

        return results.Count == BatchCount && results.All(r => !r.HasError);
    }

    /// <summary>Rolls a d20 with each random-number roller and checks the value is a valid face.</summary>
    private static bool RandomRollersStayInRange()
    {
        IDieRoller[] rollers = [new RandomDieRoller(), new CryptoDieRoller(), new MathNetDieRoller()];

        return rollers.All(roller => roller.Roll(D20) is >= 1 and <= D20);
    }

    /// <summary>Saves die roll tracking data to JSON and loads it into a new tracker unchanged.</summary>
    private static async Task<bool> DieRollTrackerRoundTripsJsonAsync()
    {
        var tracker = new DieRollTracker();
        new Dice().RollMany("3d6", BatchCount, new RandomDieRoller(tracker));
        var json = await tracker.ToJsonAsync();

        var reloaded = new DieRollTracker();
        await reloaded.LoadFromJsonAsync(json);

        return (await reloaded.GetTrackingDataAsync()).Count > 0 && await reloaded.ToJsonAsync() == json;
    }

    /// <summary>Saves aggregate tracking data to JSON and loads it into a new tracker unchanged.</summary>
    private static bool AggregateTrackerRoundTripsJson()
    {
        var tracker = new AggregateRollTracker();
        new Dice().RollMany("3d6", BatchCount, new MathNetDieRoller(tracker));
        var json = tracker.ToJson();

        var reloaded = new AggregateRollTracker();
        reloaded.LoadFromJson(json);

        // Compare before reading the view, because GetFrequencyDataView rewrites the stored data.
        return reloaded.ToJson() == json && reloaded.GetFrequencyDataView().Count > 0;
    }

    /// <summary>Replays each seedable algorithm from its replay token and gets the same sequence.</summary>
    private static bool ReplayTokensReproduceRolls() =>
        Enum.GetValues<DieRollerAlgorithm>().All(algorithm =>
        {
            var original = DieRollerFactory.CreateSeeded(Seed, algorithm);
            var token = original.ReplayToken ?? throw new InvalidOperationException($"No token for {algorithm}.");
            var replay = DieRollerFactory.CreateFromReplayToken(token);

            return RollSequence(original).SequenceEqual(RollSequence(replay));
        });

    /// <summary>Rolls a fixed number of d20s with the given roller.</summary>
    private static int[] RollSequence(IDieRoller roller) =>
        [.. Enumerable.Range(0, ReplayLength).Select(_ => roller.Roll(D20))];

    /// <summary>Registers the library with a roller and tracker, then rolls through the resolved services.</summary>
    private static bool ResolvesFromServiceProvider()
    {
        using var provider = new ServiceCollection()
            .AddDiceNotation(options => options.UseMathNetDieRoller().UseAggregateRollTracker())
            .BuildServiceProvider();
        using var scope = provider.CreateScope();

        var result = scope.ServiceProvider.GetRequiredService<IDice>().Roll("d20");
        var tracker = scope.ServiceProvider.GetRequiredService<IAggregateRollTracker>();

        return !result.HasError && tracker.GetFrequencyDataView().Count > 0;
    }
}

/// <summary>A named smoke check that reports whether it passed.</summary>
internal readonly record struct SmokeCheck(string Name, Func<Task<bool>> Run);
