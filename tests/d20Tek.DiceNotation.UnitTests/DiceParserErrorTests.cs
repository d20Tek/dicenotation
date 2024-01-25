using Microsoft.VisualStudio.TestTools.UnitTesting;
using d20Tek.DiceNotation.DieRoller;
using System;
using System.Diagnostics.CodeAnalysis;

namespace d20Tek.DiceNotation.UnitTests
{
    /// <summary>
    /// Summary description for DiceParserErrorTests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DiceParserErrorTests
    {
        private readonly DiceConfiguration config = new DiceConfiguration();
        readonly IDieRoller roller = new ConstantDieRoller(2);

        public DiceParserErrorTests()
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
        public void DiceParser_UnrecognizedOperatorTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            Assert.ThrowsException<FormatException>(() => parser.Parse("1d20g4", this.config, roller));

            // validate results
        }

        [TestMethod]
        public void DiceParser_ParseDiceDropNumberErrorTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            Assert.ThrowsException<FormatException>(() => parser.Parse("4d6p4", this.config, roller));

            // validate results
        }

        [TestMethod]
        public void DiceParser_ParseDiceOperatorNoValueTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            Assert.ThrowsException<FormatException>(() => parser.Parse("2d4x", this.config, roller));
            Assert.ThrowsException<FormatException>(() => parser.Parse("2d4/", this.config, roller));
            Assert.ThrowsException<FormatException>(() => parser.Parse("2d4k", this.config, roller));
            Assert.ThrowsException<FormatException>(() => parser.Parse("2d4l", this.config, roller));
            Assert.ThrowsException<FormatException>(() => parser.Parse("2+l2d4", this.config, roller));

            // validate results
        }

        [TestMethod]
        public void DiceParser_ParseDiceOperatorMultipleTimesTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            Assert.ThrowsException<FormatException>(() => parser.Parse("2d4k1k2", this.config, roller));
            Assert.ThrowsException<FormatException>(() => parser.Parse("2d4l1l2", this.config, roller));

            // validate results
        }

        [TestMethod]
        public void DiceParser_ParseRandomStringsTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            Assert.ThrowsException<FormatException>(() => parser.Parse("eosnddik+9", this.config, roller));
            Assert.ThrowsException<FormatException>(() => parser.Parse("2drk4/9", this.config, roller));
            Assert.ThrowsException<FormatException>(() => parser.Parse("7y+2d4k4", this.config, roller));
            Assert.ThrowsException<FormatException>(() => parser.Parse("7!y+2d4", this.config, roller));
            // validate results
        }

        [TestMethod]
        public void DiceParser_ParseDicePercentilErrorTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            Assert.ThrowsException<FormatException>(() => parser.Parse("2d6%3", this.config, roller));

            // validate results
        }

        [TestMethod]
        public void DiceParser_ParseDiceFudgeErrorTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            Assert.ThrowsException<FormatException>(() => parser.Parse("2d6f", this.config, roller));
            Assert.ThrowsException<FormatException>(() => parser.Parse("6fd", this.config, roller));

            // validate results
        }

        [TestMethod]
        public void DiceParser_ParseDiceEmptyNullExpressionTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            Assert.ThrowsException<ArgumentNullException>(() => parser.Parse("", this.config, roller));
            Assert.ThrowsException<ArgumentNullException>(() => parser.Parse(null, this.config, roller));

            // validate results
        }
    }
}
