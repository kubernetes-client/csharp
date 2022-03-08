using k8s.Models;
using System.Globalization;
using System.IO;
using System.Xml;

namespace k8s
{
    internal static class KubernetesJson
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
            private const string SerializeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.ffffffK";
            private const string Iso8601Format = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";

            public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var str = reader.GetString();
                return DateTimeOffset.ParseExact(str, new[] { Iso8601Format, SerializeFormat }, CultureInfo.InvariantCulture);
            }

            public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString(SerializeFormat));
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

        public static TValue Deserialize<TValue>(string json)
        {
            return JsonSerializer.Deserialize<TValue>(json, JsonSerializerOptions);
        }

        public static TValue Deserialize<TValue>(Stream json)
        {
            return JsonSerializer.Deserialize<TValue>(json, JsonSerializerOptions);
        }


        public static string Serialize(object value)
        {
            return JsonSerializer.Serialize(value, JsonSerializerOptions);
        }
    }
}
