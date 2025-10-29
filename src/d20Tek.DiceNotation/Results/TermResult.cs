namespace d20Tek.DiceNotation.Results;

public class TermResult
{
    public double Scalar { get; set; }

    public int Value { get; set; }

    public string Type { get; set; } = string.Empty;

    public bool AppliesToResultCalculation { get; set; } = true;

    public TermResult(double scalar, int value, string type) => (Scalar, Value, Type) = (scalar, value, type);

    public TermResult() { }
}
