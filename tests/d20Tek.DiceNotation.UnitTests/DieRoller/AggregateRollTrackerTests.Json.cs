using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

[TestClass]
public class AggregateRollTrackerJsonTests
{
    private readonly AggregateRollTracker _tracker = new();

    [TestMethod]
    public void ToJsonTest()
    {
        // arrange
        _tracker.SetupTrackingSampleData();

        // act
        var data = _tracker.ToJson();

        // assert
        Assert.IsFalse(string.IsNullOrEmpty(data));
        Assert.Contains("17", data);
        Assert.Contains("ConstantDieRoller", data);
        Assert.Contains("12", data);
    }

    [TestMethod]
    public void LoadFromJsonTest_NoData()
    {
        // arrange
        string data = string.Empty;

        // act
        var other = new AggregateRollTracker();
        other.LoadFromJson(data);

        // assert
        var list = other.GetFrequencyDataView();
        Assert.IsEmpty(list);
    }

    [TestMethod]
    public void LoadFromJsonTest()
    {
        // arrange
        _tracker.SetupTrackingSampleData();
        var data = _tracker.ToJson();
        Assert.IsFalse(string.IsNullOrEmpty(data));

        // act
        var other = new AggregateRollTracker();
        other.LoadFromJson(data);

        // assert
        var list = other.GetFrequencyDataView();
        Assert.HasCount(22, list);
    }
}
