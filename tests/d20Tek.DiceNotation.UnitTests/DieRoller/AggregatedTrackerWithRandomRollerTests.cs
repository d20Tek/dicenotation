using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

[TestClass]
public class AggregatedTrackerWithRandomRollerTests
{
    private readonly AggregateRollTracker _tracker = new();
    private readonly IDieRoller _roller;

    public AggregatedTrackerWithRandomRollerTests() => _roller = new RandomDieRoller(_tracker);

    [TestMethod]
    public void SingleDieSidesTest()
    {
        // arrange
        int[] rolls = _roller.RollMultiple(5, 12);
        int expected = rolls.Distinct().Count();

        // act
        var data = _tracker.GetFrequencyDataView();

        // assert
        Assert.HasCount(expected, data);
        foreach (AggregateDieTrackingData e in data)
        {
            Assert.AreEqual("RandomDieRoller", e.RollerType);
            Assert.AreEqual("12", e.DieSides);
        }
    }

    [TestMethod]
    public void MultipleDieSidesTest()
    {
        // arrange
        int[] rolls12 = _roller.RollMultiple(4, 12);
        var expected12 = rolls12.Distinct().Count();

        int[] rolls8 = _roller.RollMultiple(3, 8);
        var expected8 = rolls8.Distinct().Count();

        int[] rolls20 = _roller.RollMultiple(10, 20);
        var expected20 = rolls20.Distinct().Count();

        // act
        var data = _tracker.GetFrequencyDataView();
        var list12s = data.Where(p => p.DieSides == "12").ToArray();
        var list8s = data.Where(p => p.DieSides == "8").ToArray();
        var list20s = data.Where(p => p.DieSides == "20").ToArray();

        // assert
        Assert.HasCount(expected12, list12s);
        Assert.HasCount(expected8, list8s);
        Assert.HasCount(expected20, list20s);
    }
}
