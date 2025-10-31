using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation;

public interface IDice
{
    IDiceConfiguration Configuration { get; }

    void Clear();

    IDice Dice(int sides, int numberDice = 1, double scalar = 1, int? choose = null, int? exploding = null);

    IDice FudgeDice(int numberDice = 1, int? choose = null);

    IDice Constant(int constant);

    IDice Concat(IDice otherDice);

    DiceResult Roll(IDieRoller? dieRoller = null);

    DiceResult Roll(DiceExpression expresion, IDieRoller? dieRoller = null);

    DiceResult Roll(DiceRequest diceRequest, IDieRoller? dieRoller = null);

    DiceResult Roll(string expression, IDieRoller? dieRoller = null);
}
