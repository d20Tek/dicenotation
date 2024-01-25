using d20Tek.DiceNotation.DieRoller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace d20Tek.DiceNotation.UnitTests.DieRoller
{
    [TestClass]
    public class AggregatedTrackerWithMathNetRollerTests
    {
        private readonly IAggregateRollTracker tracker = new AggregateRollTracker();
        private readonly IDieRoller roller;

        public AggregatedTrackerWithMathNetRollerTests()
        {
            this.roller = new MathNetDieRoller(this.tracker);
        }

        [TestMethod]
        public void SingleDieSidesTest()
        {
            // setup test
            int[] rolls = new int[5];

            rolls[0] = this.roller.Roll(12);
            rolls[1] = this.roller.Roll(12);
            rolls[2] = this.roller.Roll(12);
            rolls[3] = this.roller.Roll(12);
            rolls[4] = this.roller.Roll(12);

            int expected = rolls.Distinct().Count();

            // run test
            var data = this.tracker.GetFrequencyDataView();

            // validate results
            Assert.AreEqual(expected, data.Count);
            foreach (AggregateDieTrackingData e in data)
            {
                Assert.AreEqual("MathNetDieRoller", e.RollerType);
                Assert.AreEqual("12", e.DieSides);
            }
        }

        [TestMethod]
        public void MultipleDieSidesTest()
        {
            // setup test
            var rolls12 = new int[4];
            rolls12[0] = this.roller.Roll(12);
            rolls12[1] = this.roller.Roll(12);
            rolls12[2] = this.roller.Roll(12);
            rolls12[3] = this.roller.Roll(12);
            var expected12 = rolls12.Distinct().Count();

            var rolls8 = new int[3];
            rolls8[0] = this.roller.Roll(8);
            rolls8[1] = this.roller.Roll(8);
            rolls8[2] = this.roller.Roll(8);
            var expected8 = rolls8.Distinct().Count();

            var rolls20 = new int[10];
            rolls20[0] = this.roller.Roll(20);
            rolls20[1] = this.roller.Roll(20);
            rolls20[2] = this.roller.Roll(20);
            rolls20[3] = this.roller.Roll(20);
            rolls20[4] = this.roller.Roll(20);
            rolls20[5] = this.roller.Roll(20);
            rolls20[6] = this.roller.Roll(20);
            rolls20[7] = this.roller.Roll(20);
            rolls20[8] = this.roller.Roll(20);
            rolls20[9] = this.roller.Roll(20);
            var expected20 = rolls20.Distinct().Count();

            // run test
            var data = this.tracker.GetFrequencyDataView();
            var list12s = data.Where(p => p.DieSides == "12");
            var list8s = data.Where(p => p.DieSides == "8");
            var list20s = data.Where(p => p.DieSides == "20");

            // validate results
            Assert.AreEqual(expected12, list12s.Count());
            Assert.AreEqual(expected8, list8s.Count());
            Assert.AreEqual(expected20, list20s.Count());
        }
    }
}
