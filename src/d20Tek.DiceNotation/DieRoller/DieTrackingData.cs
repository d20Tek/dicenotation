namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Represents a single tracked die roll entry.
/// </summary>
public class DieTrackingData
{
    /// <summary>
    /// Gets or sets the unique identifier for this roll entry.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the die roller type that produced the roll.
    /// </summary>
    public string RollerType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of die sides, represented as a string.
    /// </summary>
    public string DieSides { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value produced by the roll.
    /// </summary>
    public int Result { get; set; }

    /// <summary>
    /// Gets or sets the timestamp at which the roll was recorded.
    /// </summary>
    public DateTime Timpstamp { get; set; }

    /// <summary>
    /// Creates a new <see cref="DieTrackingData"/> entry with a generated identifier and current timestamp.
    /// </summary>
    /// <param name="rollerName">The name of the die roller type.</param>
    /// <param name="dieSides">The number of die sides.</param>
    /// <param name="result">The value produced by the roll.</param>
    /// <returns>A new <see cref="DieTrackingData"/> instance.</returns>
    public static DieTrackingData Create(string rollerName, int dieSides, int result) => new()
    {
        Id = Guid.NewGuid(),
        RollerType = rollerName,
        DieSides = dieSides.ToString(),
        Result = result,
        Timpstamp = DateTime.Now,
    };
}
