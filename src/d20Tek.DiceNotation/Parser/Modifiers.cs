namespace d20Tek.DiceNotation.Parser;

internal abstract record Modifier(Position Pos);

internal sealed record ExplodingModifier(Expression? ThresholdArg, Position Pos) : Modifier(Pos);

internal sealed record SelectModifier(SelectKind Kind, Expression CountArg, Position Pos) : Modifier(Pos);
