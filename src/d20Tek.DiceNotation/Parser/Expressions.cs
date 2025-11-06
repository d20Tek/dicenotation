namespace d20Tek.DiceNotation.Parser;

internal abstract record Expression(Position Pos);

internal sealed record GroupExpression(Expression Inner, Position Pos) : Expression(Pos);

internal sealed record NumberExpression(int Value, Position Pos) : Expression(Pos);

internal sealed record BinaryExpression(Expression Left, BinaryOperator Operator, Expression Right, Position Pos) :
    Expression(Pos);

internal sealed record UnaryExpression(UnaryOperator Operator, Expression Operand, Position Pos) : Expression(Pos);

internal abstract record DiceExpressionBase(IReadOnlyList<Modifier> Modifiers, Position Pos) : Expression(Pos);

internal sealed record DiceExpression(
    Expression? CountArg,
    bool HasPercentSides,
    Expression? SidesArg,
    IReadOnlyList<Modifier> Modifiers,
    Position Pos) : DiceExpressionBase(Modifiers, Pos);

internal sealed record FudgeExpression(
    Expression? CountArg,
    IReadOnlyList<Modifier> Modifiers,
    Position Pos) : DiceExpressionBase(Modifiers, Pos);

internal abstract record Modifier(Position Pos);

internal sealed record ExplodingModifier(Expression? ThresholdArg, Position Pos) : Modifier(Pos);

internal sealed record SelectModifier(SelectKind Kind, Expression CountArg, Position Pos) : Modifier(Pos);
