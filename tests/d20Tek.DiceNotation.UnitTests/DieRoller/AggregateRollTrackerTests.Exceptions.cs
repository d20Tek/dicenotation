using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

[TestClass]
public class AggreateRollTrackerExceptionTests
{
    private readonly AggregateRollTracker _tracker = new();

    [TestMethod]
    public void AddDieRollErrorsTest1()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(1, 1, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void AddDieRollErrorsTest2()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(0, 5, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void AddDieRollErrorsTest3()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(-4, 5, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void AddDieRollErrorsTest4()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(6, 8, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void AddDieRollErrorsTest5()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(6, -2, typeof(RandomDieRoller)));
    }

    [TestMethod]
    public void AddDieRollErrorsTest6()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(6, 4, null));
    }

    [TestMethod]
    public void AddDieRollErrorsTest7()
    {
        // arrange

        // act - assert
        Assert.ThrowsExactly<ArgumentException>(
            [ExcludeFromCodeCoverage] () => _tracker.AddDieRoll(6, 4, this.GetType()));
    }
}
