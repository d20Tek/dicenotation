using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;
using D20Tek.DiceNotation.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace d20Tek.DiceNotation.UnitTests.DiceTerms
{

    /// <summary>
    /// Summary description for FudgeDiceTermTests
    /// </summary>
    [TestClass]
    public class FudgeDiceTermTests
    {
        private readonly IDieRoller roller = new RandomDieRoller();

        public FudgeDiceTermTests()
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
        public void FudgeDiceTerm_ConstructorTest()
        {
            // setup test

            // run test
            IExpressionTerm term = new FudgeDiceTerm(3);

            // validate results
            Assert.IsNotNull(term);
            Assert.IsInstanceOfType(term, typeof(IExpressionTerm));
            Assert.IsInstanceOfType(term, typeof(FudgeDiceTerm));
        }

        [TestMethod]
        public void DiceTerm_CalculateResultsTest()
        {
            // setup test
            IExpressionTerm term = new FudgeDiceTerm(1);

            // run test
            IReadOnlyList<TermResult> results = term.CalculateResults(roller);

            // validate results
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            TermResult r = results.FirstOrDefault();
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            AssertHelpers.IsWithinRangeInclusive(-1, 1, r.Value);
            Assert.AreEqual("FudgeDiceTerm.dF", r.Type);
        }

        [TestMethod]
        public void DiceTerm_CalculateResultsMultipleDiceTest()
        {
            // setup test
            IExpressionTerm term = new FudgeDiceTerm(3);

            // run test
            IReadOnlyList<TermResult> results = term.CalculateResults(roller);

            // validate results
            Assert.IsNotNull(results);
            Assert.AreEqual(3, results.Count);
            foreach (TermResult r in results)
            {
                Assert.IsNotNull(r);
                Assert.AreEqual(1, r.Scalar);
                AssertHelpers.IsWithinRangeInclusive(-1, 1, r.Value);
                Assert.AreEqual("FudgeDiceTerm.dF", r.Type);
            }
        }

        [TestMethod]
        public void DiceTerm_CalculateResultsChooseDiceTest()
        {
            // setup test
            IExpressionTerm term = new FudgeDiceTerm(5, 3);

            // run test
            IReadOnlyList<TermResult> results = term.CalculateResults(roller);

            // validate results
            Assert.IsNotNull(results);
            Assert.AreEqual(5, results.Count);
            int included = 0;
            foreach (TermResult r in results)
            {
                Assert.IsNotNull(r);
                Assert.AreEqual(1, r.Scalar);
                AssertHelpers.IsWithinRangeInclusive(-1, 1, r.Value);
                Assert.AreEqual("FudgeDiceTerm.dF", r.Type);
                if (r.AppliesToResultCalculation) included++;
            }
            Assert.AreEqual(3, included);
        }

        [TestMethod]
        [ExcludeFromCodeCoverage]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FudgeDiceTerm_CalculateResultsNullDieRollerTest()
        {
            // setup test
            IExpressionTerm term = new FudgeDiceTerm(1);

            // run test
            term.CalculateResults(null);

            // validate results
        }

        [TestMethod]
        public void FudgeDiceTerm_ToStringTest()
        {
            // setup test
            IExpressionTerm term = new FudgeDiceTerm(2);

            // run test
            string result = term.ToString();

            // validate results
            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.AreEqual("2f", result);
        }

        [TestMethod]
        public void FudgeDiceTerm_ToStringChooseTest()
        {
            // setup test
            IExpressionTerm term = new FudgeDiceTerm(5, 3);

            // run test
            string result = term.ToString();

            // validate results
            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.AreEqual("5fk3", result);
        }

        [TestClass]
        [ExcludeFromCodeCoverage]
        public class FudgeDiceTermErrorTests
        {
            [TestMethod]
            public void FudgeDiceTerm_ConstructorInvalidNumDiceTest()
            {
                // setup test

                // run test
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(0));
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(-5));

                // validate results
            }

            [TestMethod]
            public void FudgeDiceTerm_ConstructorInvalidChooseTest()
            {
                // setup test

                // run test
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(3, choose: 0));
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(3, choose: -4));
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => new FudgeDiceTerm(3, choose: 4));

                // validate results
            }
        }
    }
}
