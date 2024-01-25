using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;
using D20Tek.DiceNotation.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace d20Tek.DiceNotation.UnitTests
{
    /// <summary>
    /// Summary description for DiceTests
    /// </summary>
    [TestClass]
    public class DiceTests
    {
        private readonly IDieRoller roller = new RandomDieRoller();

        public DiceTests()
        {
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
        public void Dice_ConstructorTest()
        {
            // setup test

            // run test
            IDice dice = new Dice();

            // validate results
            Assert.IsNotNull(dice);
            Assert.IsInstanceOfType(dice, typeof(IDice));
            Assert.IsInstanceOfType(dice, typeof(Dice));
            Assert.IsTrue(string.IsNullOrEmpty(dice.ToString()));
            Assert.IsTrue(dice.Configuration.HasBoundedResult);
            Assert.AreEqual(1, dice.Configuration.BoundedResultMinimum);
        }

        [TestMethod]
        public void Dice_ConstantTest()
        {
            // setup test
            IDice dice = new Dice();

            // run test
            IDice result = dice.Constant(5);

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IDice));
            Assert.IsInstanceOfType(result, typeof(Dice));
            Assert.AreEqual("5", dice.ToString());
        }

        [TestMethod]
        public void Dice_DiceSidesTest()
        {
            // setup test
            IDice dice = new Dice();

            // run test
            IDice result = dice.Dice(8);

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IDice));
            Assert.IsInstanceOfType(result, typeof(Dice));
            Assert.AreEqual("1d8", dice.ToString());
        }

        [TestMethod]
        public void Dice_DiceChainingTest()
        {
            // setup test
            IDice dice = new Dice();

            // run test
            IDice result = dice.Dice(6, 4, choose: 3).Dice(8).Constant(5);

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IDice));
            Assert.IsInstanceOfType(result, typeof(Dice));
            Assert.AreEqual("4d6k3+1d8+5", dice.ToString());
        }

        [TestMethod]
        public void Dice_DiceClearTest()
        {
            // setup test
            IDice dice = new Dice();
            dice = dice.Dice(6, 4, choose: 3).Dice(8).Constant(5);

            // run test
            dice.Clear();
            dice = dice.Dice(6, 1);

            // validate results
            Assert.IsNotNull(dice);
            Assert.IsInstanceOfType(dice, typeof(IDice));
            Assert.IsInstanceOfType(dice, typeof(Dice));
            Assert.AreEqual("1d6", dice.ToString());
        }

        [TestMethod]
        public void Dice_FudgeDiceNumberTest()
        {
            // setup test
            IDice dice = new Dice();

            // run test
            IDice result = dice.FudgeDice(3);

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IDice));
            Assert.IsInstanceOfType(result, typeof(Dice));
            Assert.AreEqual("3f", dice.ToString());
        }

        [TestMethod]
        public void Dice_RollConstantTest()
        {
            // setup test
            IDice dice = new Dice().Constant(3);

            // run test
            DiceResult result = dice.Roll(roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
            Assert.AreEqual(3, result.Value);
            Assert.AreEqual(1, result.Results.Count);
            foreach(TermResult r in result.Results)
            {
                Assert.AreEqual(3, r.Value);
            }
            Assert.AreEqual("3", result.DiceExpression);
        }

        [TestMethod]
        public void Dice_RollSingleDieTest()
        {
            // setup test
            IDice dice = new Dice();
            dice.Dice(20);

            // run test
            DiceResult result = dice.Roll(roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
            AssertHelpers.IsWithinRangeInclusive(1, 20, result.Value);
            Assert.AreEqual(1, result.Results.Count);
            int sum = 0;
            foreach (TermResult r in result.Results)
            {
                AssertHelpers.IsWithinRangeInclusive(1, 20, r.Value);
                sum += r.Value;
            }
            Assert.AreEqual(sum, result.Value);
            Assert.AreEqual("1d20", result.DiceExpression);
        }

        [TestMethod]
        public void Dice_RollMultipleDiceTest()
        {
            // setup test
            IDice dice = new Dice();
            dice.Dice(6, 3);

            // run test
            DiceResult result = dice.Roll(roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
            AssertHelpers.IsWithinRangeInclusive(3, 18, result.Value);
            Assert.AreEqual(3, result.Results.Count);
            int sum = 0;
            foreach (TermResult r in result.Results)
            {
                AssertHelpers.IsWithinRangeInclusive(1, 6, r.Value);
                sum += r.Value;
            }
            Assert.AreEqual(sum, result.Value);
            Assert.AreEqual("3d6", result.DiceExpression);
        }

        [TestMethod]
        public void Dice_RollScalarMultiplierDiceTest()
        {
            // setup test
            IDice dice = new Dice();
            dice.Dice(8, 2, 10);

            // run test
            DiceResult result = dice.Roll(roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
            AssertHelpers.IsWithinRangeInclusive(20, 160, result.Value);
            Assert.AreEqual(2, result.Results.Count);
            int sum = 0;
            foreach (TermResult r in result.Results)
            {
                AssertHelpers.IsWithinRangeInclusive(1, 8, r.Value);
                sum += (int)(r.Value * r.Scalar);
            }
            Assert.AreEqual(sum, result.Value);
            Assert.AreEqual("2d8x10", result.DiceExpression);
        }

        [TestMethod]
        public void Dice_RollChooseDiceTest()
        {
            // setup test
            IDice dice = new Dice();
            dice.Dice(6, 4, choose: 3);

            // run test
            DiceResult result = dice.Roll(roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
            AssertHelpers.IsWithinRangeInclusive(3, 18, result.Value);
            AssertHelpers.AssertDiceChoose(result, "4d6k3", "DiceTerm.d6", 4, 3);
        }

        [TestMethod]
        [ExcludeFromCodeCoverage]
        public void Dice_RollExplodingDiceTest()
        {
            // setup test
            IDice dice = new Dice();
            dice.Dice(6, 4, exploding: 6);

            // run test
            DiceResult result = dice.Roll(roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
            int sum = 0, count = 4;
            foreach (TermResult r in result.Results)
            {
                Assert.IsNotNull(r);
                Assert.AreEqual(1, r.Scalar);
                AssertHelpers.IsWithinRangeInclusive(1, 6, r.Value);
                Assert.AreEqual("DiceTerm.d6", r.Type);
                sum += r.Value;
                if (r.Value >= 6) count++;
            }
            Assert.AreEqual(count, result.Results.Count);
            Assert.AreEqual(sum, result.Value);
            Assert.AreEqual("4d6!6", result.DiceExpression);
        }

        [TestMethod]
        public void Dice_RollChainedDiceTest()
        {
            // setup test
            IDice dice = new Dice();
            dice.Dice(6, 4, choose: 3).Dice(8).Constant(5);

            // run test
            DiceResult result = dice.Roll(new ConstantDieRoller(1));

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.ConstantDieRoller", result.DieRollerUsed);
            Assert.AreEqual(9, result.Value);
            AssertHelpers.AssertDiceChoose(result, "4d6k3+1d8+5", "", 6, 5);
        }

        [TestMethod]
        public void Dice_RollFudgeSingleDieTest()
        {
            // setup test
            IDice dice = new Dice();
            dice.FudgeDice(1);

            // run test
            DiceResult result = dice.Roll(this.roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
            AssertHelpers.IsWithinRangeInclusive(-1, 1, result.Value);
            Assert.AreEqual(1, result.Results.Count);
            int sum = 0;
            foreach (TermResult r in result.Results)
            {
                AssertHelpers.IsWithinRangeInclusive(-1, 1, r.Value);
                sum += r.Value;
            }
            Assert.AreEqual(sum, result.Value);
            Assert.AreEqual("1f", result.DiceExpression);
        }

        [TestMethod]
        public void Dice_RollMultipleFudgeDiceTest()
        {
            // setup test
            IDice dice = new Dice();
            dice.FudgeDice(6);

            // run test
            DiceResult result = dice.Roll(this.roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
            AssertHelpers.IsWithinRangeInclusive(-6, 6, result.Value);
            Assert.AreEqual(6, result.Results.Count);
            int sum = 0;
            foreach (TermResult r in result.Results)
            {
                AssertHelpers.IsWithinRangeInclusive(-1, 1, r.Value);
                sum += r.Value;
            }
            Assert.AreEqual(sum, result.Value);
            Assert.AreEqual("6f", result.DiceExpression);
        }

        [TestMethod]
        public void Dice_RollFudgeChooseDiceTest()
        {
            // setup test
            IDice dice = new Dice();
            dice.FudgeDice(6, 3);

            // run test
            DiceResult result = dice.Roll(this.roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
            AssertHelpers.IsWithinRangeInclusive(-3, 3, result.Value);
            AssertHelpers.AssertDiceChoose(result, "6fk3", "FudgeDiceTerm.dF", 6, 3);
        }

        [TestMethod]
        public void Dice_RollNullRollerTest()
        {
            // setup test
            IDice dice = new Dice();
            dice.Dice(20);

            // run test
            var actual = dice.Roll(null);

            // validate results
            Assert.That.InRange(actual.Value, 1, 20);
        }

        [TestMethod]
        public void Dice_RollStringNullRollerTest()
        {
            // setup test
            IDice dice = new Dice();

            // run test
            var actual = dice.Roll("1d20");

            // validate results
            Assert.That.InRange(actual.Value, 1, 20);
        }

        [TestMethod]
        public void Dice_ParseMultipleDiceTest()
        {
            // setup test
            IDice dice = new Dice();

            // run test
            DiceResult result = dice.Roll("3d6+2", roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
            AssertHelpers.IsWithinRangeInclusive(5, 20, result.Value);
            int sum = 0;
            foreach (TermResult r in result.Results)
            {
                AssertHelpers.IsWithinRangeInclusive(1, 6, r.Value);
                sum += r.Value;
            }
            sum += 2;
            Assert.AreEqual(sum, result.Value);
            Assert.AreEqual("3d6+2", result.DiceExpression);
        }

        [TestMethod]
        public void Dice_ParseChainedDiceTest()
        {
            // setup test
            IDice dice = new Dice();
            
            // run test
            DiceResult result = dice.Roll("4d6k3 + 1d8 + 5", new ConstantDieRoller(1));

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.ConstantDieRoller", result.DieRollerUsed);
            Assert.AreEqual(9, result.Value);
            AssertHelpers.AssertDiceChoose(result, "4d6k3+1d8+5", "DiceTerm", 5, 4, 5);
        }

        [TestMethod]
        public void Dice_RollWithNegativeResultUnboundedTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            IDice dice = new Dice();
            dice.Configuration.HasBoundedResult = false;
            DiceResult result = parser.Parse("d12-3", dice.Configuration, new ConstantDieRoller(1));

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d12-3", result.DiceExpression);
            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual(-2, result.Value);
        }

        [TestMethod]
        public void Dice_DiceConcat()
        {
            // setup test
            IDice dice1 = new Dice();
            IDice dice2 = new Dice();

            // run test
            dice1.Dice(6, 4, choose: 3);
            dice2.Dice(8).Constant(5);
            IDice result = dice1.Concat(dice2);

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IDice));
            Assert.IsInstanceOfType(result, typeof(Dice));
            Assert.AreEqual("4d6k3+1d8+5", result.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [ExcludeFromCodeCoverage]
        public void Dice_DiceConcat_WithNullOther()
        {
            // setup test
            IDice dice1 = new Dice();
            dice1.Dice(6, 4, choose: 3);

            // run test
            dice1.Concat(null);

            // validate results
        }

        [TestMethod]
        public void Dice_DiceWith1Side()
        {
            // setup test
            IDice dice = new Dice();

            // run test
            IDice result = dice.Dice(1);
            DiceResult dr = result.Roll();

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IDice));
            Assert.IsInstanceOfType(result, typeof(Dice));
            Assert.AreEqual("1d1", dice.ToString());
            Assert.AreEqual(1, dr.Value);
        }

        [TestMethod]
        public void Dice_DiceStringWith1Side()
        {
            // setup test
            IDice dice = new Dice();

            // run test
            DiceResult dr = dice.Roll("1d1");

            // validate results
            Assert.AreEqual("1d1", dr.DiceExpression);
            Assert.AreEqual(1, dr.Value);
        }

        [TestMethod]
        public void Dice_RollDiceRequest_SingleDieTest()
        {
            // setup test
            var request = new DiceRequest(1, 20);
            IDice dice = new Dice();

            // run test
            DiceResult result = dice.Roll(request);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
            AssertHelpers.IsWithinRangeInclusive(1, 20, result.Value);
            Assert.AreEqual(1, result.Results.Count);
            int sum = 0;
            foreach (TermResult r in result.Results)
            {
                AssertHelpers.IsWithinRangeInclusive(1, 20, r.Value);
                sum += r.Value;
            }
            Assert.AreEqual(sum, result.Value);
            Assert.AreEqual("1d20", result.DiceExpression);
        }

        [TestMethod]
        public void Dice_RollDiceRequest_ChooseDiceTest()
        {
            // setup test
            IDice dice = new Dice();
            var request = new DiceRequest(4, 6, choose: 3);

            // run test
            DiceResult result = dice.Roll(request, roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
            AssertHelpers.IsWithinRangeInclusive(3, 18, result.Value);
            AssertHelpers.AssertDiceChoose(result, "4d6k3", "DiceTerm.d6", 4, 3);
        }

        [TestMethod]
        [ExcludeFromCodeCoverage]
        public void Dice_RollDiceRequest_ExplodingDiceTest()
        {
            // setup test
            IDice dice = new Dice();
            var request = new DiceRequest(4, 6, exploding: 6);

            // run test
            DiceResult result = dice.Roll(request, roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("d20Tek.DiceNotation.DieRoller.RandomDieRoller", result.DieRollerUsed);
            int sum = 0, count = 4;
            foreach (TermResult r in result.Results)
            {
                Assert.IsNotNull(r);
                Assert.AreEqual(1, r.Scalar);
                AssertHelpers.IsWithinRangeInclusive(1, 6, r.Value);
                Assert.AreEqual("DiceTerm.d6", r.Type);
                sum += r.Value;
                if (r.Value >= 6) count++;
            }
            Assert.AreEqual(count, result.Results.Count);
            Assert.AreEqual(sum, result.Value);
            Assert.AreEqual("4d6!6", result.DiceExpression);
        }
    }
}
