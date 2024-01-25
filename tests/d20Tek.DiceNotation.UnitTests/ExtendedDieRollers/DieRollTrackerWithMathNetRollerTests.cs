using MathNet.Numerics.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using d20Tek.DiceNotation.DieRoller;
using D20Tek.DiceNotation.UnitTests.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace d20Tek.DiceNotation.UnitTests.DieRoller
{
    /// <summary>
    /// Summary description for DieRollTrackerWithMathNetRollerTests
    /// </summary>
    [TestClass]
    public class DieRollTrackerWithMathNetRollerTests
    {
        private readonly IDieRollTracker tracker = new DieRollTracker();
        private readonly IDieRoller roller;

        public DieRollTrackerWithMathNetRollerTests()
        {
            this.roller = new MathNetDieRoller(new MersenneTwister(), this.tracker);
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void DieRollTrackerWithMathNetRoller_SingleDieSidesTest()
        {
            // setup test
            this.roller.Roll(12);
            this.roller.Roll(12);
            this.roller.Roll(12);
            this.roller.Roll(12);
            this.roller.Roll(12);

            // run test
            Task<IList<DieTrackingData>> t = this.tracker.GetTrackingDataAsync();
            t.Wait();
            IList<DieTrackingData> data = t.Result;

            // validate results
            Assert.AreEqual(5, data.Count);
            foreach (DieTrackingData e in data)
            {
                Assert.AreEqual("MathNetDieRoller", e.RollerType);
                Assert.AreEqual("12", e.DieSides);
                AssertHelpers.IsWithinRangeInclusive(1, 12, e.Result);
            }
        }

        [TestMethod]
        public void DieRollTrackerWithMathNetRoller_MultipleDieSidesTest()
        {
            // setup test
            this.roller.Roll(12);
            this.roller.Roll(12);
            this.roller.Roll(12);
            this.roller.Roll(12);
            this.roller.Roll(8);
            this.roller.Roll(8);
            this.roller.Roll(8);
            this.roller.Roll(20);
            this.roller.Roll(20);
            this.roller.Roll(20);
            this.roller.Roll(20);
            this.roller.Roll(20);
            this.roller.Roll(20);
            this.roller.Roll(20);
            this.roller.Roll(20);
            this.roller.Roll(20);
            this.roller.Roll(20);

            // run test
            Task<IList<DieTrackingData>> t1 = this.tracker.GetTrackingDataAsync(dieSides: "12");
            t1.Wait();
            IList<DieTrackingData> data1 = t1.Result;

            Task<IList<DieTrackingData>> t2 = this.tracker.GetTrackingDataAsync(dieSides: "8");
            t2.Wait();
            IList<DieTrackingData> data2 = t2.Result;

            Task<IList<DieTrackingData>> t3 = this.tracker.GetTrackingDataAsync(dieSides: "20");
            t3.Wait();
            IList<DieTrackingData> data3 = t3.Result;

            // validate results
            Assert.AreEqual(17, data1.Count + data2.Count + data3.Count);
            Assert.AreEqual(4, data1.Count);
            foreach (DieTrackingData e in data1)
            {
                Assert.AreEqual("12", e.DieSides);
                AssertHelpers.IsWithinRangeInclusive(1, 12, e.Result);
            }
            Assert.AreEqual(3, data2.Count);
            foreach (DieTrackingData e in data2)
            {
                Assert.AreEqual("8", e.DieSides);
                AssertHelpers.IsWithinRangeInclusive(1, 8, e.Result);
            }
            Assert.AreEqual(10, data3.Count);
            foreach (DieTrackingData e in data3)
            {
                Assert.AreEqual("20", e.DieSides);
                AssertHelpers.IsWithinRangeInclusive(1, 20, e.Result);
            }
        }
    }
}
