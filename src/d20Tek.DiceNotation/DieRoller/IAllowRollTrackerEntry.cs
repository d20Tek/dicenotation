//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation.DieRoller;

public interface IAllowRollTrackerEntry
{
    void AddDieRoll(int dieSides, int result, Type dieRoller);
}
