using System.Collections.Immutable;

namespace d20Tek.DiceNotation.Parser.Parselets;

internal class ModifierParser
{
    public ImmutableList<Modifier> Parse(IParser parser, ArgParser args)
    {
        var mods = new List<Modifier>();
        while (true)
        {
            if (parser.Match(TokenKind.Exploding))
            {
                var expToken = parser.Advance();
                var threshold = (parser.Match(TokenKind.Number) || parser.Match(TokenKind.GroupStart))
                              ? args.Parse(parser)
                              : null;

                mods.Add(new ExplodingModifier(threshold, expToken.Pos));
            }
            else if (parser.Match(TokenKind.Keep) || parser.Match(TokenKind.Drop) || parser.Match(TokenKind.KeepLowest))
            {
                var expToken = parser.Advance();
                var threshold = args.Parse(parser);
                var kind = SelectKindMapper.FromTokenKind(expToken.Kind, expToken.Pos);

                mods.Add(new SelectModifier(kind, threshold, expToken.Pos));
            }
            else break;
        }

        return [.. mods];
    }
}
