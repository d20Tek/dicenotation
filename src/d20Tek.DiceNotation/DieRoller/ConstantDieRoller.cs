//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation.DieRoller;

public class ConstantDieRoller : IDieRoller
{
    private const int DefaultRollValue = 1;
    private readonly int constantRollValue;

    public ConstantDieRoller(int rollValue = DefaultRollValue)
    {
        constantRollValue = rollValue;
    }

    /// <inheritdoc/>
    public int Roll(int sides, int? factor = null) => constantRollValue;
}
