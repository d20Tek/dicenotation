using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace d20Tek.DiceNotation.UnitTests
{
    /// <summary>
    /// Summary description for DiceParserTokenizeTests
    /// </summary>
    [TestClass]
    public class DiceParserTokenizeTests
    {
        public DiceParserTokenizeTests()
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
        public void DiceParser_TokenizeSimpleTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("d20");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("20", result[2]);
        }

        [TestMethod]
        public void DiceParser_TokenizeSimpleConstantTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("42");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("42", result[0]);
        }

        [TestMethod]
        public void DiceParser_TokenizeSimpleModifierTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("1d20+10");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("20", result[2]);
            Assert.AreEqual("+", result[3]);
            Assert.AreEqual("10", result[4]);
        }

        [TestMethod]
        public void DiceParser_TokenizeModifierFirstTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("2+d6");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("2", result[0]);
            Assert.AreEqual("+", result[1]);
            Assert.AreEqual("1", result[2]);
            Assert.AreEqual("d", result[3]);
            Assert.AreEqual("6", result[4]);
        }

        [TestMethod]
        public void DiceParser_TokenizeNegativeModifierTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("1d20-1");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("20", result[2]);
            Assert.AreEqual("-", result[3]);
            Assert.AreEqual("1", result[4]);
        }

        [TestMethod]
        public void DiceParser_TokenizeMultiplyTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("2d6x10");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("2", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("6", result[2]);
            Assert.AreEqual("x", result[3]);
            Assert.AreEqual("10", result[4]);
        }

        [TestMethod]
        public void DiceParser_TokenizeMultiplyFirstTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("10x2d6");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("10", result[0]);
            Assert.AreEqual("x", result[1]);
            Assert.AreEqual("2", result[2]);
            Assert.AreEqual("d", result[3]);
            Assert.AreEqual("6", result[4]);
        }

        [TestMethod]
        public void DiceParser_TokenizeDivideTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("2d6/10");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("2", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("6", result[2]);
            Assert.AreEqual("/", result[3]);
            Assert.AreEqual("10", result[4]);
        }

        [TestMethod]
        public void DiceParser_TokenizeDivideFirstTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("10/2d6");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("10", result[0]);
            Assert.AreEqual("/", result[1]);
            Assert.AreEqual("2", result[2]);
            Assert.AreEqual("d", result[3]);
            Assert.AreEqual("6", result[4]);
        }

        [TestMethod]
        public void DiceParser_TokenizeChooseTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("4d6k3");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("4", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("6", result[2]);
            Assert.AreEqual("k", result[3]);
            Assert.AreEqual("3", result[4]);
        }

        [TestMethod]
        public void DiceParser_TokenizeDropLowestTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("4d6p2");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("4", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("6", result[2]);
            Assert.AreEqual("p", result[3]);
            Assert.AreEqual("2", result[4]);
        }

        [TestMethod]
        public void DiceParser_TokenizeKeepLowestTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("4d6l2");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("4", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("6", result[2]);
            Assert.AreEqual("l", result[3]);
            Assert.AreEqual("2", result[4]);
        }

        [TestMethod]
        public void DiceParser_TokenizeExplodingTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("4d6!5");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("4", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("6", result[2]);
            Assert.AreEqual("!", result[3]);
            Assert.AreEqual("5", result[4]);
        }

        [TestMethod]
        public void DiceParser_TokenizeExplodingNoValueTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("4d6!");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("4", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("6", result[2]);
            Assert.AreEqual("!", result[3]);
        }

        [TestMethod]
        public void DiceParser_TokenizeChainedDiceExpressionTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("4d6k3 + d8 + 3");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(11, result.Count);
            Assert.AreEqual("4", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("6", result[2]);
            Assert.AreEqual("k", result[3]);
            Assert.AreEqual("3", result[4]);
            Assert.AreEqual("+", result[5]);
            Assert.AreEqual("1", result[6]);
            Assert.AreEqual("d", result[7]);
            Assert.AreEqual("8", result[8]);
            Assert.AreEqual("+", result[9]);
            Assert.AreEqual("3", result[10]);
        }

        [TestMethod]
        public void DiceParser_TokenizeUnaryOperatorTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("-1 + 4d6k3");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(7, result.Count);
            Assert.AreEqual("-1", result[0]);
            Assert.AreEqual("+", result[1]);
            Assert.AreEqual("4", result[2]);
            Assert.AreEqual("d", result[3]);
            Assert.AreEqual("6", result[4]);
            Assert.AreEqual("k", result[5]);
            Assert.AreEqual("3", result[6]);
        }

        [TestMethod]
        public void DiceParser_TokenizeUnaryOperatorPositiveTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("+1 + 4d6k3");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(7, result.Count);
            Assert.AreEqual("+1", result[0]);
            Assert.AreEqual("+", result[1]);
            Assert.AreEqual("4", result[2]);
            Assert.AreEqual("d", result[3]);
            Assert.AreEqual("6", result[4]);
            Assert.AreEqual("k", result[5]);
            Assert.AreEqual("3", result[6]);
        }

        [TestMethod]
        public void DiceParser_TokenizeParenthesesSimpleTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("4d6k3 + (d8 - 2)");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("4", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("6", result[2]);
            Assert.AreEqual("k", result[3]);
            Assert.AreEqual("3", result[4]);
            Assert.AreEqual("+", result[5]);
            Assert.AreEqual("(", result[6]);
            Assert.AreEqual("1", result[7]);
            Assert.AreEqual("d", result[8]);
            Assert.AreEqual("8", result[9]);
            Assert.AreEqual("-", result[10]);
            Assert.AreEqual("2", result[11]);
            Assert.AreEqual(")", result[12]);
        }

        [TestMethod]
        public void DiceParser_TokenizeParenthesesMuliplyTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("(2d6 + 1)x2");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(9, result.Count);
            Assert.AreEqual("(", result[0]);
            Assert.AreEqual("2", result[1]);
            Assert.AreEqual("d", result[2]);
            Assert.AreEqual("6", result[3]);
            Assert.AreEqual("+", result[4]);
            Assert.AreEqual("1", result[5]);
            Assert.AreEqual(")", result[6]);
            Assert.AreEqual("x", result[7]);
            Assert.AreEqual("2", result[8]);
        }

        [TestMethod]
        public void DiceParser_TokenizeDefaultMuliplyTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("(2d6 + 1)2");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(9, result.Count);
            Assert.AreEqual("(", result[0]);
            Assert.AreEqual("2", result[1]);
            Assert.AreEqual("d", result[2]);
            Assert.AreEqual("6", result[3]);
            Assert.AreEqual("+", result[4]);
            Assert.AreEqual("1", result[5]);
            Assert.AreEqual(")", result[6]);
            Assert.AreEqual("x", result[7]);
            Assert.AreEqual("2", result[8]);
        }

        [TestMethod]
        public void DiceParser_TokenizeDefaultMuliplyFirstTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("2(2d6 + 1)");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(9, result.Count);
            Assert.AreEqual("2", result[0]);
            Assert.AreEqual("x", result[1]);
            Assert.AreEqual("(", result[2]);
            Assert.AreEqual("2", result[3]);
            Assert.AreEqual("d", result[4]);
            Assert.AreEqual("6", result[5]);
            Assert.AreEqual("+", result[6]);
            Assert.AreEqual("1", result[7]);
            Assert.AreEqual(")", result[8]);
        }

        [TestMethod]
        public void DiceParser_TokenizeParenthesesNestedTest()
        {
            // setup test
            DiceParser parser = new DiceParser();

            // run test
            List<string> result = parser.Tokenize("4d6k3 - (2 + 3) + (2(d8 - 2))");

            // validate results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual(23, result.Count);
            Assert.AreEqual("4", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("6", result[2]);
            Assert.AreEqual("k", result[3]);
            Assert.AreEqual("3", result[4]);
            Assert.AreEqual("-", result[5]);
            Assert.AreEqual("(", result[6]);
            Assert.AreEqual("2", result[7]);
            Assert.AreEqual("+", result[8]);
            Assert.AreEqual("3", result[9]);
            Assert.AreEqual(")", result[10]);
            Assert.AreEqual("+", result[11]);
            Assert.AreEqual("(", result[12]);
            Assert.AreEqual("2", result[13]);
            Assert.AreEqual("x", result[14]);
            Assert.AreEqual("(", result[15]);
            Assert.AreEqual("1", result[16]);
            Assert.AreEqual("d", result[17]);
            Assert.AreEqual("8", result[18]);
            Assert.AreEqual("-", result[19]);
            Assert.AreEqual("2", result[20]);
            Assert.AreEqual(")", result[21]);
            Assert.AreEqual(")", result[22]);
        }
    }
}
