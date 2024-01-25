# Dice Notation Specification
Dice notation (also known as dice algebra, common dice notation, RPG dice notation, and several other titles) is a system to represent different combinations of dice in role-playing games using simple algebra-like notation such as 2d6+12.

### Basic Dice Notation
The number of sides and the number of dice used in a game are conventionally notated as a short sequence of characters.

Regular six-sided dice are notated as d6 and eight-sided dice are d8. The d represents the die. The number of sides on the die is represented by the number following the d.

If you're throwing more than 1 die then precede the letter d with the number of dice your using. So 3 dice that are six-sided are notated as 3d6 and 4 dice that are ten-sided are notated as 4d10. The number of dice used is represented by the number preceding the d.

The notation can be modified with a mathematic operator and number. So 2d6 - 3 means two six-sided dice totalled and then subtracted by 3. The operator could be either -, +, x, /.

__XdY [-] [+] [x] [/] N__          X dice with Y-sides operated on by (-, +, x, /) with number N

A second roll of different dice could be used instead of a fixed operating number.

__XdY [-] [+] [x] [/] AdB__        The first roll of X dice with Y-sides is operated on by (-, +. x, /) with a second roll of A dice with B-sides.

Dice roll subexpressions are allowed to keep highest or drop lowest values:

__XdY[k/p/l]z__                    X dice with Y-sides but either keeping or dropping z of the dice (k => keep z dice; p => drop z dice; l => keep lowest z dice)

__XdY[!][z]__                      Exploding dice: X dice with Y sides, and performs extra rolls for any result >= z. z is optional and if omitted, default to the maximum value of die

Another type of roll for Fudge or FATE dice (these are values fixed to -1, 0, 1).

__Xf [-] [+] [x] [/] N__           X fudge dice operated on by (-, +, x, /) with number N

__d%__ represents a percentile dice. That is one with 100 faces. Two ten sided dice numbered 0 - 9 and 00 - 90 can be used as percentile dice.  Two 10 faced dice numbered 0 to 9 can be used with one die representing 10s and the other representing units. If both dice are rolled 0 then the roll represents 100%.

### Math Operators 
Dice notation supports the basic arithmetic operators in any mathematical formula: +, -, x (multiply), and / (divide).

Operations are performed by order of precedence, just like in normal mathematics. From highest to lowest precedence: 
* Grouping with parentheses (( and )); just like other math expresssions, you can modify the precedence ordering of a dice expression by wrapping parentheses around values 
* Division (/), in the order they appear (left-to-right) in the formula 
* Multiplication (x), in the order they appear (left-to-right) in the formula 
* Subtraction (-), in the order they appear (left-to-right) in the formula 
* Addition (+), in the order they appear (left-to-right) in the formula 

### Order of Operations 
While the Dice Notation does support basic math operations, there are special rules for handling dice operations that modify the order of operations. Here is the general order of operations: 
* First, parentheses are evaluated to group subexpressions and operations are performed on those subexpressions.
* Then, dice are rolled for any dice notation (e.g. "2d6" is rolled; including any special dice operations such as kept).
* This includes all of the modifiers to a dice expression (dice sides, choose operator, etc).
* Then the result of that roll is substituted into the formula. 
* Finally, the entire remaining formula is evaluated, including observing proper math order of operations (parentheses, then multiplication/division, then addition/subtraction). 
