using d20Tek.DiceNotation.DiceTerms;
using d20Tek.DiceNotation.DieRoller;
using d20Tek.DiceNotation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace d20Tek.DiceNotation.UnitTests.DiceTerms
{
    /// <summary>
    /// Summary description for ConstantTermTests
    /// </summary>
    [TestClass]
    public class ConstantTermTests
    {
        private static readonly IDieRoller dieRoller = new RandomDieRoller();

        public ConstantTermTests()
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
        public void ConstantTerm_ConstructorTest()
        {
            // setup test

            // run test
            IExpressionTerm term = new ConstantTerm(16);

            // validate results
            Assert.IsNotNull(term);
            Assert.IsInstanceOfType(term, typeof(IExpressionTerm));
            Assert.IsInstanceOfType(term, typeof(ConstantTerm));
        }

        [TestMethod]
        public void ConstantTerm_CalculateResultsTest()
        {
            // setup test
            IExpressionTerm term = new ConstantTerm(4);

            // run test
            IReadOnlyList<TermResult> results = term.CalculateResults(dieRoller);

            // validate results
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            TermResult r = results.FirstOrDefault();
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            Assert.AreEqual(4, r.Value);
            Assert.AreEqual("ConstantTerm", r.Type);
        }

        [TestMethod]
        public void ConstantTerm_CalculateResultsNullDieRollerTest()
        {
            // setup test
            IExpressionTerm term = new ConstantTerm(8);

            // run test
            IReadOnlyList<TermResult> results = term.CalculateResults(null);

            // validate results
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            TermResult r = results.FirstOrDefault();
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Scalar);
            Assert.AreEqual(8, r.Value);
            Assert.AreEqual("ConstantTerm", r.Type);
        }

        [TestMethod]
        public void ConstantTerm_ToStringTest()
        {
            // setup test
            IExpressionTerm term = new ConstantTerm(3);

            // run test
            string result = term.ToString();

            // validate results
            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.AreEqual("3", result);
        }
    }
}
