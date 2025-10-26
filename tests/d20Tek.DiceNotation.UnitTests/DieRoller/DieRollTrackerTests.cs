using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

[TestClass]
public class DieRollTrackerTests
{
    private const string _expectedRollerType = "RandomDieRoller";
    private readonly DieRollTracker _tracker = new();

    [TestMethod]
    public void DieRollTracker_ConstructorTest()
    {
        // arrange

        // act
        var t = new DieRollTracker();

        // assert
        Assert.IsNotNull(t);
        Assert.IsInstanceOfType<IDieRollTracker>(t);
        Assert.IsInstanceOfType<DieRollTracker>(t);
    }

    [TestMethod]
    public async Task DieRollTracker_AddDieRollTest()
    {
        // arrange
        var t = new DieRollTracker
        {
            TrackerDataLimit = 100000
        };

        // act
        t.AddDieRoll(6, 4, typeof(RandomDieRoller));
        IList<DieTrackingData> d = await t.GetTrackingDataAsync();

        // assert
        Assert.IsNotNull(d);
        Assert.HasCount(1, d);
        Assert.AreEqual(_expectedRollerType, d[0].RollerType);
        Assert.AreEqual("6", d[0].DieSides);
        Assert.AreEqual(4, d[0].Result);
        Assert.IsInstanceOfType<Guid>(d[0].Id);
        Assert.IsInstanceOfType<DateTime>(d[0].Timpstamp);
    }

