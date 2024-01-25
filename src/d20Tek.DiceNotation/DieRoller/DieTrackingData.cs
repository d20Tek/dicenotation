//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation.DieRoller;

public class DieTrackingData
{
    public Guid Id { get; set; }

    public string RollerType { get; set; } = string.Empty;

    public string DieSides { get; set; } = string.Empty;

    public int Result { get; set; }

    public DateTime Timpstamp { get; set; }
}
