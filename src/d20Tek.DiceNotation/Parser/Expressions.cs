namespace d20Tek.DiceNotation.Parser;

internal abstract record Expression;

internal sealed record NumberExpression(int Value) : Expression;

internal sealed record DiceExpression(int Count, int Sides, int? Keep, int? Explode) : Expression;

internal sealed record BinaryExpression(Expression Left, string Operator, Expression Right) : Expression;

internal sealed record GroupExpression(Expression Inner) : Expression;
