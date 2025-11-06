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
        var expr = Parse(Precedence.None);
        Expect(TokenKind.EndOfInput);
        return expr;
    }

    private Expression Parse(int rbp)
    {
        var t = Advance();
        var left = Nud(t);
        while (rbp < Precedence.Get(PeekKind()))
        {
            var op = Advance();
            left = Led(op, left);
        }
        return left;
    }

    // ---------- Nud (prefix / atom) ----------
    private Expression Nud(Token t) => t.Kind switch
    {
        TokenKind.Number => new NumberExpression(t.IntValue!.Value, t.Pos),
        TokenKind.GroupStart => NudGroup(t),
        TokenKind.Plus => new UnaryExpression(UnaryOperator.Positive, Parse(Precedence.Unary), t.Pos),
        TokenKind.Minus => new UnaryExpression(UnaryOperator.Negative, Parse(Precedence.Unary), t.Pos),
        TokenKind.Dice => NudDicePrefix(t),             // Prefix 'D' for omitted count: d% | d(arg)
        TokenKind.FudgeDice => NudFudgePrefix(t),       // Prefix 'F' for omitted count: f
        _ => throw Error($"Unexpected token {t.Kind} in prefix position.")
    };

    private Expression NudGroup(Token lp)
    {
        var inner = Parse(Precedence.None);
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

        return new DiceExpression(null, percent, sidesArg, ParseModifiers(), dTok.Pos);
    }

    private Expression NudFudgePrefix(Token dTok) => new FudgeExpression(null, ParseModifiers(), dTok.Pos);

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

    private Expression LedBinary(Expression left, BinaryOperator op, Position pos, int bp) =>
        new BinaryExpression(left, op, Parse(bp), pos);

    private Expression LedDice(Expression left, Token dTok)
    {
        // Enforce Arg Parentheses for COUNT on the left:
        ParseException.ThrowIfFalse(IsArg(left), "Dice count must be a Number or parenthesized expression.", dTok.Pos);

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

        return new DiceExpression(left, percent, sidesArg, ParseModifiers(), dTok.Pos);
    }

    private Expression LedFudge(Expression left, Token fTok)
    {
        ParseException.ThrowIfFalse(IsArg(left), "Fudge count must be a Number or parenthesized expression.", fTok.Pos);
        return new FudgeExpression(left, ParseModifiers(), fTok.Pos);
    }

    // ---------- Modifiers (suffix loop bound to a dice node only) ----------
    private ImmutableList<Modifier> ParseModifiers()
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
            var t = Consume();
            return new NumberExpression(t.IntValue!.Value, t.Pos);
        }
        if (Match(TokenKind.GroupStart))
        {
            var lp = Consume();
            var inner = Parse(Precedence.None);
            Expect(TokenKind.GroupEnd);
            Consume();
            return new GroupExpression(inner, lp.Pos);
        }
        throw Error("Expected argument: Number or parenthesized expression.");
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
        if (_curr.Kind != k) throw Error($"Expected token of kind {k}, found {_curr.Kind}.");
    }

    private Token Consume() => Advance();

    private ParseException Error(string msg) => new(msg, _curr.Pos);
}