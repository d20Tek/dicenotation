using d20Tek.DiceNotation.Results;

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

    [TestMethod]
    public void Roles_ByDefault_IsKept()
    {
        // arrange

        // act
        var result = new TermResult();

        // validate
        Assert.AreEqual(DieRollRole.Kept, result.Roles);
    }

    [TestMethod]
    public void Roles_WhenSetToCombinedFlags_RetainsAllFlags()
    {
        // arrange

        // act
        var result = new TermResult { Roles = DieRollRole.Exploded | DieRollRole.Dropped };

        // validate
        Assert.IsTrue(result.Roles.HasFlag(DieRollRole.Exploded));
        Assert.IsTrue(result.Roles.HasFlag(DieRollRole.Dropped));
    }
}
