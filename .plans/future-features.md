# Future Features

This document captures potential future enhancements for the d20Tek.DiceNotation library. The
ideas below build on the current functionality: parsing and rolling dice notation (standard dice,
percentile, fudge/FATE, keep-highest/lowest, drop-lowest, exploding/penetrating dice), grouping and
math expressions, multiple die rollers (Random, Crypto, MathNet, Constant), and roll tracking with
frequency statistics.

## Notation and Parsing Enhancements

- **Rerolls (`r` / `ro`)**: Reroll dice below a threshold, for example `4d6r1` (reroll all 1s) or
  `4d6ro1` (reroll once). Very common in tabletop roleplaying games.
- **Success/pool counting (`>=`, `<=`)**: Count dice meeting a target rather than summing, for
  example `10d6>=5` returns the number of successes. Core to games like World of Darkness and
  Shadowrun.
- **Advantage/Disadvantage sugar**: First-class `adv`/`dis` tokens (roll 2d20, keep highest/lowest)
  since D&D 5e made this pattern ubiquitous.
- **Min/Max clamping per term (`min`/`max`)**: For example, `2d6min2` treats any die below 2 as 2
  (Great Weapon Fighting style).
- **Critical detection metadata**: Flag natural max/min rolls (nat 20 / nat 1) in the result so
  consumers do not have to inspect raw rolls.

## Result and API Enhancements

- [DONE] **Verbose/structured breakdown**: A richer result model showing which dice were kept, dropped,
  rerolled, or exploded, useful for UI display and audit trails.
- **Seeded/replayable rolling** (low priority / nice-to-have): Supply a seed to produce reproducible
  roll sequences, useful for reroll "original versus rerolled" testing and for app-level replay
  (reproducing a reported sequence or replaying a session). The main gaps this
  fills are asserting that a reroll yields a different value than the original, and runtime replay.

  Reproducibility depends on whether the underlying generator is seedable, not merely on being
  "random," so this applies to more than the pseudo-random roller. The cleaner abstraction is a roller
  wrapping a seedable source, capturing `(algorithm, seed)` as the replay token rather than a single
  `SeededDieRoller`. Per-roller support:

  - **RandomDieRoller** (supported): `System.Random` accepts a seed via `new Random(seed)` and
    produces a fully deterministic sequence. This is the natural home for seeded rolling.
  - **MathNetDieRoller** (supported, with caveats): MathNet random sources (for example
    `MersenneTwister`, `Mrg32k3a`, `SystemRandomSource`) accept a seed. Reproducibility is only
    guaranteed for the same generator type, MathNet version, and platform, so replay data must record
    which generator was used, not just the seed.
  - **CryptoDieRoller** (not supported by design): `RandomNumberGenerator` is a CSPRNG that pulls from
    OS entropy and exposes no seed, because reproducibility would undermine its security guarantees.

## Integration and Infrastructure

- [DONE] **DI extension package**: `AddDiceNotation()` service-collection extensions to register `IDice`,
  configuration, roller, and tracker cleanly.

## Highest-Value Priorities

If prioritizing, the following provide the most value:

1. **Rerolls and success-counting notation**: Fills the biggest gaps versus dice expectations.
2. **A DI extensions package**: Low effort, improves adoption in modern .NET applications.
