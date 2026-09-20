using d20Tek.DiceNotation.Results;
using System.Text.Json;

namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Provides a tracker that records individual die rolls and produces statistical views asynchronously.
/// </summary>
public class DieRollTracker : IDieRollTracker
{
    private List<DieTrackingData> rollData = [];

    /// <inheritdoc />
    public int TrackerDataLimit { get; set; } = Constants.DefaultTrackerDataLimit;

    /// <inheritdoc />
    public void AddDieRoll(int dieSides, int result, Type dieRoller)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(dieSides, 2);
        ArgumentOutOfRangeException.ThrowIfLessThan(result, -1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(result, dieSides);
        ArgumentNullException.ThrowIfNull(dieRoller);
        TypeException.ThrowIfNotAssignableFrom<IDieRoller>(dieRoller);

        rollData.Add(DieTrackingData.Create(dieRoller.Name, dieSides, result));
    }

    /// <inheritdoc />
    public async Task<IList<DieTrackingData>> GetTrackingDataAsync(string? dieType = null, string? dieSides = null) =>
        await Task.Run(() => GetTrackingData(dieType, dieSides));

    /// <inheritdoc />
    public void Clear() => rollData.Clear();

    /// <inheritdoc />
    public async Task<string> ToJsonAsync() => await Task.Run(() =>
    {
        rollData = [.. GetTrimmedData()];
        return JsonSerializer.Serialize(rollData);
    });

    /// <inheritdoc />
    public async Task LoadFromJsonAsync(string jsonText) => await Task.Run(() =>
    {
        if (string.IsNullOrEmpty(jsonText)) return;

        var data = JsonSerializer.Deserialize<List<DieTrackingData>>(jsonText)!;
        rollData = [.. data.Take(TrackerDataLimit)];
    });

    /// <inheritdoc />
    public async Task<IList<AggregateDieTrackingData>> GetFrequencyDataViewAsync() =>
        await Task.Run(GetFrequencyDataView);

    private List<DieTrackingData> GetTrackingData(string? dieType = null, string? dieSides = null) =>
        [.. GetTrimmedData().FilterIfNotEmpty(dieType, e => e.RollerType == dieType)
                            .FilterIfNotEmpty(dieSides, e => e.DieSides == dieSides)
                            .OrderBy(e => e.DieSides).ThenBy(e => e.Result)];

    private List<AggregateDieTrackingData> GetFrequencyDataView()
    {
        var results = GetTrackingData()
            .GroupBy(d => d.RollerType)
            .SelectMany(rollerTypes => rollerTypes
                .GroupBy(d => d.DieSides)
                .SelectMany(sides =>
                {
                    var total = (float)sides.Count();

                    return sides.GroupBy(d => d.Result)
                                .Select(resultGroup => new AggregateDieTrackingData
                                {
                                    RollerType = rollerTypes.Key,
                                    DieSides = sides.Key,
                                    Result = resultGroup.Key,
                                    Count = resultGroup.Count(),
                                    Percentage = (float)Math.Round(resultGroup.Count() / total * Constants.Percentage, 1)
                                });
                })
            );

        return [.. results];
    }

    private IEnumerable<DieTrackingData> GetTrimmedData() =>
        rollData.OrderByDescending(d => d.Timpstamp).Take(TrackerDataLimit);
}
