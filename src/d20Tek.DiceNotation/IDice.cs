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
}
