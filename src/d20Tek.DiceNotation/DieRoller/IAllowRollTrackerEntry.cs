namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Defines the ability to record individual die roll entries with a tracker.
/// </summary>
public interface IAllowRollTrackerEntry
{
    /// <summary>
    /// Records a single die roll with the tracker.
    /// </summary>
    /// <param name="dieSides">The number of sides on the die that was rolled.</param>
    /// <param name="result">The value produced by the roll.</param>
    /// <param name="dieRoller">The type of die roller that produced the roll.</param>
    void AddDieRoll(int dieSides, int result, Type dieRoller);
}
