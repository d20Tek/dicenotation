namespace DiceCli.Commands;

internal sealed class NotationCommand(IAnsiConsole console) : Command
{
    private readonly IAnsiConsole _console = console;

    public override int Execute(CommandContext context, CancellationToken cancellation)
    {
        _console.WriteMessages(
            "[bold]Basic Dice Notation[/]",
            "-------------------",
            "The number of sides and the number of dice used in a game are conventionally",
            "notated as a short sequence of characters.",
            "Regular six-sided dice are notated as d6 and eight-sided dice are d8. The d",
            "represents the die. The number of sides on the die is represented by the number",
            "following the d.",
            "If you're throwing more than 1 die then precede the letter d with the number of",
            "dice your using. So 3 dice that are six-sided are notated as 3d6 and 4 dice that",
            "are ten-sided are notated as 4d10. The number of dice used is represented by the",
            "number preceding the d.",
            "The notation can be modified with a mathematic operator and number. So 2d6 - 3",
            "means two six-sided dice totalled and then subtracted by 3. The operator could be",
            "either -, +, x, /.",
            "",
            "[bold]Notation Examples[/]",
            "-----------------",
            "XdY [[-]] [[+]] [[x]] [[/]] N     => X dice with Y-sides operated on by (-, +, x, /) with",
            "number N (ie: 1d20+5).",
            "XdY [[-]] [[+]] [[x]] [[/]] AdB   => The first roll of X dice with Y-sides is operated",
            "on by (-, +. x, /) with a second roll of A dice with B-sides (ie: 3d6+1d4).",
            "XdY[[k/p/l]]z               => X dice with Y-sides but either keeping or dropping z",
            "of the dice (k ⇒ keep z dice; p ⇒ drop z dice; l ⇒ keep lowest z dice) (ie: ",
            "46k3).",
            "XdY[[!]][[z]]                 => Exploding dice: X dice with Y sides, and performs",
            "extra rolls for any result >= z. z is optional and if omitted, default to the",
            "maximum value of die (ie: 36!6).",
            "d%                        => Represents a percentile dice (1-100).",
            "",
            "Another type of roll for Fudge or FATE dice (these are values fixed to -1, 0, 1).",
            "Xf [[-]] [[+]] [[x]] [[/]] N      => X fudge dice operated on by (-, +, x, /) with",
            "number N (3f+1).",
            "",
            "[bold]Order of Operations[/]",
            "-------------------",
            "- First, parentheses are evaluated to group subexpressions and operations are",
            "  performed on those subexpressions.",
            "- Then, dice are rolled for any dice notation (e.g. '2d6' is rolled; including",
            "  any special dice operations such as kept).",
            "- This includes all of the modifiers to a dice expression (dice sides, choose",
            "  operator, etc).",
            "- Then the result of that roll is substituted into the formula.",
            "- Finally, the entire remaining formula is evaluated, including observing proper",
            "  math order of operations (parentheses, then multiplication/division, then",
            "  addition/subtraction).");

        return 0;
    }
}
