using System.Globalization;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace k8s.Models;

public sealed class KubernetesDateTimeOffsetYamlConverter : IYamlTypeConverter
{
    private const string RFC3339MicroFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.ffffff'Z'";
    private const string RFC3339NanoFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffff'Z'";
    private const string RFC3339Format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";

    public bool Accepts(Type type) => type == typeof(DateTimeOffset);

    public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        if (parser?.Current is Scalar scalar)
        {
            try
            {
                if (string.IsNullOrEmpty(scalar.Value))
                {
                    return null;
                }

                var str = scalar.Value;

                if (DateTimeOffset.TryParseExact(str, new[] { RFC3339Format, RFC3339MicroFormat }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                {
                    return result;
                }

                // try RFC3339NanoLenient by trimming 1-9 digits to 7 digits
                var originalstr = str;
                str = Regex.Replace(str, @"\.\d+", m => (m.Value + "000000000").Substring(0, 7 + 1)); // 7 digits + 1 for the dot
                if (DateTimeOffset.TryParseExact(str, new[] { RFC3339NanoFormat }, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    return result;
                }
            }
            finally
            {
                parser.MoveNext();
            }
        }

        throw new InvalidOperationException($"Unable to parse '{parser.Current?.ToString()}' as RFC3339, RFC3339Micro, or RFC3339Nano");
    }

    public void WriteYaml(IEmitter emitter, object value, Type type, ObjectSerializer serializer)
    {
        // Output as RFC3339Nano
        var date = ((DateTimeOffset)value).ToUniversalTime();

        var basePart = date.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
        var frac = date.ToString(".fffffff", CultureInfo.InvariantCulture)
            .TrimEnd('0')
            .TrimEnd('.');

        emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, basePart + frac + "Z", ScalarStyle.DoubleQuoted, true, false));
    }
}
