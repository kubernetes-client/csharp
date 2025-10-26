using static k8s.KubernetesJson;
using static k8s.Models.V1Status;

namespace k8s;

[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true,
    Converters = new[] { typeof(Iso8601TimeSpanConverter), typeof(KubernetesDateTimeConverter), typeof(KubernetesDateTimeOffsetConverter), typeof(V1StatusObjectViewConverter) })
    ]
public partial class SourceGenerationContext : JsonSerializerContext
{
}

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
