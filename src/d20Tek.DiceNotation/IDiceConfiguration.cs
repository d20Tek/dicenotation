namespace d20Tek.DiceNotation;

public interface IDiceConfiguration
{
    int DefaultDieSides { get; }

    bool HasBoundedResult { get; }

    int BoundedResultMinimum { get; }

    IDieRoller DefaultDieRoller { get; }

    void SetDefaultDieSides(int dieSides);

    void SetHasBoundedResult(bool hasBoundedResult);

    void SetBoundedMinimumResult(int boundedMinResult);

    void SetDefaultDieRoller(IDieRoller dieRoller);
}