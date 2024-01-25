//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace d20Tek.DiceNotation;

public class DiceRequest
{
    public int NumberDice { get; private init; }

    public int Sides { get; private init; }

    public double Scalar { get; private init; }

    public int? Choose { get; private init; }

    public int? Exploding { get; private init; }

    public DiceRequest(
        int numberDice,
        int sides,
        double scalar = 1,
        int? choose = null,
        int? exploding = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(numberDice, 0, nameof(numberDice));
        ArgumentOutOfRangeException.ThrowIfLessThan(sides, 1, nameof(sides));
        ArgumentOutOfRangeException.ThrowIfEqual(scalar, 0, nameof(scalar));

        if (choose is not null)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(choose.Value, numberDice, nameof(choose));
            ArgumentOutOfRangeException.ThrowIfLessThan(choose.Value, -numberDice, nameof(choose));
            ArgumentOutOfRangeException.ThrowIfEqual(choose.Value, 0, nameof(choose));
        }

        if (exploding != null)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(exploding.Value, -numberDice, nameof(exploding));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(exploding.Value, sides, nameof(exploding));
        }

        NumberDice = numberDice;
        Sides = sides;
        Scalar = scalar;
        Choose = choose;
        Exploding = exploding;
    }
}