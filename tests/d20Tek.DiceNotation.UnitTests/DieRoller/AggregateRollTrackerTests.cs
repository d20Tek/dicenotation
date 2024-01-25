using d20Tek.DiceNotation.DieRoller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace d20Tek.DiceNotation.UnitTests.DieRoller
{
    [TestClass]
    public class AggregateRollTrackerTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            // setup test

            // run test
            IAggregateRollTracker t = new AggregateRollTracker();

            // validate results
            Assert.IsNotNull(t);
            Assert.IsInstanceOfType(t, typeof(IAggregateRollTracker));
            Assert.IsInstanceOfType(t, typeof(AggregateRollTracker));
            Assert.AreEqual(0, t.GetFrequencyDataView().Count);
        }

        [TestMethod]
        public void AddDieRollTest()
        {
            // setup test
            var t = new AggregateRollTracker();

            // run test
            t.AddDieRoll(6, 4, typeof(RandomDieRoller));
            var d = t.GetFrequencyDataView();

            // validate results
            Assert.IsNotNull(d);
            Assert.AreEqual(1, d.Count);
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
            // setup test
            var t = new AggregateRollTracker();

            // run test
            t.AddDieRoll(6, 4, typeof(RandomDieRoller));
            t.AddDieRoll(6, 3, typeof(RandomDieRoller));
            t.AddDieRoll(6, 1, typeof(RandomDieRoller));
            t.AddDieRoll(6, 6, typeof(RandomDieRoller));
            t.AddDieRoll(6, 2, typeof(RandomDieRoller));
            t.AddDieRoll(6, 4, typeof(RandomDieRoller));

            var d = t.GetFrequencyDataView();

            // validate results
            Assert.IsNotNull(d);
            Assert.AreEqual(5, d.Count);
            var e = t.GetFrequencyDataView().First(p => p.RollerType == "RandomDieRoller" && p.DieSides == "6" && p.Result == 4);
            Assert.AreEqual(2, e.Count);
            e = t.GetFrequencyDataView().First(p => p.RollerType == "RandomDieRoller" && p.DieSides == "6" && p.Result == 1);
            Assert.AreEqual(1, e.Count);
        }

        [TestMethod]
        [ExcludeFromCodeCoverage]
        public void AddMultipleDieRollDieSidesTest()
        {
            // setup test
            var t = new AggregateRollTracker();

            // run test
            t.AddDieRoll(6, 4, typeof(RandomDieRoller));
            t.AddDieRoll(6, 3, typeof(RandomDieRoller));
            t.AddDieRoll(6, 1, typeof(RandomDieRoller));
            t.AddDieRoll(6, 6, typeof(RandomDieRoller));
            t.AddDieRoll(8, 2, typeof(RandomDieRoller));
            t.AddDieRoll(8, 4, typeof(RandomDieRoller));

            var d = t.GetFrequencyDataView();

            // validate results
            Assert.IsNotNull(d);
            Assert.AreEqual(6, d.Count);
            var e = t.GetFrequencyDataView().Where(p => p.RollerType == "RandomDieRoller" && p.DieSides == "6");
            Assert.AreEqual(4, e.Count());
            e = t.GetFrequencyDataView().Where(p => p.RollerType == "RandomDieRoller" && p.DieSides == "8");
            Assert.AreEqual(2, e.Count());
        }

        [TestMethod]
        public void AddMultipleDieRollDieTypesTest()
        {
            // setup test
            var t = new AggregateRollTracker();

            // run test
            t.AddDieRoll(6, 4, typeof(RandomDieRoller));
            t.AddDieRoll(6, 3, typeof(RandomDieRoller));
            t.AddDieRoll(6, 4, typeof(RandomDieRoller));
            t.AddDieRoll(6, 6, typeof(RandomDieRoller));
            t.AddDieRoll(6, 6, typeof(RandomDieRoller));
            t.AddDieRoll(6, 2, typeof(ConstantDieRoller));
            t.AddDieRoll(6, 4, typeof(ConstantDieRoller));

            var d = t.GetFrequencyDataView();

            // validate results
            Assert.IsNotNull(d);
            Assert.AreEqual(5, d.Count);
            var e = t.GetFrequencyDataView().Where(p => p.RollerType == "RandomDieRoller" && p.DieSides == "6");
            Assert.AreEqual(3, e.Count());
            e = t.GetFrequencyDataView().Where(p => p.RollerType == "ConstantDieRoller" && p.DieSides == "6");
            Assert.AreEqual(2, e.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [ExcludeFromCodeCoverage]
        public void AddDieRollErrorsTest1()
        {
            // setup test
            var t = new AggregateRollTracker();

            // run test
            t.AddDieRoll(1, 1, typeof(RandomDieRoller));

            // validate results
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [ExcludeFromCodeCoverage]
        public void AddDieRollErrorsTest2()
        {
            // setup test
            var t = new AggregateRollTracker();

            // run test
            t.AddDieRoll(0, 5, typeof(RandomDieRoller));

            // validate results
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [ExcludeFromCodeCoverage]
        public void AddDieRollErrorsTest3()
        {
            // setup test
            var t = new AggregateRollTracker();

            // run test
            t.AddDieRoll(-4, 5, typeof(RandomDieRoller));

            // validate results
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [ExcludeFromCodeCoverage]
        public void AddDieRollErrorsTest4()
        {
            // setup test
            var t = new AggregateRollTracker();

            // run test
            t.AddDieRoll(6, 8, typeof(RandomDieRoller));

            // validate results
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [ExcludeFromCodeCoverage]
        public void AddDieRollErrorsTest5()
        {
            // setup test
            var t = new AggregateRollTracker();

            // run test
            t.AddDieRoll(6, -2, typeof(RandomDieRoller));

            // validate results
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [ExcludeFromCodeCoverage]
        public void AddDieRollErrorsTest6()
        {
            // setup test
            var t = new AggregateRollTracker();

            // run test
            t.AddDieRoll(6, 4, null);

            // validate results
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [ExcludeFromCodeCoverage]
        public void AddDieRollErrorsTest7()
        {
            // setup test
            var t = new AggregateRollTracker();

            // run test
            t.AddDieRoll(6, 4, this.GetType());

            // validate results
        }

        [TestMethod]
        public void ClearTest()
        {
            // setup test
            var t = new AggregateRollTracker();
            t.AddDieRoll(6, 4, typeof(RandomDieRoller));
            t.AddDieRoll(6, 3, typeof(RandomDieRoller));
            t.AddDieRoll(6, 1, typeof(RandomDieRoller));
            t.AddDieRoll(6, 6, typeof(RandomDieRoller));
            t.AddDieRoll(6, 2, typeof(RandomDieRoller));
            t.AddDieRoll(6, 4, typeof(RandomDieRoller));

            var d = t.GetFrequencyDataView();

            // run test
            t.Clear();

            // validate results
            var r = t.GetFrequencyDataView();
            Assert.AreEqual(0, r.Count);
        }

        [TestMethod]
        public void ToJsonTest()
        {
            // setup test
            var t = new AggregateRollTracker();
            this.SetupTrackingSampleData(t);

            // run test
            var data = t.ToJson();

            // validate results
            Assert.IsFalse(string.IsNullOrEmpty(data));
            StringAssert.Contains(data, "17");
            StringAssert.Contains(data, "ConstantDieRoller");
            StringAssert.Contains(data, "12");
        }

        [TestMethod]
        public void LoadFromJsonTest_NoData()
        {
            // setup test
            string data = "";

            // run test
            var other = new AggregateRollTracker();
            other.LoadFromJson(data);

            // validate results
            var list = other.GetFrequencyDataView();
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void LoadFromJsonTest()
        {
            // setup test
            var t = new AggregateRollTracker();
            this.SetupTrackingSampleData(t);
            var data = t.ToJson();
            Assert.IsFalse(string.IsNullOrEmpty(data));

            // run test
            var other = new AggregateRollTracker();
            other.LoadFromJson(data);

            // validate results
            var list = other.GetFrequencyDataView();
            Assert.AreEqual(22, list.Count);
        }

        [TestMethod]
        public void DieRollTracker_GetFrequencyDataTest()
        {
            // setup test
            var t = new AggregateRollTracker();
            List<AggregateDieTrackingData> aggExpected = this.SetupStatisticalTrackingData(t);

            // run test
            var data = t.GetFrequencyDataView();

            // validate results
            Assert.IsNotNull(data);
            Assert.IsInstanceOfType(data, typeof(IList<AggregateDieTrackingData>));
            Assert.AreEqual(23, data.Count);

            Assert.AreEqual(aggExpected.Count, data.Count);
            for (int i = 0; i < data.Count; i++)
            {
                Assert.AreEqual(aggExpected[i].RollerType, data[i].RollerType, "Failed roller type for item: " + i.ToString());
                Assert.AreEqual(aggExpected[i].DieSides, data[i].DieSides, "Failed die sides for item: " + i.ToString());
                Assert.AreEqual(aggExpected[i].Result, data[i].Result, "Failed result for item: " + i.ToString());
                Assert.AreEqual(aggExpected[i].Count, data[i].Count, "Failed count for item: " + i.ToString());
                Assert.AreEqual(aggExpected[i].Percentage, data[i].Percentage, "Failed percentage for item: " + i.ToString());
            }
        }

        private void SetupTrackingSampleData(IAllowRollTrackerEntry tracker)
        {
            tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));
            tracker.AddDieRoll(6, 3, typeof(RandomDieRoller));
            tracker.AddDieRoll(6, 1, typeof(RandomDieRoller));
            tracker.AddDieRoll(6, 6, typeof(RandomDieRoller));
            tracker.AddDieRoll(8, 2, typeof(ConstantDieRoller));
            tracker.AddDieRoll(8, 4, typeof(ConstantDieRoller));
            tracker.AddDieRoll(10, 2, typeof(ConstantDieRoller));
            tracker.AddDieRoll(10, 8, typeof(ConstantDieRoller));
            tracker.AddDieRoll(10, 9, typeof(ConstantDieRoller));
            tracker.AddDieRoll(10, 4, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 5, typeof(ConstantDieRoller));
            tracker.AddDieRoll(20, 18, typeof(ConstantDieRoller));
            tracker.AddDieRoll(20, 11, typeof(ConstantDieRoller));
            tracker.AddDieRoll(20, 5, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 5, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 17, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 9, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 12, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 20, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 8, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 11, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 9, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 12, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 20, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 14, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 13, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 9, typeof(RandomDieRoller));
        }

        private List<AggregateDieTrackingData> SetupStatisticalTrackingData(IAllowRollTrackerEntry tracker)
        {
            this.SetupTrackingSampleData(tracker);

            tracker.AddDieRoll(20, 5, typeof(ConstantDieRoller));
            tracker.AddDieRoll(20, 18, typeof(ConstantDieRoller));
            tracker.AddDieRoll(20, 11, typeof(ConstantDieRoller));
            tracker.AddDieRoll(20, 5, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 5, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 17, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 9, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 12, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 20, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 8, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 11, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 9, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 10, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 12, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 20, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 14, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 13, typeof(RandomDieRoller));
            tracker.AddDieRoll(20, 9, typeof(RandomDieRoller));

            List<AggregateDieTrackingData> expectedAggegate = new List<AggregateDieTrackingData>
            {
                new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "10", Result = 2, Count = 1, Percentage = 33.3f },
                new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "10", Result = 8, Count = 1, Percentage = 33.3f },
                new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "10", Result = 9, Count = 1, Percentage = 33.3f },
                new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "20", Result = 5, Count = 2, Percentage = 33.3f },
                new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "20", Result = 11, Count = 2, Percentage = 33.3f },
                new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "20", Result = 18, Count = 2, Percentage = 33.3f },
                new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "8", Result = 2, Count = 1, Percentage = 50.0f },
                new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "8", Result = 4, Count = 1, Percentage = 50.0f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "10", Result = 4, Count = 1, Percentage = 100.0f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 5, Count = 4, Percentage = 13.8f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 8, Count = 2, Percentage = 6.9f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 9, Count = 6, Percentage = 20.7f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 10, Count = 1, Percentage = 3.4f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 11, Count = 2, Percentage = 6.9f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 12, Count = 4, Percentage = 13.8f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 13, Count = 2, Percentage = 6.9f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 14, Count = 2, Percentage = 6.9f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 17, Count = 2, Percentage = 6.9f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 20, Count = 4, Percentage = 13.8f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "6", Result = 1, Count = 1, Percentage = 25.0f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "6", Result = 3, Count = 1, Percentage = 25.0f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "6", Result = 4, Count = 1, Percentage = 25.0f },
                new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "6", Result = 6, Count = 1, Percentage = 25.0f }
            };

            return expectedAggegate;

            /* Expected aggregate data view
                ConstantDieRoller	10	2	1
                ConstantDieRoller	10	8	1
                ConstantDieRoller	10	9	1
                ConstantDieRoller	20	5	2
                ConstantDieRoller	20	11	2
                ConstantDieRoller	20	18	2
                ConstantDieRoller	8	2	1
                ConstantDieRoller	8	4	1
                RandomDieRoller	    10	4	1
                RandomDieRoller	    20	5	4
                RandomDieRoller	    20	8	2
                RandomDieRoller	    20	9	6
                RandomDieRoller	    20	10	1
                RandomDieRoller	    20	11	2
                RandomDieRoller	    20	12	4
                RandomDieRoller	    20	13	2
                RandomDieRoller	    20	14	2
                RandomDieRoller	    20	17	2
                RandomDieRoller	    20	20	4
                RandomDieRoller	    6	1	1
                RandomDieRoller	    6	3	1
                RandomDieRoller	    6	4	1
                RandomDieRoller	    6	6	1
            */
        }
    }
}
