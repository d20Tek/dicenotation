//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using System.Security.Cryptography;

namespace d20Tek.DiceNotation.DieRoller;

public class CryptoDieRoller : RandomDieRollerBase
{
    private static readonly RandomNumberGenerator Generator = RandomNumberGenerator.Create();

    public CryptoDieRoller(IAllowRollTrackerEntry? tracker = null)
        : base(tracker)
    {
    }

    protected override int GetNextRandom(int sides)
    {
        return NumberBetween(1, sides);
    }

    private int NumberBetween(int minValue, int maxValue)
    {
        var randomNumber = new byte[1];
        Generator.GetBytes(randomNumber);

        var asciiValue = Convert.ToDouble(randomNumber[0]);

        // using Math.Max and subtract 0.0000000000001 to ensure multiplier
        // is within expected range. Otherwise it could be 1 and cause rounding errors
        var multiplier = Math.Max(0, asciiValue / 255d - 0.00000000001d);

        // need to add one to range to allow for rounding with Math.Floor
        int range = maxValue - minValue + 1;

        double randomValueInRange = Math.Floor(multiplier * range);

        return (int)(minValue + randomValueInRange);
    }
}
