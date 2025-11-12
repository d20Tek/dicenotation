using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.Results;

[ExcludeFromCodeCoverage]
internal class MockDiceConfiguration : IDiceConfiguration
{
    public int DefaultDieSides => 6;

    public bool HasBoundedResult => true;

    public int BoundedResultMinimum => 1;

    public IDieRoller DefaultDieRoller => new RandomDieRoller();

    public void SetBoundedMinimumResult(int boundedMinResult) { }

    public void SetDefaultDieRoller(IDieRoller dieRoller) { }
        
    public void SetDefaultDieSides(int dieSides) { }
        
    public void SetHasBoundedResult(bool hasBoundedResult) { }
}
