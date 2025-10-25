using d20Tek.DiceNotation.DieRoller;
using D20Tek.DiceNotation.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace d20Tek.DiceNotation.UnitTests.ExtendedDieRollers;

[TestClass]
public class DieRollTrackerWithCryptoRollerTests
{
    private readonly IDieRollTracker _tracker = new DieRollTracker();
    private readonly IDieRoller _roller;

    public DieRollTrackerWithCryptoRollerTests() => _roller = new CryptoDieRoller(_tracker);

    [TestMethod]
    public async Task DieRollTrackerWithSecureRoller_SingleDieSidesTest()
    {
        // arrange
        _roller.Roll(12);
        _roller.Roll(12);
        _roller.Roll(12);
        _roller.Roll(12);
        _roller.Roll(12);

        // act
        var data = await _tracker.GetTrackingDataAsync();

        // assert
        Assert.HasCount(5, data);
        foreach (var e in data)
        {
            Assert.AreEqual("CryptoDieRoller", e.RollerType);
            Assert.AreEqual("12", e.DieSides);
            AssertHelpers.IsWithinRangeInclusive(1, 12, e.Result);
        }
    }

    [TestMethod]
    public async Task DieRollTrackerWithSecureRoller_MultipleDieSidesTest()
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

        // act
        var data1 = await _tracker.GetTrackingDataAsync(dieSides: "12");
        var data2 = await _tracker.GetTrackingDataAsync(dieSides: "8");
        var data3 = await _tracker.GetTrackingDataAsync(dieSides: "20");

        // assert
        Assert.AreEqual(17, data1.Count + data2.Count + data3.Count);
        Assert.HasCount(4, data1);
        foreach (var e in data1)
        {
            Assert.AreEqual("12", e.DieSides);
            AssertHelpers.IsWithinRangeInclusive(1, 12, e.Result);
        }
        Assert.HasCount(3, data2);
        foreach (var e in data2)
        {
            Assert.AreEqual("8", e.DieSides);
            AssertHelpers.IsWithinRangeInclusive(1, 8, e.Result);
        }
        Assert.HasCount(10, data3);
        foreach (var e in data3)
        {
            Assert.AreEqual("20", e.DieSides);
            AssertHelpers.IsWithinRangeInclusive(1, 20, e.Result);
        }
    }
}
