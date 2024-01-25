//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation;

public interface IDiceConfiguration
{
    int DefaultDieSides { get; set; }

    bool HasBoundedResult { get; set; }

    int BoundedResultMinimum { get; set; }

    IDieRoller DefaultDieRoller { get; set; }
}