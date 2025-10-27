using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.UnitTests.Helpers;

internal static class DiceAsserts
{
    public static void AssertConstruction(this Dice dice)
    {
        Assert.IsNotNull(dice);
        Assert.IsInstanceOfType<IDice>(dice);
        Assert.IsInstanceOfType<Dice>(dice);
        Assert.IsTrue(string.IsNullOrEmpty(dice.ToString()));
        Assert.IsTrue(dice.Configuration.HasBoundedResult);
        Assert.AreEqual(1, dice.Configuration.BoundedResultMinimum);
    }

    public static void AssertNotation(this IDice dice, string notation)
    {
        Assert.IsNotNull(dice);
        Assert.IsInstanceOfType<IDice>(dice);
        Assert.IsInstanceOfType<Dice>(dice);
        Assert.AreEqual(notation, dice.ToString());
    }
}
