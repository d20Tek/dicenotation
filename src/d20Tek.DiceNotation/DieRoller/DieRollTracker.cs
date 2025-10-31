using d20Tek.DiceNotation.Results;
using System.Text.Json;

namespace d20Tek.DiceNotation.DieRoller;

public class DieRollTracker : IDieRollTracker
{
    private const int DefaultTrackerDataLimit = 250000;
    private List<DieTrackingData> rollData = [];

    public int TrackerDataLimit { get; set; } = DefaultTrackerDataLimit;

    public void AddDieRoll(int dieSides, int result, Type dieRoller)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(dieSides, 2);
        ArgumentOutOfRangeException.ThrowIfLessThan(result, -1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(result, dieSides);
        ArgumentNullException.ThrowIfNull(dieRoller);
        TypeException.ThrowIfNotAssignableFrom<IDieRoller>(dieRoller);

        rollData.Add(DieTrackingData.Create(dieRoller.Name, dieSides, result));
    }

    public async Task<IList<DieTrackingData>> GetTrackingDataAsync(string? dieType = null, string? dieSides = null) =>
        await Task.Run(() => GetTrackingData(dieType, dieSides));

    public void Clear() => rollData.Clear();

    public async Task<string> ToJsonAsync() => await Task.Run(() =>
    {
        rollData = [.. GetTrimmedData()];
        return JsonSerializer.Serialize(rollData);
    });

    public async Task LoadFromJsonAsync(string jsonText) => await Task.Run(() =>
    {
        if (string.IsNullOrEmpty(jsonText)) return;

        var data = JsonSerializer.Deserialize<List<DieTrackingData>>(jsonText)!;
        rollData = [.. data.Take(TrackerDataLimit)];
    });

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
                                    Percentage = (float)Math.Round(resultGroup.Count() / total * 100, 1)
                                });
                })
            );

        return [.. results];
    }

    private IEnumerable<DieTrackingData> GetTrimmedData() =>
        rollData.OrderByDescending(d => d.Timpstamp).Take(TrackerDataLimit);
}
