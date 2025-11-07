using System.Collections.Immutable;

namespace d20Tek.DiceNotation.Parser.Parselets;

internal class ModifierParser
{
    public ImmutableList<Modifier> Parse(IParser parser, ArgParser args)
    {
        var mods = new List<Modifier>();
        while (true)
        {
            if (parser.Match(TokenKind.Exploding)) mods.Add(ParseExplodingModifier(parser, args));
            else if (IsSelectKind(parser))         mods.Add(ParseSelectModifier(parser, args));
            else break;
        }

        return [.. mods];
    }

    private static bool IsSelectKind(IParser parser) => 
        parser.Match(TokenKind.Keep) || parser.Match(TokenKind.Drop) || parser.Match(TokenKind.KeepLowest);

    private static ExplodingModifier ParseExplodingModifier(IParser parser, ArgParser args)
    {
        var expToken = parser.Advance();
        var threshold = (parser.Match(TokenKind.Number) || parser.Match(TokenKind.GroupStart))
                      ? args.Parse(parser)
                      : null;

        return new ExplodingModifier(threshold, expToken.Pos);
    }

    private static SelectModifier ParseSelectModifier(IParser parser, ArgParser args)
    {
        var expToken = parser.Advance();
        var threshold = args.Parse(parser);
        var kind = SelectKindMapper.FromTokenKind(expToken.Kind, expToken.Pos);

        return new SelectModifier(kind, threshold, expToken.Pos);
    }
}
