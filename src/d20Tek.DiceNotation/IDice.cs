using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation;

/// <summary>
/// Defines the primary entry point for parsing and rolling dice expressions.
/// </summary>
public interface IDice
{
    /// <summary>
    /// Gets the configuration that controls default dice behavior for this instance.
    /// </summary>
    IDiceConfiguration Configuration { get; }

    /// <summary>
    /// Rolls the specified dice expression and returns the calculated result.
    /// </summary>
    /// <param name="expression">The dice expression to evaluate.</param>
    /// <param name="dieRoller">
    /// The die roller used to generate random values, or <see langword="null"/> to use the
    /// configured default die roller.
    /// </param>
    /// <returns>A <see cref="DiceResult"/> containing the outcome of the roll.</returns>
    DiceResult Roll(DiceExpression expression, IDieRoller? dieRoller = null);

    /// <summary>
    /// Parses and rolls the specified dice notation and returns the calculated result.
    /// </summary>
    /// <param name="notation">The dice notation to parse and evaluate (for example, "3d6+1").</param>
    /// <param name="dieRoller">
    /// The die roller used to generate random values, or <see langword="null"/> to use the
    /// configured default die roller.
    /// </param>
    /// <returns>A <see cref="DiceResult"/> containing the outcome of the roll.</returns>
    DiceResult Roll(string notation, IDieRoller? dieRoller = null);

    /// <summary>
    /// Rolls the specified dice expression multiple times and returns the calculated result for
    /// each roll.
    /// </summary>
    /// <param name="expression">The dice expression to evaluate.</param>
    /// <param name="count">The number of times to roll the expression.</param>
    /// <param name="dieRoller">
    /// The die roller used to generate random values, or <see langword="null"/> to use the
    /// configured default die roller.
    /// </param>
    /// <returns>
    /// A list of <see cref="DiceResult"/> instances, one for each roll, in the order they were
    /// produced.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is less than 1.
    /// </exception>
    IReadOnlyList<DiceResult> RollMany(DiceExpression expression, int count, IDieRoller? dieRoller = null);

    /// <summary>
    /// Parses the specified dice notation and rolls it multiple times, returning the calculated
    /// result for each roll.
    /// </summary>
    /// <param name="notation">The dice notation to parse and evaluate (for example, "3d6+1").</param>
    /// <param name="count">The number of times to roll the notation.</param>
    /// <param name="dieRoller">
    /// The die roller used to generate random values, or <see langword="null"/> to use the
    /// configured default die roller.
    /// </param>
    /// <returns>
    /// A list of <see cref="DiceResult"/> instances, one for each roll, in the order they were
    /// produced.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is less than 1.
    /// </exception>
    IReadOnlyList<DiceResult> RollMany(string notation, int count, IDieRoller? dieRoller = null);
}
