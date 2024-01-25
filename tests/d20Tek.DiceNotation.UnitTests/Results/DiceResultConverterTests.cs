using d20Tek.DiceNotation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace d20Tek.DiceNotation.UnitTests
{
    /// <summary>
    /// Summary description for DiceResultConverterTests
    /// </summary>
    [TestClass]
    public class DiceResultConverterTests
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
        public void DiceResultConverter_ConstructorTest()
        {
            // setup test

            // run test
            DiceResultConverter conv = new DiceResultConverter();

            // validate results
            Assert.IsNotNull(conv);
            Assert.IsInstanceOfType(conv, typeof(DiceResultConverter));
        }


        [TestMethod]
        public void DiceResultConverter_ConvertTextTest()
        {
            // setup test
            DiceResultConverter conv = new DiceResultConverter();
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
            string result = conv.Convert(diceResult, typeof(string), null, "en-us") as string;

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("3 (d6)", result);
        }

        [TestMethod]
        public void DiceResultConverter_ConvertComplexTextTest()
        {
            // setup test
            DiceResultConverter conv = new DiceResultConverter();
            //DiceResult diceResult = this.dice.Roll("4d6k3 + d8 + 5", this.roller);
            DiceResult diceResult = new DiceResult
            {
                DiceExpression = "4d6k3+d8+5",
                DieRollerUsed = "ConstantDieRoller",
                Results = new List<TermResult>
                {
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                    new TermResult { Scalar = 1, Type = "DiceTerm", Value = 3, AppliesToResultCalculation = true },
                },
                Value = 17,
            };

            // run test
            string result = conv.Convert(diceResult, typeof(string), null, "en-us") as string;

            // validate results
            Assert.IsNotNull(result);
            Assert.AreEqual("17 (4d6k3+d8+5)", result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [ExcludeFromCodeCoverage]
        public void DiceResultConverter_ConvertErrorTargetTypeTest()
        {
            // setup test
            DiceResultConverter conv = new DiceResultConverter();
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
            conv.Convert(diceResult, typeof(int), null, "en-us");

            // validate results
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [ExcludeFromCodeCoverage]
        public void DiceResultConverter_ConvertErrorValueNullTest()
        {
            // setup test
            DiceResultConverter conv = new DiceResultConverter();

            // run test
            conv.Convert(null, typeof(string), null, "en-us");

            // validate results
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [ExcludeFromCodeCoverage]
        public void DiceResultConverter_ConvertErrorValueTypeTest()
        {
            // setup test
            DiceResultConverter conv = new DiceResultConverter();
            string value = "testString";

            // run test
            conv.Convert(value, typeof(string), null, "en-us");

            // validate results
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        [ExcludeFromCodeCoverage]
        public void DiceResultConverter_ConvertBackTest()
        {
            // setup test
            DiceResultConverter conv = new DiceResultConverter();
            string value = "testString";

            // run test
            conv.ConvertBack(value, typeof(string), null, "en-us");

            // validate results
        }
    }
}
