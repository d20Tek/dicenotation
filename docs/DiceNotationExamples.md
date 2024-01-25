# Examples of Dice Notations

The OnePlat.DiceNotation library supports many complex dice expressions. There is a well defined [dice language](DiceNotationSpecCurrent.md) for what is possible, but sometimes those are hard to wrap your head around. So here are some example expressions that may help better understand what is possible.

There are more possible combinations than are shown here, but this gives you a flavor for what you can create. You can experiment with the library or the command-line tool to try your own expressions.

### Examples - Basic
* **3d6**: rolls 3 six-sided dice
* **d20+5**: rolls 1 20-sided die and adds 5 to result
* **1d20-2**: rolls 1 20-sided die and subtracts 2 from result
* **2d12x10**: rolls 2 12-sided dice and multiplies result by 10
* **4d8/2**: rolls 4 eight-sided dice and divides result by 2
* **d%+5**: rolls percentile (or d100) dice and adds 5 to result
* **3d+5**: rolls 3 default(6-sided) dice and adds 5 to result
* **4d6k3 + d8 + 5**: rolls 4 six-sided dice and keeps the highest 3 results; then rolls 1 eight-sided die; then adds both results together and adds another 5 modifier 

### Examples - Math order
* **5+d6**: rolls a six-sided die and adds 5 to result
* **20-d4**: rolls a four-sided die and subtracts the result from 20
* **2x4d6**: rolls 4 six-sided dice and multiplies the result by 2
* **100/2d8**: rolls 2 eight-sided dice and divides 100 by that result
* **4d6 x d10**: rolls 4 six-sided dice and 1 ten-sided die, then multiplies the results together

### Examples - Grouping
* **(1+2)d6 + 1**: adds group to 3, then rolls 3 six-sided dice and adds 1 to result
* **1d(3+7)**: adds 3+7 => 10, then uses that to roll 1 ten-sided die
* **(2d10 + 2)x10**: rolls 2 ten-sided dice and adds 2; that value is then multiplied by 10
* **(((1+2)d6) - (4-3) + d8) + 3**: try it out and see what happens... :)

### Examples - Keep/Drop Dice
* **4d6k3**: rolls 4 six-sided dice and keeps the 3 highest results
* **6d6p2**: rolls 6 six-sided dice and drops the 2 lowest results
* **4d6l1**: rolls 4 six-sided dice and keeps the 1 lowest result(s)

### Examples - Exploding Dice
* **6d6!**: rolls 6 six-sided dice and performs extra rolls for any max rolls (6) [this is known as exploding or penetration rolls]
* **4d8!7**: rolls 4 eight-sided dice and performs extra rolls for any results of 7 or greater.

### Examples - Fudge/FATE Dice
* **3f**: rolls 3 fudge dice (allowed values: -1, 0, 1)
* **3f+1**: rolls 3 fudge dice and adds modifier of 1
* **6fk4**: rolls 6 fudge dice but only keeps 4 highest in result
* **4fp1**: rolls 4 fudge dice and drops the lowest one
