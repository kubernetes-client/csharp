using static k8s.KubernetesJson;

namespace k8s;

/// <summary>
/// Used by V1Status in order to avoid the recursive loop as SourceGenerationContext contains V1StatusObjectViewConverter
/// </summary>
[JsonSerializable(typeof(V1Status))]
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true,
    Converters = new[] { typeof(Iso8601TimeSpanConverter), typeof(KubernetesDateTimeConverter), typeof(KubernetesDateTimeOffsetConverter) })
    ]
public partial class StatusSourceGenerationContext : JsonSerializerContext
{
}
