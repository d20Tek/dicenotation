//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation;

public class DiceConfiguration : IDiceConfiguration
{
    private int defaultDieSides = 6;
    private IDieRoller defaultDieRoller = new RandomDieRoller();

    public bool HasBoundedResult { get; set; } = true;

    public int BoundedResultMinimum { get; set; } = 1;

    public int DefaultDieSides
    {
        get => defaultDieSides;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 2);
            defaultDieSides = value;
        }
    }

    public IDieRoller DefaultDieRoller
    {
        get => defaultDieRoller;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(DefaultDieRoller));
            defaultDieRoller = value;
        }
    }
}
