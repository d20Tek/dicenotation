using d20Tek.DiceNotation.DieRoller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

[TestClass]
public class AggregateRollTrackerTests
{
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
        var t = new AggregateRollTracker();

        // act
        t.AddDieRoll(6, 4, typeof(RandomDieRoller));
        var d = t.GetFrequencyDataView();

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
        var t = new AggregateRollTracker();

        // act
        t.AddDieRoll(6, 4, typeof(RandomDieRoller));
        t.AddDieRoll(6, 3, typeof(RandomDieRoller));
        t.AddDieRoll(6, 1, typeof(RandomDieRoller));
        t.AddDieRoll(6, 6, typeof(RandomDieRoller));
        t.AddDieRoll(6, 2, typeof(RandomDieRoller));
        t.AddDieRoll(6, 4, typeof(RandomDieRoller));

        var d = t.GetFrequencyDataView();

        // assert
        Assert.IsNotNull(d);
        Assert.HasCount(5, d);
        var e = t.GetFrequencyDataView().First(p => p.RollerType == "RandomDieRoller" && p.DieSides == "6" && p.Result == 4);
        Assert.AreEqual(2, e.Count);
        e = t.GetFrequencyDataView().First(p => p.RollerType == "RandomDieRoller" && p.DieSides == "6" && p.Result == 1);
        Assert.AreEqual(1, e.Count);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void AddMultipleDieRollDieSidesTest()
    {
        // arrange
        var t = new AggregateRollTracker();

        // act
        t.AddDieRoll(6, 4, typeof(RandomDieRoller));
        t.AddDieRoll(6, 3, typeof(RandomDieRoller));
        t.AddDieRoll(6, 1, typeof(RandomDieRoller));
        t.AddDieRoll(6, 6, typeof(RandomDieRoller));
        t.AddDieRoll(8, 2, typeof(RandomDieRoller));
        t.AddDieRoll(8, 4, typeof(RandomDieRoller));

        var d = t.GetFrequencyDataView();

        // assert
        Assert.IsNotNull(d);
        Assert.HasCount(6, d);
        var e = t.GetFrequencyDataView().Where(p => p.RollerType == "RandomDieRoller" && p.DieSides == "6");
        Assert.AreEqual(4, e.Count());
        e = t.GetFrequencyDataView().Where(p => p.RollerType == "RandomDieRoller" && p.DieSides == "8");
        Assert.AreEqual(2, e.Count());
    }

    [TestMethod]
    public void AddMultipleDieRollDieTypesTest()
    {
        // arrange
        var t = new AggregateRollTracker();

        // act
        t.AddDieRoll(6, 4, typeof(RandomDieRoller));
        t.AddDieRoll(6, 3, typeof(RandomDieRoller));
        t.AddDieRoll(6, 4, typeof(RandomDieRoller));
        t.AddDieRoll(6, 6, typeof(RandomDieRoller));
        t.AddDieRoll(6, 6, typeof(RandomDieRoller));
        t.AddDieRoll(6, 2, typeof(ConstantDieRoller));
        t.AddDieRoll(6, 4, typeof(ConstantDieRoller));

        var d = t.GetFrequencyDataView();

        // assert
        Assert.IsNotNull(d);
        Assert.HasCount(5, d);
        var e = t.GetFrequencyDataView().Where(p => p.RollerType == "RandomDieRoller" && p.DieSides == "6");
        Assert.AreEqual(3, e.Count());
        e = t.GetFrequencyDataView().Where(p => p.RollerType == "ConstantDieRoller" && p.DieSides == "6");
        Assert.AreEqual(2, e.Count());
    }

    [TestMethod]
    public void AddDieRollErrorsTest1()
    {
        // arrange
        var t = new AggregateRollTracker();

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => t.AddDieRoll(1, 1, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void AddDieRollErrorsTest2()
    {
        // arrange
        var t = new AggregateRollTracker();

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => t.AddDieRoll(0, 5, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void AddDieRollErrorsTest3()
    {
        // arrange
        var t = new AggregateRollTracker();

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => t.AddDieRoll(-4, 5, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void AddDieRollErrorsTest4()
    {
        // arrange
        var t = new AggregateRollTracker();

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => t.AddDieRoll(6, 8, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void AddDieRollErrorsTest5()
    {
        // arrange
        var t = new AggregateRollTracker();

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => t.AddDieRoll(6, -2, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void AddDieRollErrorsTest6()
    {
        // arrange
        var t = new AggregateRollTracker();

        // act - assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => t.AddDieRoll(6, 4, null));
    }

    [TestMethod]
    public void AddDieRollErrorsTest7()
    {
        // arrange
        var t = new AggregateRollTracker();

        // act - assert
        Assert.ThrowsExactly<ArgumentException>(
            [ExcludeFromCodeCoverage] () => t.AddDieRoll(6, 4, this.GetType()));
    }

    [TestMethod]
    public void ClearTest()
    {
        // arrange
        var t = new AggregateRollTracker();
        t.AddDieRoll(6, 4, typeof(RandomDieRoller));
        t.AddDieRoll(6, 3, typeof(RandomDieRoller));
        t.AddDieRoll(6, 1, typeof(RandomDieRoller));
        t.AddDieRoll(6, 6, typeof(RandomDieRoller));
        t.AddDieRoll(6, 2, typeof(RandomDieRoller));
        t.AddDieRoll(6, 4, typeof(RandomDieRoller));
        _ = t.GetFrequencyDataView();

        // act
        t.Clear();

        // assert
        var r = t.GetFrequencyDataView();
        Assert.IsEmpty(r);
    }

    [TestMethod]
    public void ToJsonTest()
    {
        // arrange
        var t = new AggregateRollTracker();
        t.SetupTrackingSampleData();

        // act
        var data = t.ToJson();

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
        var t = new AggregateRollTracker();
        t.SetupTrackingSampleData();
        var data = t.ToJson();
        Assert.IsFalse(string.IsNullOrEmpty(data));

        // act
        var other = new AggregateRollTracker();
        other.LoadFromJson(data);

        // assert
        var list = other.GetFrequencyDataView();
        Assert.HasCount(22, list);
    }

    [TestMethod]
    public void DieRollTracker_GetFrequencyDataTest()
    {
        // arrange
        var t = new AggregateRollTracker();
        var aggExpected = t.SetupStatisticalTrackingData();

        // act
        var data = t.GetFrequencyDataView();

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
