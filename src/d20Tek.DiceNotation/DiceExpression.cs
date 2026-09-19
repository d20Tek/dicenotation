using d20Tek.DiceNotation.DiceTerms;

namespace d20Tek.DiceNotation;

/// <summary>
/// Represents a composable dice expression built from constant, dice, and fudge dice terms.
/// </summary>
public class DiceExpression
{
    private readonly List<IExpressionTerm> _terms = [];

    /// <summary>
    /// Creates a new, empty <see cref="DiceExpression"/> instance.
    /// </summary>
    /// <returns>A new <see cref="DiceExpression"/>.</returns>
    public static DiceExpression Create() => new();

    /// <summary>
    /// Adds a constant term to the expression.
    /// </summary>
    /// <param name="constant">The constant value to add. A value of zero is ignored.</param>
    /// <returns>The current <see cref="DiceExpression"/> instance for chaining.</returns>
    public DiceExpression AddConstant(int constant)
    {
        if (constant != 0) _terms.Add(new ConstantTerm(constant));
        return this;
    }

    /// <summary>
    /// Adds a dice term to the expression.
    /// </summary>
    /// <param name="sides">The number of sides on each die.</param>
    /// <param name="numberDice">The number of dice to roll.</param>
    /// <param name="scalar">A multiplier applied to the term result.</param>
    /// <param name="choose">
    /// The number of highest dice to keep, or <see langword="null"/> to keep all dice.
    /// </param>
    /// <param name="exploding">
    /// The threshold at which dice explode, or <see langword="null"/> for no exploding dice.
    /// </param>
    /// <returns>The current <see cref="DiceExpression"/> instance for chaining.</returns>
    public DiceExpression AddDice(
        int sides,
        int numberDice = 1,
        double scalar = 1,
        int? choose = null,
        int? exploding = null) =>
        AddDiceTerm(new DiceTerm(numberDice, sides, scalar, choose, exploding));

    /// <summary>
    /// Adds a Fudge/FATE dice term to the expression.
    /// </summary>
    /// <param name="numberDice">The number of Fudge dice to roll.</param>
    /// <param name="choose">
    /// The number of highest dice to keep, or <see langword="null"/> to keep all dice.
    /// </param>
    /// <returns>The current <see cref="DiceExpression"/> instance for chaining.</returns>
    public DiceExpression AddFudgeDice(int numberDice = 1, int? choose = null) =>
        AddDiceTerm(new FudgeDiceTerm(numberDice, choose));

    /// <summary>
    /// Removes all terms from the expression.
    /// </summary>
    /// <returns>The current <see cref="DiceExpression"/> instance for chaining.</returns>
    public DiceExpression Clear()
    {
        _terms.Clear();
        return this;
    }

    /// <summary>
    /// Appends the terms of another expression to this expression.
    /// </summary>
    /// <param name="otherDice">The expression whose terms are appended.</param>
    /// <returns>The current <see cref="DiceExpression"/> instance for chaining.</returns>
    public DiceExpression Concat(DiceExpression otherDice)
    {
        _terms.AddRange(otherDice._terms);
        return this;
    }

    /// <summary>
    /// Returns the read-only list of terms that make up this expression.
    /// </summary>
    /// <returns>The terms contained in the expression.</returns>
    public IReadOnlyList<IExpressionTerm> Evaluate() => _terms;

    /// <summary>
    /// Returns the dice notation representation of this expression.
    /// </summary>
    /// <returns>The dice notation string.</returns>
    public override string ToString() => Constants.JoinSigns(_terms);

    private DiceExpression AddDiceTerm(IExpressionTerm term)
    {
        _terms.Add(term);
        return this;
    }
}
