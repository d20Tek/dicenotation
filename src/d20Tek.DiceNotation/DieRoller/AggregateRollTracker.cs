using d20Tek.DiceNotation.Results;
using System.Text.Json;

namespace d20Tek.DiceNotation.DieRoller;

public class AggregateRollTracker : IAggregateRollTracker
{
    private List<AggregateDieTrackingData> _aggRollData = [];

    public void AddDieRoll(int dieSides, int result, Type dieRoller)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(dieSides, 2);
        ArgumentOutOfRangeException.ThrowIfLessThan(result, -1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(result, dieSides);
        ArgumentNullException.ThrowIfNull(dieRoller);
        TypeException.ThrowIfNotAssignableFrom<IDieRoller>(dieRoller);

        GetOrCreateTrackingData(dieSides, result, dieRoller).IncrementCount();
    }

    private AggregateDieTrackingData GetOrCreateTrackingData(int dieSides, int result, Type dieRoller)
    {
        var entry = _aggRollData.Find(x => x.IsEquivalent(dieSides, result, dieRoller));
        if (entry == null)
        {
            entry = AggregateDieTrackingData.Create(dieRoller.Name, dieSides, result);
            _aggRollData.Add(entry);
        }

        return entry;
    }

    public void Clear() => _aggRollData.Clear();

    public IList<AggregateDieTrackingData> GetFrequencyDataView()
    {
        var updated = GetOrderedData()
            .GroupBy(d => d.RollerType)
            .SelectMany(rollerTypes => rollerTypes
                .GroupBy(d => d.DieSides)
                .SelectMany(sides =>
                {
                    float total = sides.Sum(p => p.Count);

                    return sides.Select(entry => new AggregateDieTrackingData
                    {
                        RollerType = entry.RollerType,
                        DieSides = entry.DieSides,
                        Result = entry.Result,
                        Count = entry.Count,
                        Percentage = (float)Math.Round(entry.Count / total * 100, 1)
                    });
                })
            );

        _aggRollData = [.. updated];
        return _aggRollData;
    }

    public void LoadFromJson(string jsonText)
    {
        if (string.IsNullOrEmpty(jsonText)) return;
        _aggRollData = JsonSerializer.Deserialize<List<AggregateDieTrackingData>>(jsonText)!;
    }

    public string ToJson() => JsonSerializer.Serialize(_aggRollData);

    private List<AggregateDieTrackingData> GetOrderedData() =>
        [.. _aggRollData.OrderBy(p => p.RollerType).ThenBy(p => p.DieSides).ThenBy(p => p.Result)];
}
