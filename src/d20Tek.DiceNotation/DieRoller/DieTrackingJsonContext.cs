using System.Text.Json.Serialization;

namespace d20Tek.DiceNotation.DieRoller;

/// <summary>
/// Provides a source-generated JSON serializer context for the roll tracking data types, so that
/// tracker serialization works without reflection and is compatible with trimming and Native AOT.
/// </summary>
[JsonSerializable(typeof(List<DieTrackingData>))]
[JsonSerializable(typeof(List<AggregateDieTrackingData>))]
internal sealed partial class DieTrackingJsonContext : JsonSerializerContext
{
}
