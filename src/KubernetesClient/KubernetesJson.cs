using System.Globalization;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml;

#if NET8_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
#endif

namespace k8s
{
    public static class KubernetesJson
    {
        internal static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions();

        public sealed class Iso8601TimeSpanConverter : JsonConverter<TimeSpan>
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

        public sealed class KubernetesDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
        {
            private const string RFC3339MicroFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.ffffffZ";
            private const string RFC3339NanoFormat = "yyyy-MM-dd'T'HH':'mm':'ss.fffffffZ";
            private const string RFC3339Format = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";

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
                // Output as RFC3339Micro
                var date = value.ToUniversalTime();

                var basePart = date.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
                var frac = date.ToString(".ffffff", CultureInfo.InvariantCulture)
                    .TrimEnd('0')
                    .TrimEnd('.');

                writer.WriteStringValue(basePart + frac + "Z");
            }
        }

        public sealed class KubernetesDateTimeConverter : JsonConverter<DateTime>
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
#if K8S_AOT
            // Uses Source Generated IJsonTypeInfoResolver
            JsonSerializerOptions.TypeInfoResolver = SourceGenerationContext.Default;
#else
#if NET8_0_OR_GREATER
            // Uses Source Generated IJsonTypeInfoResolver when available and falls back to reflection
            JsonSerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(SourceGenerationContext.Default, new DefaultJsonTypeInfoResolver());
#endif
            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
#endif
            JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            JsonSerializerOptions.Converters.Add(new Iso8601TimeSpanConverter());
            JsonSerializerOptions.Converters.Add(new KubernetesDateTimeConverter());
            JsonSerializerOptions.Converters.Add(new KubernetesDateTimeOffsetConverter());
            JsonSerializerOptions.Converters.Add(new V1Status.V1StatusObjectViewConverter());
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
#if NET8_0_OR_GREATER
            var info = (JsonTypeInfo<TValue>)(jsonSerializerOptions ?? JsonSerializerOptions).GetTypeInfo(typeof(TValue));
            return JsonSerializer.Deserialize(json, info);
#else
            return JsonSerializer.Deserialize<TValue>(json, jsonSerializerOptions ?? JsonSerializerOptions);
#endif
        }

        public static TValue Deserialize<TValue>(Stream json, JsonSerializerOptions jsonSerializerOptions = null)
        {
#if NET8_0_OR_GREATER
            var info = (JsonTypeInfo<TValue>)(jsonSerializerOptions ?? JsonSerializerOptions).GetTypeInfo(typeof(TValue));
            return JsonSerializer.Deserialize(json, info);
#else
            return JsonSerializer.Deserialize<TValue>(json, jsonSerializerOptions ?? JsonSerializerOptions);
#endif
        }

        public static TValue Deserialize<TValue>(JsonDocument json, JsonSerializerOptions jsonSerializerOptions = null)
        {
#if NET8_0_OR_GREATER
            var info = (JsonTypeInfo<TValue>)(jsonSerializerOptions ?? JsonSerializerOptions).GetTypeInfo(typeof(TValue));
            return JsonSerializer.Deserialize(json, info);
#else
            return JsonSerializer.Deserialize<TValue>(json, jsonSerializerOptions ?? JsonSerializerOptions);
#endif
        }

        public static TValue Deserialize<TValue>(JsonElement json, JsonSerializerOptions jsonSerializerOptions = null)
        {
#if NET8_0_OR_GREATER
            var info = (JsonTypeInfo<TValue>)(jsonSerializerOptions ?? JsonSerializerOptions).GetTypeInfo(typeof(TValue));
            return JsonSerializer.Deserialize(json, info);
#else
            return JsonSerializer.Deserialize<TValue>(json, jsonSerializerOptions ?? JsonSerializerOptions);
#endif
        }

        public static TValue Deserialize<TValue>(JsonNode json, JsonSerializerOptions jsonSerializerOptions = null)
        {
#if NET8_0_OR_GREATER
            var info = (JsonTypeInfo<TValue>)(jsonSerializerOptions ?? JsonSerializerOptions).GetTypeInfo(typeof(TValue));
            return JsonSerializer.Deserialize(json, info);
#else
            return JsonSerializer.Deserialize<TValue>(json, jsonSerializerOptions ?? JsonSerializerOptions);
#endif
        }

        public static string Serialize(object value, JsonSerializerOptions jsonSerializerOptions = null)
        {
#if NET8_0_OR_GREATER
            var info = (jsonSerializerOptions ?? JsonSerializerOptions).GetTypeInfo(value.GetType());
            return JsonSerializer.Serialize(value, info);
#else
            return JsonSerializer.Serialize(value, jsonSerializerOptions ?? JsonSerializerOptions);
#endif
        }

        public static string Serialize(JsonDocument value, JsonSerializerOptions jsonSerializerOptions = null)
        {
#if NET8_0_OR_GREATER
            var info = (jsonSerializerOptions ?? JsonSerializerOptions).GetTypeInfo(value.GetType());
            return JsonSerializer.Serialize(value, info);
#else
            return JsonSerializer.Serialize(value, jsonSerializerOptions ?? JsonSerializerOptions);
#endif
        }

        public static string Serialize(JsonElement value, JsonSerializerOptions jsonSerializerOptions = null)
        {
#if NET8_0_OR_GREATER
            var info = (jsonSerializerOptions ?? JsonSerializerOptions).GetTypeInfo(value.GetType());
            return JsonSerializer.Serialize(value, info);
#else
            return JsonSerializer.Serialize(value, jsonSerializerOptions ?? JsonSerializerOptions);
#endif
        }

        public static string Serialize(JsonNode value, JsonSerializerOptions jsonSerializerOptions = null)
        {
#if NET8_0_OR_GREATER
            var info = (jsonSerializerOptions ?? JsonSerializerOptions).GetTypeInfo(value.GetType());
            return JsonSerializer.Serialize(value, info);
#else
            return JsonSerializer.Serialize(value, jsonSerializerOptions ?? JsonSerializerOptions);
#endif
        }
    }
}
