namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Defines an in-memory tracker that aggregates die roll frequency data.
/// </summary>
public interface IAggregateRollTracker : IAllowRollTrackerEntry
{
    /// <summary>
    /// Gets a view of aggregated die roll frequency data.
    /// </summary>
    /// <returns>A list of <see cref="AggregateDieTrackingData"/> entries.</returns>
    IList<AggregateDieTrackingData> GetFrequencyDataView();

    /// <summary>
    /// Removes all tracked roll data.
    /// </summary>
    void Clear();

    /// <summary>
    /// Serializes the tracked roll data to a JSON string.
    /// </summary>
    /// <returns>The JSON representation of the tracked data.</returns>
    string ToJson();

    /// <summary>
    /// Loads tracked roll data from a JSON string.
    /// </summary>
    /// <param name="jsonText">The JSON text to load.</param>
    void LoadFromJson(string jsonText);
}
