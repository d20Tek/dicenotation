using d20Tek.DiceNotation.DieRoller;
using System.Threading.Tasks;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

[TestClass]
public class DieRollTrackerJsonTests
{
    private readonly DieRollTracker _tracker = new();

    [TestMethod]
    public async Task DieRollTracker_ToJsonTest()
    {
        // arrange
        _tracker.SetupTrackingSampleData();

        // act
        var data = await _tracker.ToJsonAsync();

        // assert
        Assert.IsFalse(string.IsNullOrEmpty(data));
    }

    [TestMethod]
    public async Task DieRollTracker_LaodFromJsonTest()
    {
        // arrange
        _tracker.SetupTrackingSampleData();
        string data = await _tracker.ToJsonAsync();
        Assert.IsFalse(string.IsNullOrEmpty(data));

        // act
        var other = new DieRollTracker();
        await other.LoadFromJsonAsync(data);

        IList<DieTrackingData> list = await other.GetTrackingDataAsync("RandomDieRoller", "20");

        // assert
        list.AssertTrackingData(14, "RandomDieRoller", 20);
    }

    [TestMethod]
    public async Task DieRollTracker_LaodFromJsonTest_NoData()
    {
        // arrange
        string data = string.Empty;

        // act
        var other = new DieRollTracker();
        await other.LoadFromJsonAsync(data);

        IList<DieTrackingData> list = await other.GetTrackingDataAsync("RandomDieRoller", "20");

        // assert
        Assert.IsEmpty(list);
    }
}
