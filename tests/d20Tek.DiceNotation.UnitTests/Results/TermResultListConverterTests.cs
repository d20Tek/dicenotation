using d20Tek.DiceNotation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace d20Tek.DiceNotation.UnitTests
{
    /// <summary>
    /// Summary description for TermResultListConverterTests
    /// </summary>
    [TestClass]
    public class TermResultListConverterTests
    {
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
        public void TermResultListConverter_ConstructorTest()
        {
            // setup test

            // run test
            TermResultListConverter conv = new TermResultListConverter();

            // validate results
            Assert.IsNotNull(conv);
            Assert.IsInstanceOfType(conv, typeof(TermResultListConverter));
        }

        [TestMethod]
        public void TermResultListConverter_ConvertTextTest()
        {
            // setup test
            TermResultListConverter conv = new TermResultListConverter();
            DiceResult diceResult = new DiceResult
            {
                DiceExpression = "d6",
                DieRollerUsed = "ConstantDieRoller",
                Results = new List<TermResult>
                {
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                },
                Value = 3,
            };

            // run test
            string result = conv.Convert(diceResult.Results, typeof(string), null, "en-us") as string;

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("3", result);
        }

        [TestMethod]
        public void TermResultListConverter_ConvertChooseTextTest()
        {
            // setup test
            TermResultListConverter conv = new TermResultListConverter();
            DiceResult diceResult = new DiceResult
            {
                DiceExpression = "6d6k3",
                DieRollerUsed = "ConstantDieRoller",
                Results = new List<TermResult>
                {
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = false },
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = false },
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = false },
                },
                Value = 9,
            };

            // run test
            string result = conv.Convert(diceResult.Results, typeof(string), null, "en-us") as string;

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("3, 3, 3, 3*, 3*, 3*", result);
        }

        [TestMethod]
        public void TermResultListConverter_ConvertComplexTextTest()
        {
            // setup test
            TermResultListConverter conv = new TermResultListConverter();
            DiceResult diceResult = new DiceResult
            {
                DiceExpression = "4d6k3+d8+5",
                DieRollerUsed = "ConstantDieRoller",
                Results = new List<TermResult>
                {
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = false },
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                },
                Value = 17,
            };

            // run test
            string result = conv.Convert(diceResult.Results, typeof(string), null, "en-us") as string;

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("3, 3, 3, 3*, 3", result);
        }

        [TestMethod]
        public void TermResultListConverter_ConvertEmptyResultListTest()
        {
            // setup test
            TermResultListConverter conv = new TermResultListConverter();
            IReadOnlyList<TermResult> list = new List<TermResult>();

            // run test
            string result = conv.Convert(list, typeof(string), null, "en-us") as string;

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [ExcludeFromCodeCoverage]
        public void TermResultListConverter_ConvertErrorTargetTypeTest()
        {
            // setup test
            TermResultListConverter conv = new TermResultListConverter();
            DiceResult diceResult = new DiceResult
            {
                DiceExpression = "d20",
                DieRollerUsed = "ConstantDieRoller",
                Results = new List<TermResult>
                {
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                },
                Value = 3,
            };

            // run test
            conv.Convert(diceResult.Results, typeof(int), null, "en-us");

            // validate results
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [ExcludeFromCodeCoverage]
        public void TermResultListConverter_ConvertErrorValueNullTest()
        {
            // setup test
            TermResultListConverter conv = new TermResultListConverter();

            // run test
            conv.Convert(null, typeof(string), null, "en-us");

            // validate results
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [ExcludeFromCodeCoverage]
        public void TermResultListConverter_ConvertErrorValueTypeTest()
        {
            // setup test
            TermResultListConverter conv = new TermResultListConverter();
            string value = "testString";

            // run test
            conv.Convert(value, typeof(string), null, "en-us");

            // validate results
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        [ExcludeFromCodeCoverage]
        public void TermResultListConverter_ConvertBackTest()
        {
            // setup test
            TermResultListConverter conv = new TermResultListConverter();
            string value = "testString";

            // run test
            conv.ConvertBack(value, typeof(string), null, "en-us");

            // validate results
        }
    }
}
