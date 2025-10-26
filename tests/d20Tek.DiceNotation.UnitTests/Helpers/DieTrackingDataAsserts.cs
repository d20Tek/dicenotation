using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.Helpers;

internal static class DieTrackingDataAsserts
{
    public static void AssertTrackingData(
        this IList<DieTrackingData> list,
        int expectedCount,
        string rollerType,
        int sides)
    {
        Assert.HasCount(expectedCount, list);
        list.ToList().ForEach(d => d.AssertTrackingData(rollerType, sides));
    }

    public static void AssertTrackingData(this DieTrackingData data, string rollerType, int sides)
    {
        Assert.AreEqual(rollerType, data.RollerType);
        Assert.AreEqual($"{sides}", data.DieSides);
        AssertHelpers.IsWithinRangeInclusive(1, sides, data.Result);
    }
}
