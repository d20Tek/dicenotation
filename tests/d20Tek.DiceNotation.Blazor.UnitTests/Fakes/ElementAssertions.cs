namespace d20Tek.DiceNotation.Blazor.UnitTests.Fakes;

using AngleSharp.Dom;

/// <summary>
/// Element helpers for component tests. Attribute reads are non-nullable here because a
/// missing attribute is itself a test failure rather than an expected value.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ElementAssertions
{
    /// <summary>Gets an attribute value, failing the test when the attribute is absent.</summary>
    public static string AttributeValue(this IElement element, string name) =>
        element.GetAttribute(name) ??
            throw new AssertFailedException($"The element has no '{name}' attribute.");
}
