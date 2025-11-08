using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser.Evalutors;

internal class ModifierEvaluator
{
    public int? EvalChoose(
        Evaluator evaluator,
        IReadOnlyList<Modifier> mods,
        int diceCount,
        IDieRoller roller,
        List<TermResult> terms)
    {
        var selectMod = mods.OfType<SelectModifier>().LastOrDefault();
        return selectMod switch
        {
            { Kind: SelectKind.KeepHigh } => evaluator.EvalInternal(selectMod.CountArg, roller, terms),
            { Kind: SelectKind.DropLow } => diceCount - evaluator.EvalInternal(selectMod.CountArg, roller, terms),
            { Kind: SelectKind.KeepLow } => -evaluator.EvalInternal(selectMod.CountArg, roller, terms),
            _ => null
        };
    }

    public int? EvalExploding(
        Evaluator evaluator,
        IReadOnlyList<Modifier> mods,
        int diceCount,
        IDieRoller roller,
        List<TermResult> terms)
    {
        var mod = mods.OfType<ExplodingModifier>().LastOrDefault();
        if (mod is null) return null;

        return mod.ThresholdArg is null ? diceCount : evaluator.EvalInternal(mod.ThresholdArg, roller, terms);
    }
}
