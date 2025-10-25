using Microsoft.VisualStudio.TestTools.UnitTesting;
using d20Tek.DiceNotation.DieRoller;
using D20Tek.DiceNotation.UnitTests.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

[TestClass]
public class DieRollTrackerWithRandomRollerTests
{
    private readonly DieRollTracker _tracker = new();
    private readonly IDieRoller _roller;

    public DieRollTrackerWithRandomRollerTests() => _roller = new RandomDieRoller(_tracker);

    [TestMethod]
    public async Task DieRollTrackerWithRandomRoller_SingleDieSidesTest()
    {
        // arrange
        _roller.Roll(12);
        _roller.Roll(12);
        _roller.Roll(12);
        _roller.Roll(12);
        _roller.Roll(12);

        // act
        IList<DieTrackingData> data = await _tracker.GetTrackingDataAsync();

        // validate results
        Assert.HasCount(5, data);
        foreach (DieTrackingData e in data)
        {
            Assert.AreEqual("RandomDieRoller", e.RollerType);
            Assert.AreEqual("12", e.DieSides);
            AssertHelpers.IsWithinRangeInclusive(1, 12, e.Result);
        }
    }

    [TestMethod]
    public async Task DieRollTrackerWithRandomRoller_MultipleDieSidesTest()
    {
        // arrange
        _roller.Roll(12);
        _roller.Roll(12);
        _roller.Roll(12);
        _roller.Roll(12);
        _roller.Roll(8);
        _roller.Roll(8);
        _roller.Roll(8);
        _roller.Roll(20);
        _roller.Roll(20);
        _roller.Roll(20);
        _roller.Roll(20);
        _roller.Roll(20);
        _roller.Roll(20);
        _roller.Roll(20);
        _roller.Roll(20);
        _roller.Roll(20);
        _roller.Roll(20);

        // run test
        IList<DieTrackingData> data1 = await _tracker.GetTrackingDataAsync(dieSides: "12");
        IList<DieTrackingData> data2 = await _tracker.GetTrackingDataAsync(dieSides: "8");
        IList<DieTrackingData> data3 = await _tracker.GetTrackingDataAsync(dieSides: "20");

        // validate results
        Assert.AreEqual(17, data1.Count + data2.Count + data3.Count);
        Assert.HasCount(4, data1);
        foreach (DieTrackingData e in data1)
        {
            Assert.AreEqual("12", e.DieSides);
            AssertHelpers.IsWithinRangeInclusive(1, 12, e.Result);
        }
        Assert.HasCount(3, data2);
        foreach (DieTrackingData e in data2)
        {
            Assert.AreEqual("8", e.DieSides);
            AssertHelpers.IsWithinRangeInclusive(1, 8, e.Result);
        }
        Assert.HasCount(10, data3);
        foreach (DieTrackingData e in data3)
        {
            Assert.AreEqual("20", e.DieSides);
            AssertHelpers.IsWithinRangeInclusive(1, 20, e.Result);
        }
    }
}
