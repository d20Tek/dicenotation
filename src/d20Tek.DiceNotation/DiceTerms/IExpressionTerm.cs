using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.DiceTerms;

/// <summary>
/// Represents a single term within a dice expression that can be evaluated to produce results.
/// </summary>
public interface IExpressionTerm
{
    /// <summary>
    /// Calculates the results of this term using the specified die roller.
    /// </summary>
    /// <param name="dieRoller">The die roller used to generate random values.</param>
    /// <returns>The list of <see cref="TermResult"/> values produced by this term.</returns>
    IReadOnlyList<TermResult> CalculateResults(IDieRoller dieRoller);
}
