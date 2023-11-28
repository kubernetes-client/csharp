using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace k8s
{
    public static class KubernetesJson
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions();

        private sealed class Iso8601TimeSpanConverter : JsonConverter<TimeSpan>
        {
            public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var str = reader.GetString();
                return XmlConvert.ToTimeSpan(str);
            }

            public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
            {
                var iso8601TimeSpanString = XmlConvert.ToString(value); // XmlConvert for TimeSpan uses ISO8601, so delegate serialization to it
                writer.WriteStringValue(iso8601TimeSpanString);
            }
        }

        private sealed class KubernetesDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
        {
            private const string RFC3339MicroFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.ffffffK";
            private const string RFC3339NanoFormat = "yyyy-MM-dd'T'HH':'mm':'ss.fffffffK";
            private const string RFC3339Format = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";

            public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var str = reader.GetString();

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

                throw new FormatException($"Unable to parse {originalstr} as RFC3339 RFC3339Micro or RFC3339Nano");
            }

            public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString(RFC3339MicroFormat));
            }
        }

        private sealed class KubernetesDateTimeConverter : JsonConverter<DateTime>
        {
            private static readonly JsonConverter<DateTimeOffset> UtcConverter = new KubernetesDateTimeOffsetConverter();
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return UtcConverter.Read(ref reader, typeToConvert, options).UtcDateTime;
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                UtcConverter.Write(writer, value, options);
            }
        }

        static KubernetesJson()
        {
            JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            JsonSerializerOptions.Converters.Add(new Iso8601TimeSpanConverter());
            JsonSerializerOptions.Converters.Add(new KubernetesDateTimeConverter());
            JsonSerializerOptions.Converters.Add(new KubernetesDateTimeOffsetConverter());
            JsonSerializerOptions.Converters.Add(new V1Status.V1StatusObjectViewConverter());
            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        /// <summary>
        /// Configures <see cref="JsonSerializerOptions"/> for the <see cref="JsonSerializer"/>.
        /// To override existing converters, add them to the top of the <see cref="JsonSerializerOptions.Converters"/> list
        /// e.g. as follows: <code>options.Converters.Insert(index: 0, new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));</code>
        /// </summary>
        /// <param name="configure">An <see cref="Action"/> to configure the <see cref="JsonSerializerOptions"/>.</param>
        public static void AddJsonOptions(Action<JsonSerializerOptions> configure)
        {
            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            configure(JsonSerializerOptions);
        }

        public static TValue Deserialize<TValue>(string json, JsonSerializerOptions jsonSerializerOptions = null)
        {
            return JsonSerializer.Deserialize<TValue>(json, jsonSerializerOptions ?? JsonSerializerOptions);
        }

        public static TValue Deserialize<TValue>(Stream json, JsonSerializerOptions jsonSerializerOptions = null)
        {
            return JsonSerializer.Deserialize<TValue>(json, jsonSerializerOptions ?? JsonSerializerOptions);
        }

        public static string Serialize(object value, JsonSerializerOptions jsonSerializerOptions = null)
        {
            return JsonSerializer.Serialize(value, jsonSerializerOptions ?? JsonSerializerOptions);
        }
    }
}
