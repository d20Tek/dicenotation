using d20Tek.DiceNotation.DieRoller;
using System.Threading.Tasks;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

[TestClass]
public class DieRollTrackerTests
{
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
        Assert.AreEqual("RandomDieRoller", d[0].RollerType);
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
        Assert.IsNotNull(d);
        Assert.HasCount(6, d);
        foreach (DieTrackingData e in d)
        {
            Assert.AreEqual("RandomDieRoller", e.RollerType);
            Assert.AreEqual("6", e.DieSides);
            AssertHelpers.IsWithinRangeInclusive(1, 6, e.Result);
        }
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
        int i;
        for (i = 0; i < 4; i++)
        {
            DieTrackingData e = d[i];
            Assert.AreEqual("RandomDieRoller", e.RollerType);
            Assert.AreEqual("6", e.DieSides);
            AssertHelpers.IsWithinRangeInclusive(1, 6, e.Result);
        }

        for (int x = i; x < 5; x++)
        {
            DieTrackingData e = d[x];
            Assert.AreEqual("RandomDieRoller", e.RollerType);
            Assert.AreEqual("8", e.DieSides);
            AssertHelpers.IsWithinRangeInclusive(1, 8, e.Result);
        }
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
        int i;
        for (i = 0; i < 2; i++)
        {
            DieTrackingData e = d[i];
            Assert.AreEqual("ConstantDieRoller", e.RollerType);
            Assert.AreEqual("6", e.DieSides);
            AssertHelpers.IsWithinRangeInclusive(1, 6, e.Result);
        }

        for (int x = i; x < 6; x++)
        {
            DieTrackingData e = d[x];
            Assert.AreEqual("RandomDieRoller", e.RollerType);
            Assert.AreEqual("6", e.DieSides);
            AssertHelpers.IsWithinRangeInclusive(1, 6, e.Result);
        }
    }

    [TestMethod]
    public void DieRollTracker_AddDieRollErrorsTest1()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(1, 1, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void DieRollTracker_AddDieRollErrorsTest2()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(0, 5, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void DieRollTracker_AddDieRollErrorsTest3()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(-4, 5, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void DieRollTracker_AddDieRollErrorsTest4()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(6, 8, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void DieRollTracker_AddDieRollErrorsTest5()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(6, -2, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void DieRollTracker_AddDieRollErrorsTest6()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(6, 4, null));
    }

    [TestMethod]
    public void DieRollTracker_AddDieRollErrorsTest7()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(6, 4, this.GetType()));
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
        IList<DieTrackingData> data = await _tracker.GetTrackingDataAsync("RandomDieRoller");

        // assert
        Assert.HasCount(19, data);
        foreach(DieTrackingData e in data)
        {
            Assert.AreEqual("RandomDieRoller", e.RollerType);
        }
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
        foreach (DieTrackingData e in data)
        {
            Assert.AreEqual("10", e.DieSides);
        }
    }

    [TestMethod]
    public async Task DieRollTracker_GetTrackingDataFilterDieTypeAndSidesTest()
    {
        // arrange
        _tracker.SetupTrackingSampleData();

        // act
        IList<DieTrackingData> data = await _tracker.GetTrackingDataAsync("RandomDieRoller", "20");

        // assert
        Assert.HasCount(14, data);
        foreach (DieTrackingData e in data)
        {
            Assert.AreEqual("RandomDieRoller", e.RollerType);
            Assert.AreEqual("20", e.DieSides);
        }
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
        Assert.HasCount(14, list);
        foreach (DieTrackingData e in list)
        {
            Assert.AreEqual("RandomDieRoller", e.RollerType);
            Assert.AreEqual("20", e.DieSides);
        }
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
