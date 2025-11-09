using d20Tek.DiceNotation.DieRoller;
using System.Text.Json.Serialization;

namespace d20Tek.DiceNotation;

public class DiceConfiguration : IDiceConfiguration
{
    public bool HasBoundedResult { get; private set; }

    public int BoundedResultMinimum { get; private set; }

    public int DefaultDieSides { get; private set; }

    public IDieRoller DefaultDieRoller { get; private set; }

    [JsonConstructor]
    public DiceConfiguration(int dieSides, int boundedMinResult, bool hasBoundedResult, IDieRoller? dieRoller = null)
    {
        SetDefaultDieSides(dieSides);
        SetBoundedMinimumResult(boundedMinResult);
        SetHasBoundedResult(hasBoundedResult);
        DefaultDieRoller = dieRoller ?? new RandomDieRoller();
    }

    public DiceConfiguration() 
        : this(Constants.Config.DefaultDieSides, Constants.Config.DefaultBoundedMin, true) { }

    public void SetDefaultDieSides(int dieSides)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(dieSides, Constants.Config.MinDieSides);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(dieSides, Constants.Config.MaxDieSides);
        DefaultDieSides = dieSides;
    }

    public void SetHasBoundedResult(bool hasBoundedResult) => HasBoundedResult = hasBoundedResult;

    public void SetBoundedMinimumResult(int boundedMinResult)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(boundedMinResult, Constants.Config.DefaultBoundedMin);
        BoundedResultMinimum = boundedMinResult;
    }

    public void SetDefaultDieRoller(IDieRoller dieRoller) => DefaultDieRoller = dieRoller;
}
