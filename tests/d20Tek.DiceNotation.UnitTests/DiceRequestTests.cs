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
            Assert.AreEqual(2, request.NumberDice);
            Assert.AreEqual(6, request.Sides);
            Assert.AreEqual(1, request.Scalar);
            Assert.IsNull(request.Choose);
            Assert.IsNull(request.Exploding);
        }

        [TestMethod]
        public void DiceRequest_ValidFullCreation()
        {
            // arrange

            // act
            var request = new DiceRequest(4, 6, 2, 1, 3, 1);

            // assert
            Assert.AreEqual(4, request.NumberDice);
            Assert.AreEqual(6, request.Sides);
            Assert.AreEqual(2, request.Bonus);
            Assert.AreEqual(1, request.Scalar);
            Assert.AreEqual(3, request.Choose);
            Assert.AreEqual(1, request.Exploding);
        }
    }
}
