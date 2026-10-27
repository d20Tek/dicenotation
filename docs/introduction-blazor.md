# Introduction

## d20Tek.DiceNotation.Blazor

`d20Tek.DiceNotation.Blazor` is a Blazor component library that animates dice rolls produced by the
[d20Tek.DiceNotation](introduction.md) library. A roll is still evaluated by the dice library, and
the component simply presents the outcome as dice tumbling across a tray.

## What it does

The `VisualDiceRoller` component takes a finished `DiceResult`, launches a two-dimensional physics
simulation for the dice in that result, and draws each frame on an HTML canvas. When the dice come
to rest, the component reports a `DiceRollSummary` describing the expression, the total, and every
individual die value.

## Design principle: results drive visuals

The most important rule in this library is that the animation never decides anything. The value of
a die is fixed before the simulation starts and is drawn onto the die face throughout the roll. The
physics determines where a die lands and how it tumbles, not what it shows.

This matters for a game application. A roll must remain auditable and reproducible, and it must
agree with whatever the application already logged or sent to a server. If a rendering or interop
failure occurs, the result is still valid and the application should keep displaying it.

## Features

- Physics-driven dice that tumble, collide, and settle inside a tray.
- Shape-aware rendering: a d2 is a round coin, a d4 is a triangle, a d6 is a square, a d8 is a
  diamond, a d10 and d100 are kites, a d12 is a pentagon, and a d20 is a hexagon. The physics
  bodies use the same outlines, so a die never collides as one shape while being drawn as another.
- Themes, including the built-in `Default`, `DarkFantasy`, and `Parchment` presets.
- Configuration through `DiceRollerOptions` for tray size, theme, visual settings, simulation
  settings, and reduced motion.
- `ReplayAsync` to re-animate the current result without rolling again.
- An accessible label generated from the settled result, plus a reduced-motion mode that disables
  face churn, implied height, and motion smear.

## When to use it

Use this package when an application already rolls dice with `d20Tek.DiceNotation` and wants a
visual presentation of those rolls. It suits virtual tabletops, character generators, and any
application where a player expects to watch dice land rather than read a number appear.

If an application only needs the numbers, the core `d20Tek.DiceNotation` package is sufficient on
its own.

## Supported targets

- .NET 9 and .NET 10.
- Blazor WebAssembly and Blazor Server.

## Next steps

- [Getting started](getting-started-blazor.md) walks through installation, registration, and a
  first working roll.
- [API reference](api-reference-blazor.md) documents the component parameters, options, themes, and
  result types.
