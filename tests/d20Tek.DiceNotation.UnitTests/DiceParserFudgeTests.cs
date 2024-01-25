using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;
using D20Tek.DiceNotation.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace d20Tek.DiceNotation.UnitTests
{
    /// <summary>
    /// Summary description for DiceParserFudge
    /// </summary>
    [TestClass]
    public class DiceParserFudgeTests
    {
        private readonly DiceConfiguration config = new DiceConfiguration();
        private readonly IDieRoller roller = new RandomDieRoller();

        public DiceParserFudgeTests()
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
        public void DiceParser_ParseSingleFudgeDieTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("f", config, this.roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("f", result.DiceExpression);
            Assert.AreEqual(1, result.Results.Count);
            AssertHelpers.IsWithinRangeInclusive(-1, 1, result.Value);
        }

        [TestMethod]
        public void DiceParser_ParseDiceFudgeTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("3f", this.config, this.roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("3f", result.DiceExpression);
            Assert.AreEqual(3, result.Results.Count);
            int sum = 0;
            foreach (TermResult r in result.Results)
            {
                Assert.AreEqual("FudgeDiceTerm.dF", r.Type);
                sum += r.Value;
            }
            Assert.AreEqual(sum, result.Value);
        }

        [TestMethod]
        public void DiceParser_ParseDiceFudgeModifierTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("3f+1", this.config, this.roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("3f+1", result.DiceExpression);
            Assert.AreEqual(3, result.Results.Count);
            int sum = 0;
            foreach (TermResult r in result.Results)
            {
                Assert.AreEqual("FudgeDiceTerm.dF", r.Type);
                sum += r.Value;
            }
            Assert.AreEqual(sum + 1, result.Value);
        }

        [TestMethod]
        public void DiceParser_ParseDiceFudgeKeepTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("6fk4", this.config, this.roller);

            // validate results
            Assert.IsNotNull(result);
            AssertHelpers.AssertDiceChoose(result, "6fk4", "FudgeDiceTerm.dF", 6, 4);
        }

        [TestMethod]
        public void DiceParser_ParseDiceFudgeDropTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("6fp3", this.config, this.roller);

            // validate results
            Assert.IsNotNull(result);
            AssertHelpers.AssertDiceChoose(result, "6fp3", "FudgeDiceTerm.dF", 6, 3);
        }
    }
}
