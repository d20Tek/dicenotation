namespace d20Tek.DiceNotation;

/// <summary>
/// Defines the configurable settings that control default dice behavior.
/// </summary>
public interface IDiceConfiguration
{
    /// <summary>
    /// Gets the default number of sides used when a die notation omits an explicit side count.
    /// </summary>
    int DefaultDieSides { get; }

    /// <summary>
    /// Gets a value indicating whether roll results are bounded by a minimum value.
    /// </summary>
    bool HasBoundedResult { get; }

    /// <summary>
    /// Gets the minimum value that a bounded result can produce.
    /// </summary>
    int BoundedResultMinimum { get; }

    /// <summary>
    /// Gets the die roller used by default when no explicit roller is supplied.
    /// </summary>
    IDieRoller DefaultDieRoller { get; }

    /// <summary>
    /// Sets the default number of sides used when a die notation omits an explicit side count.
    /// </summary>
    /// <param name="dieSides">The default number of die sides.</param>
    void SetDefaultDieSides(int dieSides);

    /// <summary>
    /// Sets whether roll results are bounded by a minimum value.
    /// </summary>
    /// <param name="hasBoundedResult">
    /// <see langword="true"/> to bound results by a minimum value; otherwise, <see langword="false"/>.
    /// </param>
    void SetHasBoundedResult(bool hasBoundedResult);

    /// <summary>
    /// Sets the minimum value that a bounded result can produce.
    /// </summary>
    /// <param name="boundedMinResult">The minimum bounded result value.</param>
    void SetBoundedMinimumResult(int boundedMinResult);

    /// <summary>
    /// Sets the die roller used by default when no explicit roller is supplied.
    /// </summary>
    /// <param name="dieRoller">The default die roller to use.</param>
    void SetDefaultDieRoller(IDieRoller dieRoller);
}