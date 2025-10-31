namespace d20Tek.DiceNotation;

public interface IDieRoller
{
    int Roll(int sides, int? factor = null);
}
