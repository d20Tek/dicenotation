using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace d20Tek.DiceNotation.UnitTests
{
    [TestClass]
    public class DiceRequestTests
    {
        [TestMethod]
        public void DiceRequest_ValidCreation()
        {
            // arrange

            // act
            var request = new DiceRequest(2, 6);

            // assert
            Assert.AreEqual(request.NumberDice, 2);
            Assert.AreEqual(request.Sides, 6);
            Assert.AreEqual(request.Scalar, 1);
            Assert.IsNull(request.Choose);
            Assert.IsNull(request.Exploding);
        }

        [TestMethod]
        public void DiceRequest_ValidFullCreation()
        {
            // arrange

            // act
            var request = new DiceRequest(4, 6, 1, 3, 1);

            // assert
            Assert.AreEqual(request.NumberDice, 4);
            Assert.AreEqual(request.Sides, 6);
            Assert.AreEqual(request.Scalar, 1);
            Assert.AreEqual(request.Choose, 3);
            Assert.AreEqual(request.Exploding, 1);
        }
    }
}
