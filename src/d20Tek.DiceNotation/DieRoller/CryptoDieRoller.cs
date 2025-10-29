using System.Security.Cryptography;

namespace d20Tek.DiceNotation.DieRoller;

public class CryptoDieRoller(IAllowRollTrackerEntry? tracker = null) : RandomDieRollerBase(tracker)
{
    private static readonly RandomNumberGenerator Generator = RandomNumberGenerator.Create();

    protected override int GetNextRandom(int sides)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(sides, 0);
        return NumberBetween(1, sides);
    }

    private static int NumberBetween(int minValue, int maxValue)
    {
        var asciiValue = GetRandomValue();
        var randomValueInRange = Math.Floor(CalculateMultiplier(asciiValue) * CalculateRange(minValue, maxValue));

        return (int)(minValue + randomValueInRange);
    }

    private static double GetRandomValue()
    {
        var randomNumber = new byte[1];
        Generator.GetBytes(randomNumber);

        return Convert.ToDouble(randomNumber[0]);
    }

    // using Math.Max and subtract 0.0000000000001 to ensure multiplier
    // is within expected range. Otherwise it could be 1 and cause rounding errors
    private static double CalculateMultiplier(double asciiValue) => Math.Max(0, asciiValue / 255d - 0.00000000001d);

    // need to add one to range to allow for rounding with Math.Floor
    private static int CalculateRange(int minValue, int maxValue) => maxValue - minValue + 1;
}
