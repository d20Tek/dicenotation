using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace d20Tek.DiceNotation.DieRoller.UnitTests
{
    /// <summary>
    /// Summary description for ConstantDieRollerTests
    /// </summary>
    [TestClass]
    public class ConstantDieRollerTests
    {
        public ConstantDieRollerTests()
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
        public void ConstantDieRoller_DefaultConstructorTest()
        {
            // setup test

            // run test
            IDieRoller die = new ConstantDieRoller();

            // validate results
            Assert.IsNotNull(die);
            Assert.IsInstanceOfType(die, typeof(IDieRoller));
            Assert.IsInstanceOfType(die, typeof(ConstantDieRoller));
        }

        [TestMethod]
        public void ConstantDieRoller_RollDefaultConstantTest()
        {
            // setup test
            IDieRoller die = new ConstantDieRoller();

            // run test
            int result = die.Roll(20);

            // validate results
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ConstantDieRoller_RollConstantTest()
        {
            // setup test
            IDieRoller die = new ConstantDieRoller(3);

            // run test
            int result = die.Roll(6);

            // validate results
            Assert.AreEqual(3, result);
        }
    }
}
