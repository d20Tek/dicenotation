# d20Tek.DiceNotation: Roll the Dice, Skip the Boilerplate

Every game, simulation, and probability tool eventually needs to answer the same deceptively simple
question: what happens when you roll the dice? A player casts a fireball and needs `8d6` fire
damage. A character makes an attack with advantage and rolls `2d20`, keeping the higher result. A
loot table calls for `4d6`, dropping the lowest die. These expressions are second nature to anyone
who has played a tabletop roleplaying game, but turning them into correct, well-tested code is
surprisingly easy to get wrong.

That is exactly the problem d20Tek.DiceNotation solves. It is a small, focused .NET library that
parses, evaluates, and rolls any dice notation string, so you can spend your time building your
game or application instead of debugging modifier math and edge cases.

## What Is Dice Notation?

Dice notation (also called dice algebra or RPG dice notation) is a compact, algebra-like syntax for
describing dice rolls. The expression `2d6+12` means "roll two six-sided dice and add twelve." The
notation scales from the trivial to the sophisticated:

- `d20` - roll a single twenty-sided die.
- `3d6+2` - roll three six-sided dice and add a bonus of two.
- `4d6k3` - roll four six-sided dice and keep the highest three (classic ability score generation).
- `(2+1)d4 - (4-2)` - full arithmetic grouping, evaluated correctly.

d20Tek.DiceNotation understands all of this and more, including percentile dice, Fudge/FATE dice,
keeping the highest or lowest dice, dropping the lowest, and exploding or penetrating rolls.

## Why Developers Choose This Library

**It removes an entire category of bugs.** Parsing arithmetic, honoring operator precedence, and
applying keep/drop rules correctly is fiddly work. This library has already solved it and backs
every feature with unit tests, so your dice math is correct from the first line of code.

**It reads the way you think.** You can build expressions programmatically with a fluent API or
parse a notation string directly. Both paths produce the same evaluated result, so you can choose
whichever fits your scenario.

```csharp
using d20Tek.DiceNotation;
using d20Tek.DiceNotation.DieRoller;

IDice dice = new Dice();

// Parse a notation string...
var result = dice.Roll("4d6k3+2", new RandomDieRoller());

// ...or build the same expression in code.
var expression = DiceExpression.Create()
							   .AddDice(6, 4, choose: 3)
							   .AddConstant(2);
var sameResult = dice.Roll(expression, new RandomDieRoller());

Console.WriteLine($"You rolled: {result.Value}");
```

**It is flexible where it matters.** Randomness is pluggable through the `IDieRoller` interface. Use
the default `RandomDieRoller` for everyday needs, the `CryptoDieRoller` when you want
cryptographically strong randomness, the `MathNetDieRoller` for advanced statistical sources, or the
`ConstantDieRoller` to make your unit tests fully deterministic. If none of those fit, implement
`IDieRoller` yourself.

**It sees the whole roll, not just the total.** Every roll returns a `DiceResult` that exposes the
final value, the individual term results, error information, and a friendly display string. When you
need statistics over time, the built-in roll trackers record every die roll and produce frequency
distributions you can persist to JSON.

**It is modern and portable.** The library targets current .NET, so it drops into any .NET
application: console tools, ASP.NET services, desktop apps, or game backends.

## Use Cases

Dice notation shows up in far more places than a single tabletop game, and d20Tek.DiceNotation is
designed to serve all of them from the same small API. The following scenarios illustrate where the
library fits naturally.

### Tabletop RPG Companion Apps and Virtual Tabletops

Character sheets, initiative trackers, and virtual tabletops all need to resolve dice mechanics on
demand. When a player taps "attack," you can evaluate `1d20+7` for the to-hit roll and `2d6+4` for
damage without hardcoding a single formula. Because expressions are just strings, you can store a
weapon's damage as `2d6+4` in your data model and roll it directly, letting your content drive the
mechanics instead of your code.

```csharp
// Damage stored as data on the weapon definition.
string greatswordDamage = "2d6+4";
DiceResult damage = dice.Roll(greatswordDamage, new RandomDieRoller());
```

### Video Games and Game Servers

Roguelikes, RPGs, and strategy games lean on randomized outcomes for combat, loot, and procedural
content. The library's pluggable `IDieRoller` lets a game server use a seedable or cryptographic
source for fairness and anti-cheat, while a single-player client can use the fast default roller.
The detailed `DiceResult` also gives you every individual die value, so you can drive dice-rolling
animations and combat logs that show players exactly what happened.

### Loot Tables and Procedural Generation

Drop rates, treasure hoards, and encounter tables are naturally expressed as dice. A rule such as
"roll `3d6` gold coins" or "roll on the rare-item table when `1d100` is 96 or higher" becomes a
one-line expression. Designers can tune these values as data, and the engine evaluates them
consistently every time.

### Character and World Generation

Ability score generation is the canonical example: roll `4d6`, keep the highest three, and repeat
six times. The keep-highest and drop-lowest notation handles this directly, and the fluent API makes
it easy to generate a full stat block in a loop. The same approach applies to randomized world maps,
NPC traits, and starting equipment.

```csharp
// Classic 4d6-keep-highest-3 ability score generation.
var abilityScore = DiceExpression.Create().AddDice(6, 4, choose: 3);
int[] stats = Enumerable.Range(0, 6)
                        .Select(_ => dice.Roll(abilityScore, new RandomDieRoller()).Value)
                        .ToArray();
```

### Probability Analysis and Game Balancing

Because the built-in roll trackers record every die roll and produce frequency distributions,
d20Tek.DiceNotation doubles as a lightweight tool for studying outcomes. Roll an expression tens of
thousands of times, then inspect the aggregated results to see how often each value appears. This is
invaluable when balancing a homebrew system, validating that a die roller is fair, or teaching how
probability distributions differ between `3d6` and `1d18`.

### Education and Interactive Learning

Statistics and probability courses often use dice as an intuitive teaching aid. The combination of
readable notation, deterministic rolling for reproducible examples, and frequency tracking makes the
library a natural backend for interactive demos, notebooks, and classroom tools that visualize the
law of large numbers or compare distribution shapes.

### Simulations and Automated Testing

Any Monte Carlo style simulation that models chance can express its random events as dice rolls. And
because the `ConstantDieRoller` returns a fixed value, you can make your own tests fully
deterministic: swap in a constant roller to assert exact outcomes, then switch back to a random
roller in production. The library's error-reporting model, where invalid expressions surface through
`DiceResult.HasError` rather than throwing, keeps batch and simulation workloads robust.

### Chat Bots and Command-Line Tools

A `!roll 2d20kh1` command in a Discord or Slack bot, or a `dice-cli 3d6+2` invocation in a terminal,
maps almost directly onto a single `Roll` call. The included DiceCli sample demonstrates exactly
this pattern, turning user-typed notation into formatted results with minimal glue code.

## Get Started in Minutes

Adding dice to your project is a single package installation away:

```
PM > Install-Package d20tek-dicenotation
```

From there, one `Dice` instance and a single call to `Roll` is all it takes to bring authentic,
correct dice mechanics into your application.

Stop reinventing dice math. Let d20Tek.DiceNotation handle the notation, the parsing, and the
randomness, so you can focus on the experience you are building.

## Next Steps

- Follow the [Getting Started](getting-started.md) guide to install the package and roll your first
  dice.
- Explore the complete [API Reference](api-reference.md) for every public type and member.
