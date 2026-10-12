using d20Tek.DiceNotation.Results;

namespace d20Tek.DiceNotation.Parser.Evaluatorlets;

internal class ModifierEvaluator
{
    public int? EvalChoose(
        Evaluator evaluator,
        IReadOnlyList<Modifier> mods,
        int diceCount,
        IDieRoller roller,
        List<TermResult> terms)
    {
        SelectModifier? selectMod = null;
        for (int i = mods.Count - 1; i >= 0; i--)
        {
            if (mods[i] is SelectModifier sm)
            {
                selectMod = sm;
                break;
            }
        }

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
        ExplodingModifier? mod = null;
        for (int i = mods.Count - 1; i >= 0; i--)
        {
            if (mods[i] is ExplodingModifier em)
            {
                mod = em;
                break;
            }
        }

        if (mod is null) return null;

        return mod.ThresholdArg is null ? diceCount : evaluator.EvalInternal(mod.ThresholdArg, roller, terms);
    }
}
