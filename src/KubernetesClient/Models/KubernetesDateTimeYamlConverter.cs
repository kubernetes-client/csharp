using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace k8s.Models;

public sealed class KubernetesDateTimeYamlConverter : IYamlTypeConverter
{
    private static readonly KubernetesDateTimeOffsetYamlConverter OffsetConverter = new();

    public bool Accepts(Type type) => type == typeof(DateTime);

    public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var dto = (DateTimeOffset)OffsetConverter.ReadYaml(parser, typeof(DateTimeOffset), rootDeserializer);
        return dto.DateTime;
    }

    public void WriteYaml(IEmitter emitter, object value, Type type, ObjectSerializer serializer)
    {
        var date = new DateTimeOffset((DateTime)value);
        OffsetConverter.WriteYaml(emitter, date, typeof(DateTimeOffset), serializer);
    }
}
