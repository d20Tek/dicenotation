using d20Tek.DiceNotation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace d20Tek.DiceNotation.UnitTests.Results;

[TestClass]
public class TermResultTests
{
    [TestMethod]
    public void ValidateProperties()
    {
        // arrange

        // act
        var result = new TermResult()
        {
            Scalar = 5,
            AppliesToResultCalculation = true,
            Type = "DiceResult",
            Value = 3,
        };

        // validate
        Assert.AreEqual(5, result.Scalar);
        Assert.IsTrue(result.AppliesToResultCalculation);
        Assert.AreEqual("DiceResult", result.Type);
        Assert.AreEqual(3, result.Value);
    }
}
