namespace d20Tek.DiceNotation.UnitTests.Helpers;

internal static class DiceRollerTestExtensions
{
    public static int[] RollMultiple(this IDieRoller roller, int rollCount, int sides) =>
        [.. Enumerable.Range(0, rollCount)
                      .Select(_ => roller.Roll(sides))];
}
