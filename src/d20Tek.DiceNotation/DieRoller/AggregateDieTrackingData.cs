//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation.DieRoller;

public class AggregateDieTrackingData
{
    public string RollerType { get; set; } = string.Empty;

    public string DieSides { get; set; } = string.Empty;

    public int Result { get; set; }

    public int Count { get; set; }

    public float Percentage { get; set; }
}
