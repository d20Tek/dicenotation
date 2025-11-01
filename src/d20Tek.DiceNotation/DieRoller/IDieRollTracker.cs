namespace d20Tek.DiceNotation.DieRoller;

public interface IDieRollTracker : IAllowRollTrackerEntry
{
    int TrackerDataLimit { get; set; }

    Task<IList<DieTrackingData>> GetTrackingDataAsync(string? dieType = null, string? dieSides = null);

    Task<IList<AggregateDieTrackingData>> GetFrequencyDataViewAsync();

    void Clear();

    Task<string> ToJsonAsync();

    Task LoadFromJsonAsync(string jsonText);
}
