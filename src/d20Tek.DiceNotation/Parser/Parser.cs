using System.Collections.Immutable;

namespace d20Tek.DiceNotation.Parser;

internal sealed class Parser(Lexer lexer)
{
    private readonly Lexer _lex = lexer;
    private Token _curr = lexer.GetNextToken();

    public Expression ParseExpression()
    {
        var expression = Parse(Precedence.None);
        _curr.IsExpectedKind(TokenKind.EndOfInput);
        return expression;
    }

    private Expression Parse(int rightPrecedence)
    {
        var t = Advance();
        var left = Nud(t);
        while (rightPrecedence < Precedence.Get(_curr.Kind))
        {
            var op = Advance();
            left = Led(op, left);
        }
        return left;
    }

    // ---------- Nud (prefix) ----------
    private Expression Nud(Token token) => token.Kind switch
    {
        TokenKind.Number => new NumberExpression(token.IntValue!.Value, token.Pos),
        TokenKind.GroupStart => NudGroup(token),
        TokenKind.Plus => new UnaryExpression(UnaryOperator.Positive, Parse(Precedence.Unary), token.Pos),
        TokenKind.Minus => new UnaryExpression(UnaryOperator.Negative, Parse(Precedence.Unary), token.Pos),
        TokenKind.Dice => NudDicePrefix(token),             // Prefix 'D' for omitted count: d% | d(arg)
        TokenKind.FudgeDice => NudFudgePrefix(token),       // Prefix 'F' for omitted count: f
        _ => throw Error($"Unexpected token {token.Kind} in prefix position.")
    };

    private GroupExpression NudGroup(Token groupStart)
    {
        var inner = Parse(Precedence.None);
        _curr.IsExpectedKind(TokenKind.GroupEnd);
        Advance();

        return new(inner, groupStart.Pos);
    }

    private DiceExpression NudDicePrefix(Token diceToken)
    {
        // sides: '%' | arg
        bool percent;
        Expression? sidesArg = null;
        if (Match(TokenKind.Percent))
        {
            Advance();
            percent = true;
        }
        else
        {
            sidesArg = ParseArg();
            percent = false;
        }

        return new(null, percent, sidesArg, ParseModifiers(), diceToken.Pos);
    }

    private FudgeExpression NudFudgePrefix(Token diceToken) => new(null, ParseModifiers(), diceToken.Pos);

    // ---------- Led (infix / postfix) ----------
    internal Expression Led(Token op, Expression left) => op.Kind switch
    {
        TokenKind.Plus => LedBinary(left, BinaryOperator.Add, op.Pos, Precedence.Get(TokenKind.Plus)),
        TokenKind.Minus => LedBinary(left, BinaryOperator.Subtract, op.Pos, Precedence.Get(TokenKind.Minus)),
        TokenKind.Star => LedBinary(left, BinaryOperator.Multiply, op.Pos, Precedence.Get(TokenKind.Star)),
        TokenKind.Times => LedBinary(left, BinaryOperator.Multiply, op.Pos, Precedence.Get(TokenKind.Times)),
        TokenKind.Divide => LedBinary(left, BinaryOperator.Divide, op.Pos, Precedence.Get(TokenKind.Divide)),
        TokenKind.Dice => LedDice(left, op),                // Infix dice: left (must be arg) 'D' sides
        TokenKind.FudgeDice => LedFudge(left, op),          // Postfix fudge: left (must be arg) 'F'
        _ => throw Error($"Unexpected token {op.Kind} in infix/postfix position.")
    };

    private BinaryExpression LedBinary(Expression left, BinaryOperator op, Position pos, int precedence) =>
        new(left, op, Parse(precedence), pos);

    private DiceExpression LedDice(Expression left, Token diceToken)
    {
        // Enforce Arg Parentheses for COUNT on the left:
        ParseException.ThrowIfFalse(IsArg(left), "Dice count must be a Number or parenthesized expression.", diceToken.Pos);

        // sides: '%' | arg
        bool percent;
        Expression? sidesArg = null;
        if (Match(TokenKind.Percent))
        {
            Advance();
            percent = true;
        }
        else
        {
            sidesArg = ParseArg();
            percent = false;
        }

        return new(left, percent, sidesArg, ParseModifiers(), diceToken.Pos);
    }

    private FudgeExpression LedFudge(Expression left, Token fudgeToken)
    {
        ParseException.ThrowIfFalse(IsArg(left), "Fudge count must be a Number or parenthesized expression.", fudgeToken.Pos);
        return new(left, ParseModifiers(), fudgeToken.Pos);
    }

    // ---------- Modifiers (suffix loop bound to a dice node only) ----------
    private ImmutableList<Modifier> ParseModifiers()
    {
        var mods = new List<Modifier>();
        while (true)
        {
            if (Match(TokenKind.Exploding))
            {
                var bang = Advance();
                Expression? th = null;
                if (Match(TokenKind.Number) || Match(TokenKind.GroupStart))
                    th = ParseArg();
                mods.Add(new ExplodingModifier(th, bang.Pos));
            }
            else if (Match(TokenKind.Keep) || Match(TokenKind.Drop) || Match(TokenKind.KeepLowest))
            {
                var op = Advance();
                var arg = ParseArg();
                var kind = SelectKindMapper.FromTokenKind(op.Kind, op.Pos);
                mods.Add(new SelectModifier(kind, arg, op.Pos));
            }
            else break;
        }

        return [.. mods];
    }

    // ---------- Arg (INT or '(' expression ')') ----------
    private Expression ParseArg()
    {
        if (Match(TokenKind.Number))
        {
            var t = Advance();
            return new NumberExpression(t.IntValue!.Value, t.Pos);
        }
        if (Match(TokenKind.GroupStart))
        {
            var lp = Advance();
            var inner = Parse(Precedence.None);
            _curr.IsExpectedKind(TokenKind.GroupEnd);
            Advance();
            return new GroupExpression(inner, lp.Pos);
        }
        throw Error("Expected argument: Number or parenthesized expression.");
    }

    private static bool IsArg(Expression e) => e is NumberExpression || e is GroupExpression;

    // ---------- Token utilities ----------
    private bool Match(TokenKind k) => _curr.Kind == k;

    private Token Advance()
    {
        var t = _curr;
        _curr = _lex.GetNextToken();
        return t;
    }

    private ParseException Error(string msg) => new(msg, _curr.Pos);
}