    [TestMethod]
    public async Task DieRollTracker_AddMultipleDieRollTest()
    {
        // arrange

        // act
        _tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 3, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 1, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 6, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 2, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));

        IList<DieTrackingData> d = await _tracker.GetTrackingDataAsync();

        // assert
        d.AssertTrackingData(6, _expectedRollerType, 6);
    }

    [TestMethod]
    public async Task DieRollTracker_AddMultipleDieRollDieSidesTest()
    {
        // arrange

        // act
        _tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 3, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 1, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 6, typeof(RandomDieRoller));
        _tracker.AddDieRoll(8, 2, typeof(RandomDieRoller));
        _tracker.AddDieRoll(8, 4, typeof(RandomDieRoller));

        IList<DieTrackingData> d = await _tracker.GetTrackingDataAsync();

        // assert
        Assert.IsNotNull(d);
        Assert.HasCount(6, d);
        var list1 = d.Take(4).ToList();
        list1.AssertTrackingData(4, _expectedRollerType, 6);
        var list2 = d.TakeLast(2).ToList();
        list2.AssertTrackingData(2, _expectedRollerType, 8);
    }

    [TestMethod]
    public async Task DieRollTracker_AddMultipleDieRollDieTypesTest()
    {
        // arrange

        // act
        _tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 3, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 5, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 6, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 2, typeof(ConstantDieRoller));
        _tracker.AddDieRoll(6, 2, typeof(ConstantDieRoller));

        IList<DieTrackingData> d = await _tracker.GetTrackingDataAsync();

        // assert
        Assert.IsNotNull(d);
        Assert.HasCount(6, d);
        var list1 = d.Take(2).ToList();
        list1.AssertTrackingData(2, "ConstantDieRoller", 6);
        var list2 = d.TakeLast(4).ToList();
        list2.AssertTrackingData(4, _expectedRollerType, 6);
    }

    [TestMethod]
    public async Task DieRollTracker_ClearTest()
    {
        // arrange
        _tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 3, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 1, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 6, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 2, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));

        IList<DieTrackingData> d = await _tracker.GetTrackingDataAsync();
        Assert.IsNotNull(d);

        // act
        _tracker.Clear();

        // assert
        IList<DieTrackingData> r = await _tracker.GetTrackingDataAsync();
        Assert.IsEmpty(r);
    }

    [TestMethod]
    public async Task DieRollTracker_GetTrackingDataAllTest()
    {
        // arrange
        _tracker.SetupTrackingSampleData();

        // act
        IList<DieTrackingData> data = await _tracker.GetTrackingDataAsync();

        // assert
        Assert.HasCount(27, data);
    }

    [TestMethod]
    public async Task DieRollTracker_GetTrackingDataFilterDieTypeTest()
    {
        // arrange
        _tracker.SetupTrackingSampleData();

        // act
        IList<DieTrackingData> data = await _tracker.GetTrackingDataAsync(_expectedRollerType);

        // assert
        Assert.HasCount(19, data);
        data.ToList().ForEach(e => Assert.AreEqual(_expectedRollerType, e.RollerType));
    }

    [TestMethod]
    public async Task DieRollTracker_GetTrackingDataFilterDieSidesTest()
    {
        // arrange
        _tracker.SetupTrackingSampleData();

        // act
        IList<DieTrackingData> data = await _tracker.GetTrackingDataAsync(dieSides: "10");

        // assert
        Assert.HasCount(4, data);
        data.ToList().ForEach(e => Assert.AreEqual("10", e.DieSides));
    }

    [TestMethod]
    public async Task DieRollTracker_GetTrackingDataFilterDieTypeAndSidesTest()
    {
        // arrange
        _tracker.SetupTrackingSampleData();

        // act
        IList<DieTrackingData> data = await _tracker.GetTrackingDataAsync(_expectedRollerType, "20");

        // assert
        data.AssertTrackingData(14, _expectedRollerType, 20);
    }

    [TestMethod]
    public async Task DieRollTracker_GetTrackingDataFilterDieTypeErrorTest()
    {
        // arrange
        _tracker.SetupTrackingSampleData();

        // act
        IList<DieTrackingData> data = await _tracker.GetTrackingDataAsync("FooDieRoller", "20");

        // assert
        Assert.IsEmpty(data);
    }

    [TestMethod]
    public async Task DieRollTracker_GetTrackingDataFilterDieSidesErrorTest()
    {
        // arrange
        _tracker.SetupTrackingSampleData();

        // act
        IList<DieTrackingData> data = await _tracker.GetTrackingDataAsync("RandomDieRoller", "foo");

        // assert
        Assert.IsEmpty(data);
    }

    [TestMethod]
    public async Task DieRollTracker_GetFrequencyDataTest()
    {
        // arrange
        var aggExpected = _tracker.SetupStatisticalTrackingData();

        // act
        IList<AggregateDieTrackingData> data = await _tracker.GetFrequencyDataViewAsync();

        // assert
        IList<DieTrackingData> d = await _tracker.GetTrackingDataAsync();

        Assert.IsNotNull(data);
        Assert.IsInstanceOfType<IList<AggregateDieTrackingData>>(data);
        Assert.HasCount(45, d);
        Assert.HasCount(23, data);

        Assert.HasCount(aggExpected.Count, data);
        for (int i = 0; i < data.Count; i++)
        {
            Assert.AreEqual(aggExpected[i].RollerType, data[i].RollerType, "Failed roller type for item: " + i.ToString());
            Assert.AreEqual(aggExpected[i].DieSides, data[i].DieSides, "Failed die sides for item: " + i.ToString());
            Assert.AreEqual(aggExpected[i].Result, data[i].Result, "Failed result for item: " + i.ToString());
            Assert.AreEqual(aggExpected[i].Count, data[i].Count, "Failed count for item: " + i.ToString());
            Assert.AreEqual(aggExpected[i].Percentage, data[i].Percentage, "Failed percentage for item: " + i.ToString());
        }
    }

    [TestMethod]
    public async Task DieRollTracker_GetFrequencyDataEmptyTest()
    {
        // arrange

        // act
        IList<AggregateDieTrackingData> data = await _tracker.GetFrequencyDataViewAsync();

        // assert
        Assert.IsNotNull(data);
        Assert.IsInstanceOfType<IList<AggregateDieTrackingData>>(data);
        Assert.IsEmpty(data);
    }
}
