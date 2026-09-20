namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Represents aggregated frequency data for die rolls grouped by roller type, die sides, and result.
/// </summary>
public class AggregateDieTrackingData
{
    /// <summary>
    /// Gets or sets the name of the die roller type that produced the tracked rolls.
    /// </summary>
    public string RollerType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of die sides, represented as a string.
    /// </summary>
    public string DieSides { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the roll result value that this data aggregates.
    /// </summary>
    public int Result { get; set; }

    /// <summary>
    /// Gets or sets the number of times the result was rolled.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the percentage of rolls that produced this result within its group.
    /// </summary>
    public float Percentage { get; set; }

    /// <summary>
    /// Creates a new <see cref="AggregateDieTrackingData"/> entry with zeroed counts.
    /// </summary>
    /// <param name="rollerType">The name of the die roller type.</param>
    /// <param name="dieSides">The number of die sides.</param>
    /// <param name="result">The roll result value.</param>
    /// <returns>A new <see cref="AggregateDieTrackingData"/> instance.</returns>
    public static AggregateDieTrackingData Create(string rollerType, int dieSides, int result) => new()
    {
        RollerType = rollerType,
        DieSides = dieSides.ToString(),
        Result = result,
        Count = 0,
        Percentage = 0f
    };

    internal void IncrementCount() => Count++;

    internal bool IsEquivalent(int dieSides, int result, Type dieRollerType) => 
        RollerType == dieRollerType.Name && DieSides == dieSides.ToString() && Result == result;
}
