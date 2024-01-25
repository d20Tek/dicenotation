using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;
using D20Tek.DiceNotation.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace d20Tek.DiceNotation.UnitTests
{
    /// <summary>
    /// Summary description for DiceParserGroupingTests
    /// </summary>
    [TestClass]
    public class DiceParserGroupingTests
    {
        private readonly DiceConfiguration config = new DiceConfiguration();
        private readonly IDieRoller testRoller = new ConstantDieRoller(2);
        private readonly IDieRoller roller = new RandomDieRoller();

        public DiceParserGroupingTests()
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
        public void DiceParser_ParseParensSimpleTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("(1d20+2)", this.config, this.testRoller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("(1d20+2)", result.DiceExpression);
            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual(4, result.Value);
        }

        [TestMethod]
        public void DiceParser_ParseParensNumDiceTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("(1+3)d8", this.config, this.testRoller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("(1+3)d8", result.DiceExpression);
            Assert.AreEqual(4, result.Results.Count);
            Assert.AreEqual(8, result.Value);
        }

        [TestMethod]
        public void DiceParser_ParseParensSidesTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("2d(2x3)+2", this.config, this.testRoller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("2d(2x3)+2", result.DiceExpression);
            Assert.AreEqual(2, result.Results.Count);
            Assert.AreEqual(6, result.Value);
        }

        [TestMethod]
        public void DiceParser_ParseParensChooseTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("4d(2x3)k(1+2)", this.config, this.testRoller);

            // validate results
            Assert.IsNotNull(result);
            AssertHelpers.AssertDiceChoose(result, "4d(2x3)k(1+2)", "DiceTerm.d6", 4, 3);
        }

        [TestMethod]
        public void DiceParser_ParseParensMultiplyTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("(2d10+1) * 10", this.config, this.testRoller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("(2d10+1)*10", result.DiceExpression);
            Assert.AreEqual(2, result.Results.Count);
            Assert.AreEqual(50, result.Value);
        }


        [TestMethod]
        public void DiceParser_ParseParensDivideTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("(4d10-2) / (1+1)", this.config, this.testRoller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("(4d10-2)/(1+1)", result.DiceExpression);
            Assert.AreEqual(4, result.Results.Count);
            Assert.AreEqual(3, result.Value);
        }

        [TestMethod]
        public void DiceParser_ParseParensFudgeTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("(10f-2) / (1+1)", this.config, this.roller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("(10f-2)/(1+1)", result.DiceExpression);
            Assert.AreEqual(10, result.Results.Count);
            AssertHelpers.IsWithinRangeInclusive(-5, 5, result.Value);
        }

        [TestMethod]
        public void DiceParser_ParseParensComplexTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("(2+1d20+(2+3))x3-10", this.config, this.testRoller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("(2+1d20+(2+3))x3-10", result.DiceExpression);
            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual(17, result.Value);
        }

        [TestMethod]
        public void DiceParser_ParseParensComplex2Test()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("(2+1d20+(2+3))x3-10+()", this.config, this.testRoller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("(2+1d20+(2+3))x3-10+()", result.DiceExpression);
            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual(17, result.Value);
        }

        [TestMethod]
        public void DiceParser_ParseParensComplex3Test()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("(2+1d20+(2+3))x3-10+(3)", this.config, this.testRoller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("(2+1d20+(2+3))x3-10+(3)", result.DiceExpression);
            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual(20, result.Value);
        }

        [TestMethod]
        public void DiceParser_ParseParensComplex4Test()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            DiceResult result = parser.Parse("(((2+1d20)+(2+3))x3-10+(3))", this.config, this.testRoller);

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("(((2+1d20)+(2+3))x3-10+(3))", result.DiceExpression);
            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual(20, result.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        [ExcludeFromCodeCoverage]
        public void DiceParser_ParseParensMismatchEndTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            parser.Parse("(2+1d20+(2+3)x3-10+(3)", this.config, this.testRoller);

            // validate results
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [ExcludeFromCodeCoverage]
        public void DiceParser_ParseParensMismatchStartTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            parser.Parse("(2+1d20+2+3))x3-10+(3)", this.config, this.testRoller);

            // validate results
        }
    }
}
