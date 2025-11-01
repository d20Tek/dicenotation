using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation;

public interface IDice
{
    IDiceConfiguration Configuration { get; }

    DiceResult Roll(DiceExpression expression, IDieRoller? dieRoller = null);

    DiceResult Roll(DiceRequest diceRequest, IDieRoller? dieRoller = null);

    DiceResult Roll(string notation, IDieRoller? dieRoller = null);
}
