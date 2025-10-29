namespace d20Tek.DiceNotation.DieRoller;

public interface IAggregateRollTracker : IAllowRollTrackerEntry
{
    IList<AggregateDieTrackingData> GetFrequencyDataView();

    void Clear();

    string ToJson();

    void LoadFromJson(string jsonText);
}
