using d20Tek.DiceNotation.DiceTerms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace d20Tek.DiceNotation.UnitTests.DiceTerms
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DiceTermErrorTests
    {
        [TestMethod]
        public void DiceTerm_ConstructorInvalidNumDiceTest()
        {
            // setup test

            // run test
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DiceTerm(0, 6));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DiceTerm(-5, 6));

            // validate results
        }

        [TestMethod]
        public void DiceTerm_ConstructorInvalidSidesTest()
        {
            // setup test

            // run test
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DiceTerm(3, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DiceTerm(0, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DiceTerm(1, -20));

            // validate results
        }

        [TestMethod]
        public void DiceTerm_ConstructorInvalidScalarTest()
        {
            // setup test

            // run test
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DiceTerm(2, 8, 0));

            // validate results
        }

        [TestMethod]
        public void DiceTerm_ConstructorInvalidChooseTest()
        {
            // setup test

            // run test
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DiceTerm(3, 6, choose: 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DiceTerm(3, 6, choose: -4));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DiceTerm(3, 6, choose: 4));

            // validate results
        }

        [TestMethod]
        public void DiceTerm_ConstructorInvalidExplodingTest()
        {
            // setup test

            // run test
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DiceTerm(3, 6, exploding: 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DiceTerm(3, 6, exploding: -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DiceTerm(3, 6, exploding: 7));

            // validate results
        }
    }
}
