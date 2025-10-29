namespace d20Tek.DiceNotation.DieRoller;

public class ConstantDieRoller(int rollValue = ConstantDieRoller.DefaultRollValue) : IDieRoller
{
    private const int DefaultRollValue = 1;
    private readonly int constantRollValue = rollValue;

    public int Roll(int sides, int? factor = null) => constantRollValue;
}
