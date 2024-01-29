using static k8s.KubernetesJson;

namespace k8s;

[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    Converters = new[] { typeof(Iso8601TimeSpanConverter), typeof(KubernetesDateTimeConverter), typeof(KubernetesDateTimeOffsetConverter) })
    ]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
