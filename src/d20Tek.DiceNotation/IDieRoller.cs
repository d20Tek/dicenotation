﻿//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation;

public interface IDieRoller
{
    int Roll(int sides, int? factor = null);
}
