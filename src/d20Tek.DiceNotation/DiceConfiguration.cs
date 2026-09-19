using d20Tek.DiceNotation.DieRoller;
using System.Text.Json.Serialization;

namespace d20Tek.DiceNotation;

/// <summary>
/// Provides the default implementation of dice configuration settings.
/// </summary>
public class DiceConfiguration : IDiceConfiguration
{
    /// <inheritdoc />
    public bool HasBoundedResult { get; private set; }

    /// <inheritdoc />
    public int BoundedResultMinimum { get; private set; }

    /// <inheritdoc />
    public int DefaultDieSides { get; private set; }

    /// <inheritdoc />
    public IDieRoller DefaultDieRoller { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiceConfiguration"/> class with the specified settings.
    /// </summary>
    /// <param name="dieSides">The default number of die sides.</param>
    /// <param name="boundedMinResult">The minimum value that a bounded result can produce.</param>
    /// <param name="hasBoundedResult">
    /// <see langword="true"/> to bound results by a minimum value; otherwise, <see langword="false"/>.
    /// </param>
    /// <param name="dieRoller">
    /// The default die roller, or <see langword="null"/> to use a new <see cref="RandomDieRoller"/>.
    /// </param>
    [JsonConstructor]
    public DiceConfiguration(int dieSides, int boundedMinResult, bool hasBoundedResult, IDieRoller? dieRoller = null)
    {
        SetDefaultDieSides(dieSides);
        SetBoundedMinimumResult(boundedMinResult);
        SetHasBoundedResult(hasBoundedResult);
        DefaultDieRoller = dieRoller ?? new RandomDieRoller();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiceConfiguration"/> class with default settings.
    /// </summary>
    public DiceConfiguration() 
        : this(Constants.Config.DefaultDieSides, Constants.Config.DefaultBoundedMin, true) { }

    /// <inheritdoc />
    public void SetDefaultDieSides(int dieSides)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(dieSides, Constants.Config.MinDieSides);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(dieSides, Constants.Config.MaxDieSides);
        DefaultDieSides = dieSides;
    }

    /// <inheritdoc />
    public void SetHasBoundedResult(bool hasBoundedResult) => HasBoundedResult = hasBoundedResult;

    /// <inheritdoc />
    public void SetBoundedMinimumResult(int boundedMinResult)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(boundedMinResult, Constants.Config.DefaultBoundedMin);
        BoundedResultMinimum = boundedMinResult;
    }

    /// <inheritdoc />
    public void SetDefaultDieRoller(IDieRoller dieRoller) => DefaultDieRoller = dieRoller;
}
