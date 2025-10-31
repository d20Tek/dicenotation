namespace d20Tek.DiceNotation.DieRoller;

public class DieTrackingData
{
    public Guid Id { get; set; }

    public string RollerType { get; set; } = string.Empty;

    public string DieSides { get; set; } = string.Empty;

    public int Result { get; set; }

    public DateTime Timpstamp { get; set; }

    public static DieTrackingData Create(string rollerName, int dieSides, int result) => new()
    {
        Id = Guid.NewGuid(),
        RollerType = rollerName,
        DieSides = dieSides.ToString(),
        Result = result,
        Timpstamp = DateTime.Now,
    };
}
