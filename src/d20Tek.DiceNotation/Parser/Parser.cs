using System.Collections.Immutable;

namespace d20Tek.DiceNotation.Parser;

internal sealed class Parser
{
    private readonly Lexer _lex;
    private Token _curr;

    public Parser(Lexer lexer)
    {
        _lex = lexer;
        _curr = _lex.GetNextToken();
    }

    public Expression ParseExpression()
    {
        var expr = Parse(0);
        Expect(TokenKind.EndOfInput);
        return expr;
    }

    private Expression Parse(int rbp)
    {
        var t = Advance();
        var left = Nud(t);
        while (rbp < Lbp(PeekKind()))
        {
            var op = Advance();
            left = Led(op, left);
        }
        return left;
    }

    // ---------- Binding powers ----------
    private static int Lbp(TokenKind k) => k switch
    {
        // dice and fudge bind tighter than multiplication
        TokenKind.FudgeDice or TokenKind.Dice => 40,
        TokenKind.Star or TokenKind.Times or TokenKind.Divide => 20,
        TokenKind.Plus or TokenKind.Minus => 10,
        _ => 0
    };

    // ---------- Nud (prefix / atom) ----------
    private Expression Nud(Token t) => t.Kind switch
    {
        TokenKind.Number => new NumberExpression(t.IntValue!.Value, t.Pos),
        TokenKind.GroupStart => NudGroup(t),
        TokenKind.Plus => new UnaryExpression(UnaryOperator.Positive, Parse(35), t.Pos),
        TokenKind.Minus => new UnaryExpression(UnaryOperator.Negative, Parse(35), t.Pos),
        TokenKind.Dice => NudDicePrefix(t),             // Prefix 'D' for omitted count: d% | d(arg)
        TokenKind.FudgeDice => NudFudgePrefix(t),       // Prefix 'F' for omitted count: f
        _ => throw Error($"Unexpected token {t.Kind} in prefix position.")
    };

    private Expression NudGroup(Token lp)
    {
        var inner = Parse(0);
        Expect(TokenKind.GroupEnd);
        Consume();
        return new GroupExpression(inner, lp.Pos);
    }

    private Expression NudDicePrefix(Token dTok)
    {
        // sides: '%' | arg
        bool percent;
        Expression? sidesArg = null;
        if (Match(TokenKind.Percent))
        {
            Consume();
            percent = true;
        }
        else
        {
            sidesArg = ParseArg();
            percent = false;
        }

        var dice = new DiceExpression(null, percent, sidesArg, [], dTok.Pos);
        return AttachModifiers(dice);
    }

    private Expression NudFudgePrefix(Token dTok)
    {
        var fudge = new FudgeExpression(null, [], dTok.Pos);
        return AttachModifiers(fudge);
    }

    // ---------- Led (infix / postfix) ----------
    private Expression Led(Token op, Expression left) => op.Kind switch
    {
        TokenKind.Plus => LedBinary(left, BinaryOperator.Add, op.Pos, 10),
        TokenKind.Minus => LedBinary(left, BinaryOperator.Subtract, op.Pos, 10),
        TokenKind.Star or TokenKind.Times => LedBinary(left, BinaryOperator.Multiply, op.Pos, 20),
        TokenKind.Divide => LedBinary(left, BinaryOperator.Divide, op.Pos, 20),

        // Infix dice: left (must be arg) 'D' sides
        TokenKind.Dice => LedDice(left, op),

        // Postfix fudge: left (must be arg) 'F'
        TokenKind.FudgeDice => LedFudge(left, op),

        _ => throw Error($"Unexpected token {op.Kind} in infix/postfix position.")
    };

    private Expression LedBinary(Expression left, BinaryOperator op, Position pos, int bp)
    {
        var right = Parse(bp);
        return new BinaryExpression(left, op, right, pos);
    }

