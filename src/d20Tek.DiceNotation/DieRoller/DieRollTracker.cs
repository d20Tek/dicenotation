//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using System.Reflection;
using System.Text.Json;

namespace d20Tek.DiceNotation.DieRoller;

public class DieRollTracker : IDieRollTracker
{
    private const int DefaultTrackerDataLimit = 250000;
    private List<DieTrackingData> rollData = [];

    public int TrackerDataLimit { get; set; } = DefaultTrackerDataLimit;

    public void AddDieRoll(int dieSides, int result, Type dieRoller)
    {
        // check input values first
        ArgumentOutOfRangeException.ThrowIfLessThan(dieSides, 2);

        if (result < -1 || result > dieSides)
        {
            throw new ArgumentOutOfRangeException(nameof(result));
        }

        ArgumentNullException.ThrowIfNull(dieRoller);

        if (typeof(IDieRoller).GetTypeInfo().IsAssignableFrom(dieRoller.GetTypeInfo()) is false)
        {
            throw new ArgumentException(nameof(dieRoller));
        }

        // create tracking data entry
        DieTrackingData entry = new DieTrackingData
        {
            Id = Guid.NewGuid(),
            RollerType = dieRoller.Name,
            DieSides = dieSides.ToString(),
            Result = result,
            Timpstamp = DateTime.Now,
        };

        // save it to list
        rollData.Add(entry);
    }

    public async Task<IList<DieTrackingData>> GetTrackingDataAsync(
        string? dieType = null,
        string? dieSides = null)
    {
        return await Task.Run(() => GetTrackingData(dieType, dieSides));
    }

    public void Clear()
    {
        rollData.Clear();
    }

    public async Task<string> ToJsonAsync()
    {
        return await Task.Run(() =>
        {
            rollData = GetTrimmedData().ToList();
            return JsonSerializer.Serialize(rollData);
        });
    }

    public async Task LoadFromJsonAsync(string jsonText)
    {
        await Task.Run(() =>
        {
            if (string.IsNullOrEmpty(jsonText)) return;

            var data = JsonSerializer.Deserialize<List<DieTrackingData>>(jsonText)!;
            rollData = data.Take(TrackerDataLimit).ToList();
        });
    }

    public async Task<IList<AggregateDieTrackingData>> GetFrequencyDataViewAsync()
    {
        return await Task.Run(() => GetFrequencyDataView());
    }

    private IList<DieTrackingData> GetTrackingData(string? dieType = null, string? dieSides = null)
    {
        IEnumerable<DieTrackingData> result = GetTrimmedData();

        if (!string.IsNullOrEmpty(dieType))
        {
            result = result.Where(e => e.RollerType == dieType);
        }

        if (!string.IsNullOrEmpty(dieSides))
        {
            result = result.Where(e => e.DieSides == dieSides);
        }

        return result.OrderBy(e => e.DieSides).ThenBy(e => e.Result).ToList();
    }

    private IList<AggregateDieTrackingData> GetFrequencyDataView()
    {
        IList<AggregateDieTrackingData> results = new List<AggregateDieTrackingData>();
        var dieTypes = GetTrackingData().Select(d => d.RollerType).Distinct();

        // first go through all of the different DieRoller types
        foreach (string t in dieTypes)
        {
            var dieSides = GetTrackingData(t).Select(d => d.DieSides).Distinct();

            // then go through all of the different die sides rolled for each roller type
            foreach (string s in dieSides)
            {
                var diceBySides = GetTrackingData(t, s);
                var dieResults = diceBySides.Select(d => d.Result).Distinct();
                float total = diceBySides.Count;

                // finally go through all fo the results rolled for each die side
                foreach (int r in dieResults)
                {
                    int count = diceBySides.Count(d => d.Result == r);
                    AggregateDieTrackingData aggCount = new AggregateDieTrackingData
                    {
                        RollerType = t,
                        DieSides = s,
                        Result = r,
                        Count = count,
                        Percentage = (float)Math.Round(count / total * 100, 1),
                    };

                    // add that data into a view for each roller type, sides, result combination
                    results.Add(aggCount);
                }
            }
        }

        return results;
    }

    private IEnumerable<DieTrackingData> GetTrimmedData()
    {
        return rollData.OrderByDescending(d => d.Timpstamp).Take(TrackerDataLimit);
    }
}
