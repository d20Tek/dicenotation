using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

[TestClass]
public class DieRollTrackerWithRandomRollerTests
{
    private const string _expectedRollerType = "RandomDieRoller";
    private readonly DieRollTracker _tracker = new();
    private readonly IDieRoller _roller;

    public DieRollTrackerWithRandomRollerTests() => _roller = new RandomDieRoller(_tracker);

    [TestMethod]
    public async Task DieRollTrackerWithRandomRoller_SingleDieSidesTest()
    {
        // arrange
        _roller.RollMultiple(5, 12);

        // act
        IList<DieTrackingData> data = await _tracker.GetTrackingDataAsync();

        // assert
        data.AssertTrackingData(5, _expectedRollerType, 12);
    }

    [TestMethod]
    public async Task DieRollTrackerWithRandomRoller_MultipleDieSidesTest()
    {
        // arrange
        _roller.RollMultiple(4, 12);
        _roller.RollMultiple(3, 8);
        _roller.RollMultiple(10, 20);

        // act
        IList<DieTrackingData> data1 = await _tracker.GetTrackingDataAsync(dieSides: "12");
        IList<DieTrackingData> data2 = await _tracker.GetTrackingDataAsync(dieSides: "8");
        IList<DieTrackingData> data3 = await _tracker.GetTrackingDataAsync(dieSides: "20");

        // assert
        Assert.AreEqual(17, data1.Count + data2.Count + data3.Count);
        data1.AssertTrackingData(4, _expectedRollerType, 12);
        data2.AssertTrackingData(3, _expectedRollerType, 8);
        data3.AssertTrackingData(10, _expectedRollerType, 20);
    }
}
