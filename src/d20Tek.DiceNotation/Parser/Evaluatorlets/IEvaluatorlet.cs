using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser.Evalutors;

internal interface IEvaluatorlet<T> where T : Expression
{
    int Eval(Evaluator evaluator, IDieRoller roller, List<TermResult> terms, T expression);
}
