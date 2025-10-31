namespace d20Tek.DiceNotation.DieRoller;

public class AggregateDieTrackingData
{
    public string RollerType { get; set; } = string.Empty;

    public string DieSides { get; set; } = string.Empty;

    public int Result { get; set; }

    public int Count { get; set; }

    public float Percentage { get; set; }

    public static AggregateDieTrackingData Create(string rollerType, int dieSides, int result) => new()
    {
        RollerType = rollerType,
        DieSides = dieSides.ToString(),
        Result = result,
        Count = 0,
        Percentage = 0f
    };

    internal void IncrementCount() => Count++;

    internal bool IsEquivalent(int dieSides, int result, Type dieRollerType) => 
        RollerType == dieRollerType.Name && DieSides == dieSides.ToString() && Result == result;
}
