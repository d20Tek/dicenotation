using d20Tek.DiceNotation.DieRoller;
using System.Text.Json.Serialization;

namespace d20Tek.DiceNotation;

public class DiceConfiguration : IDiceConfiguration
{
    private const int _minDieSides = 2;
    private const int _maxDieSides = 1000;
    private const int _defaultBoundedMin = 1;
    private const int _defaultDieSides = 6;

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

    public DiceConfiguration() : this(_defaultDieSides, _defaultBoundedMin, true) { }

    public void SetDefaultDieSides(int dieSides)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(dieSides, _minDieSides);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(dieSides, _maxDieSides);
        DefaultDieSides = dieSides;
    }

    public void SetHasBoundedResult(bool hasBoundedResult) => HasBoundedResult = hasBoundedResult;

    public void SetBoundedMinimumResult(int boundedMinResult)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(boundedMinResult, _defaultBoundedMin);
        BoundedResultMinimum = boundedMinResult;
    }

    public void SetDefaultDieRoller(IDieRoller dieRoller) => DefaultDieRoller = dieRoller;
}
