using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

[TestClass]
public class AggregateRollTrackerTests
{
    private readonly AggregateRollTracker _tracker = new();

    [TestMethod]
    public void ConstructorTest()
    {
        // arrange

        // act
        var t = new AggregateRollTracker();

        // assert
        Assert.IsNotNull(t);
        Assert.IsInstanceOfType<IAggregateRollTracker>(t);
        Assert.IsInstanceOfType<AggregateRollTracker>(t);
        Assert.IsEmpty(t.GetFrequencyDataView());
    }

    [TestMethod]
    public void AddDieRollTest()
    {
        // arrange

        // act
        _tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));
        var d = _tracker.GetFrequencyDataView();

        // assert
        Assert.IsNotNull(d);
        Assert.HasCount(1, d);
        Assert.AreEqual("RandomDieRoller", d[0].RollerType);
        Assert.AreEqual("6", d[0].DieSides);
        Assert.AreEqual(4, d[0].Result);
        Assert.AreEqual(1, d[0].Count);
        Assert.AreEqual(100f, d[0].Percentage);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void AddMultipleDieRollTest()
    {
        // arrange

        // act
        _tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 3, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 1, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 6, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 2, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));

        var d = _tracker.GetFrequencyDataView();

        // assert
        Assert.IsNotNull(d);
        Assert.HasCount(5, d);
        var e = _tracker.GetFrequencyDataView().First(
            p => p.RollerType == "RandomDieRoller" && p.DieSides == "6" && p.Result == 4);
        Assert.AreEqual(2, e.Count);
        e = _tracker.GetFrequencyDataView().First(
            p => p.RollerType == "RandomDieRoller" && p.DieSides == "6" && p.Result == 1);
        Assert.AreEqual(1, e.Count);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void AddMultipleDieRollDieSidesTest()
    {
        // arrange

        // act
        _tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 3, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 1, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 6, typeof(RandomDieRoller));
        _tracker.AddDieRoll(8, 2, typeof(RandomDieRoller));
        _tracker.AddDieRoll(8, 4, typeof(RandomDieRoller));

        var d = _tracker.GetFrequencyDataView();

        // assert
        Assert.IsNotNull(d);
        Assert.HasCount(6, d);
        var e = _tracker.GetFrequencyDataView().Where(p => p.RollerType == "RandomDieRoller" && p.DieSides == "6");
        Assert.AreEqual(4, e.Count());
        e = _tracker.GetFrequencyDataView().Where(p => p.RollerType == "RandomDieRoller" && p.DieSides == "8");
        Assert.AreEqual(2, e.Count());
    }

    [TestMethod]
    public void AddMultipleDieRollDieTypesTest()
    {
        // arrange

        // act
        _tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 3, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 6, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 6, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 2, typeof(ConstantDieRoller));
        _tracker.AddDieRoll(6, 4, typeof(ConstantDieRoller));

        var d = _tracker.GetFrequencyDataView();

        // assert
        Assert.IsNotNull(d);
        Assert.HasCount(5, d);
        var e = _tracker.GetFrequencyDataView().Where(p => p.RollerType == "RandomDieRoller" && p.DieSides == "6");
        Assert.AreEqual(3, e.Count());
        e = _tracker.GetFrequencyDataView().Where(p => p.RollerType == "ConstantDieRoller" && p.DieSides == "6");
        Assert.AreEqual(2, e.Count());
    }

    [TestMethod]
    public void ClearTest()
    {
        // arrange
        _tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 3, typeof(RandomDieRoller));
        _tracker.AddDieRoll(6, 1, typeof(RandomDieRoller));
        _ = _tracker.GetFrequencyDataView();

        // act
        _tracker.Clear();

        // assert
        Assert.IsEmpty(_tracker.GetFrequencyDataView());
    }

    [TestMethod]
    public void DieRollTracker_GetFrequencyDataTest()
    {
        // arrange
        var aggExpected = _tracker.SetupStatisticalTrackingData();

        // act
        var data = _tracker.GetFrequencyDataView();

        // assert
        Assert.IsNotNull(data);
        Assert.IsInstanceOfType<IList<AggregateDieTrackingData>>(data);
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
}