    private Expression LedDice(Expression left, Token dTok)
    {
        // Enforce Arg Parentheses for COUNT on the left:
        if (!IsArg(left)) throw Error("Dice count must be a Number or a parenthesized expression.");

        // sides: '%' | arg
        bool percent;
        Expression? sidesArg = null;
        if (Match(TokenKind.Percent)) { Consume(); percent = true; }
        else { sidesArg = ParseArg(); percent = false; }

        var dice = new DiceExpression(left, percent, sidesArg, [], dTok.Pos);
        return AttachModifiers(dice);
    }

    private Expression LedFudge(Expression left, Token fTok)
    {
        if (!IsArg(left)) throw Error("Fudge count must be a Number or a parenthesized expression.");

        var fudge = new FudgeExpression(left, [], fTok.Pos);
        return AttachModifiers(fudge);
    }

    // ---------- Modifiers (suffix loop bound to a dice node only) ----------

    private Expression AttachModifiers(DiceExpression dice)
    {
        var mods = new List<Modifier>();
        while (true)
        {
            if (Match(TokenKind.Exploding))
            {
                var bang = Consume();
                Expression? th = null;
                if (Match(TokenKind.Number) || Match(TokenKind.GroupStart))
                    th = ParseArg();
                mods.Add(new ExplodingModifier(th, bang.Pos));
            }
            else if (Match(TokenKind.Keep) || Match(TokenKind.Drop) || Match(TokenKind.KeepLowest))
            {
                var op = Consume();
                var arg = ParseArg();
                var kind = op.Kind switch
                {
                    TokenKind.Keep => SelectKind.KeepHigh,
                    TokenKind.Drop => SelectKind.DropLow,
                    TokenKind.KeepLowest => SelectKind.KeepLow,
                    _ => throw Error("Invalid selection modifier")
                };
                mods.Add(new SelectModifier(kind, arg, op.Pos));
            }
            else break;
        }
        return dice with { Modifiers = mods.ToImmutableList() };
    }

    private Expression AttachModifiers(FudgeExpression fudge)
    {
        var mods = new List<Modifier>();
        while (true)
        {
            if (Match(TokenKind.Exploding))
            {
                var bang = Consume();
                Expression? th = null;
                if (Match(TokenKind.Number) || Match(TokenKind.GroupStart))
                    th = ParseArg();
                mods.Add(new ExplodingModifier(th, bang.Pos));
            }
            else if (Match(TokenKind.Keep) || Match(TokenKind.Drop) || Match(TokenKind.KeepLowest))
            {
                var op = Consume();
                var arg = ParseArg();
                var kind = op.Kind switch
                {
                    TokenKind.Keep => SelectKind.KeepHigh,
                    TokenKind.Drop => SelectKind.DropLow,
                    TokenKind.KeepLowest => SelectKind.KeepLow,
                    _ => throw Error("Invalid selection modifier")
                };
                mods.Add(new SelectModifier(kind, arg, op.Pos));
            }
            else break;
        }
        return fudge with { Modifiers = mods.ToImmutableList() };
    }

    // ---------- Arg (INT or '(' expression ')') ----------
    private Expression ParseArg()
    {
        if (Match(TokenKind.Number))
        {
            var t = Consume();
            return new NumberExpression(t.IntValue!.Value, t.Pos);
        }
        if (Match(TokenKind.GroupStart))
        {
            var lp = Consume();
            var inner = Parse(0);
            Expect(TokenKind.GroupEnd);
            Consume();
            return new GroupExpression(inner, lp.Pos);
        }
        throw Error("Expected argument: INT or parenthesized expression");
    }

    private static bool IsArg(Expression e) => e is NumberExpression || e is GroupExpression;

    // ---------- Token utilities ----------
    private TokenKind PeekKind() => _curr.Kind;

    private bool Match(TokenKind k) => _curr.Kind == k;

    private Token Advance()
    {
        var t = _curr;
        _curr = _lex.GetNextToken();
        return t;
    }

    private void Expect(TokenKind k)
    {
        if (_curr.Kind != k) throw Error($"Expected {k}, found {_curr.Kind}");
    }

    private Token Consume() => Advance();

    private ParseException Error(string msg) => new(msg, _curr.Pos);
}