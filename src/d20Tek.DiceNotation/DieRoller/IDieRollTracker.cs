namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Defines a tracker that records individual die rolls and provides statistical views asynchronously.
/// </summary>
public interface IDieRollTracker : IAllowRollTrackerEntry
{
    /// <summary>
    /// Gets or sets the maximum number of roll entries retained by the tracker.
    /// </summary>
    int TrackerDataLimit { get; set; }

    /// <summary>
    /// Asynchronously gets the tracked die roll data, optionally filtered by die type and sides.
    /// </summary>
    /// <param name="dieType">The die roller type to filter by, or <see langword="null"/> for all types.</param>
    /// <param name="dieSides">The die side count to filter by, or <see langword="null"/> for all sizes.</param>
    /// <returns>A task that resolves to the list of matching <see cref="DieTrackingData"/> entries.</returns>
    Task<IList<DieTrackingData>> GetTrackingDataAsync(string? dieType = null, string? dieSides = null);

    /// <summary>
    /// Asynchronously gets a view of aggregated die roll frequency data.
    /// </summary>
    /// <returns>A task that resolves to the list of <see cref="AggregateDieTrackingData"/> entries.</returns>
    Task<IList<AggregateDieTrackingData>> GetFrequencyDataViewAsync();

    /// <summary>
    /// Removes all tracked roll data.
    /// </summary>
    void Clear();

    /// <summary>
    /// Asynchronously serializes the tracked roll data to a JSON string.
    /// </summary>
    /// <returns>A task that resolves to the JSON representation of the tracked data.</returns>
    Task<string> ToJsonAsync();

    /// <summary>
    /// Asynchronously loads tracked roll data from a JSON string.
    /// </summary>
    /// <param name="jsonText">The JSON text to load.</param>
    /// <returns>A task that represents the asynchronous load operation.</returns>
    Task LoadFromJsonAsync(string jsonText);
}
