# DiceCli
A command-line utility to help gamers roll random dice based on a well defined dice notation. DiceCli parses the notation and rolls the appropriate dice. This can be use for any types of games including most roleplaying games.

Run Dice-Cli in your favorite shell terminal.

## Features
- Roll random dice from the command line
- Supports:
  - simple dice notation parsing (1d20+5)
  - complex dice notations and combination (4dk3 + d8 + 1)
  - run interactively to keep rolling dice continuously
- Output total result along with individual die rolls.

## Installation
Install globally using the .NET CLI:

```bash
dotnet tool install --global DiceCli
```

To update the tool:

```bash
dotnet tool update --global DiceCli
```

## Usage
```bash
Welcome to DiceCli! Use this tool to roll random dice of any type.

d20> --help
USAGE:
    dice-cli [OPTIONS] [COMMAND]

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    start                   Starts an interactive prompt for this sample CLI
    roll <DICE-NOTATION>    Rolls the dice described by the notation string
    notation                Display details about how dice notations are defined and written
    favorites               Commands to manage and use favorite rolls
```

### Examples
Roll a single dice string:
```bash
dice-cli roll "1d20+5"
```

Run DiceCli interactively:
```bash
dice-cli
```

You can manage a list of favorite rolls for quick access of your common rolls:
```bash
d20> favorites -h
DESCRIPTION:
Commands to manage and use favorite rolls

USAGE:
    dice-cli favorites [OPTIONS] <COMMAND>

EXAMPLES:
    dice-cli favorites list
    dice-cli favorites add -i attack-roll -n Attack (Longsword) -e 1d20+5
    dice-cli favorites edit -i attack-roll -n Attack (Short sword) -e 1d20+7
    dice-cli favorites delete -i damage-roll
    dice-cli favorites roll -i attack-roll

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    list      Lists all favorite rolls
    add       Adds a new favorite roll
    edit      Edits an existing favorite roll
    delete    Removes a favorite roll by name
    roll      Rolls a favorite roll by name
```

To roll a previous saved favorite, run the following command:
```bash
dice-cli favorites roll "attack-roll'
```

## Feedback
If you use this tool and have any feedback, bugs, or suggestions, please file them in the Issues section of this repository.
