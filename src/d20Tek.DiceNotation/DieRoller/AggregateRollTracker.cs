//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using System.Reflection;
using System.Text.Json;

namespace d20Tek.DiceNotation.DieRoller;

public class AggregateRollTracker : IAggregateRollTracker
{
    private List<AggregateDieTrackingData> aggRollData = [];

    public void AddDieRoll(int dieSides, int result, Type dieRoller)
    {
        // check input values first
        ArgumentOutOfRangeException.ThrowIfLessThan(dieSides, 2, nameof(dieSides));

        if (result < -1 || result > dieSides)
        {
            throw new ArgumentOutOfRangeException(nameof(result));
        }

        ArgumentNullException.ThrowIfNull(dieRoller, nameof(dieRoller));

        if (typeof(IDieRoller).GetTypeInfo().IsAssignableFrom(dieRoller.GetTypeInfo()) is false)
        {
            throw new ArgumentException("DieRoller is not of type IDieRoller.", nameof(dieRoller));
        }

        var entry = aggRollData.Find(
            p => p.RollerType == dieRoller.Name &&
                 p.DieSides == dieSides.ToString() &&
                 p.Result == result);
        if (entry == null)
        {
            // create an aggregate entry if it doesn't already exist in the list.
            entry = new AggregateDieTrackingData
            {
                RollerType = dieRoller.Name,
                DieSides = dieSides.ToString(),
                Result = result,
                Count = 0,
            };

            aggRollData.Add(entry);
        }

        entry.Count++;
    }

    public void Clear() => aggRollData.Clear();

    public IList<AggregateDieTrackingData> GetFrequencyDataView()
    {
        aggRollData = aggRollData.OrderBy(p => p.RollerType)
                                 .ThenBy(p => p.DieSides)
                                 .ThenBy(p => p.Result).ToList();
        var dieTypes = aggRollData.Select(d => d.RollerType).Distinct();

        // first go through all of the different DieRoller types
        foreach (string t in dieTypes)
        {
            var dieSides = aggRollData.Where(p => p.RollerType == t)
                                      .Select(d => d.DieSides)
                                      .Distinct();

            // then go through all of the different die sides rolled for each roller type
            foreach (string s in dieSides)
            {
                var diceBySides = aggRollData.Where(p => p.RollerType == t && p.DieSides == s);
                var dieResults = diceBySides.Distinct();
                float total = dieResults.Sum(p => p.Count);

                // finally go through all fo the results rolled for each die side
                foreach (var r in dieResults)
                {
                    // calculate the percentage of each entry.
                    r.Percentage = (float)Math.Round(r.Count / total * 100, 1);
                }
            }
        }

        return aggRollData;
    }

    /// <inheritdoc/>
    public void LoadFromJson(string jsonText)
    {
        if (string.IsNullOrEmpty(jsonText)) return;
        aggRollData = JsonSerializer.Deserialize<List<AggregateDieTrackingData>>(jsonText)!;
    }

    /// <inheritdoc/>
    public string ToJson() => JsonSerializer.Serialize(aggRollData);
}
