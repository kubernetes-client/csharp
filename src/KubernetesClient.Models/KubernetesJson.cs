using k8s.Models;
using System.Globalization;
using System.IO;
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
            private const string RFC3339NanoFormat = "yyyy-MM-dd'T'HH':'mm':'ss.fffffffK";

            private static string FormatDateTimeOffsetToSevenDigitsNanoseconds(string dateTime)
            {
                var isUtcWithZ = dateTime.EndsWith("Z");
                var isUtcWithZeroes = Regex.IsMatch(dateTime, "^*[+-]\\d{2}\\:\\d{2}$");

                var cleanedDateTime = isUtcWithZ
                                          ? dateTime.Substring(0, dateTime.Length - 1)
                                          : isUtcWithZeroes
                                                ? dateTime.Substring(0, dateTime.Length - 6)
                                                : dateTime;

                var nanoSecondsDelimiterIndex = cleanedDateTime.LastIndexOf(".", StringComparison.Ordinal);
                var withoutNanoseconds = nanoSecondsDelimiterIndex > -1
                                                   ? cleanedDateTime.Substring(0, nanoSecondsDelimiterIndex)
                                                   : cleanedDateTime;

                var sevenDigitNanoseconds = "0000000";
                if (nanoSecondsDelimiterIndex > -1)
                {
                    var nanoSecondsAsString = cleanedDateTime.Substring(nanoSecondsDelimiterIndex + 1);

                    if (nanoSecondsAsString.Length > 9)
                    {
                        throw new ArgumentException("Invalid format for nanoseconds, too many digits.");
                    }

                    var leadingZeroes = nanoSecondsAsString.TakeWhile(c => c == '0').Count();
                    var nanoSecondsWithoutLeadingZeroesAsString = nanoSecondsAsString.Substring(leadingZeroes);
                    sevenDigitNanoseconds = nanoSecondsAsString.Length > 7
                                                ? nanoSecondsAsString.Substring(0, 7)
                                                : new string('0', leadingZeroes)
                                                    + (string.IsNullOrEmpty(nanoSecondsWithoutLeadingZeroesAsString)
                                                         ? new string('0', 7 - leadingZeroes)
                                                         : int.Parse(nanoSecondsWithoutLeadingZeroesAsString)
                                                             * (int)Math.Pow(10, 7 - leadingZeroes - nanoSecondsWithoutLeadingZeroesAsString.Length));
                }

                return withoutNanoseconds + "." + sevenDigitNanoseconds + (isUtcWithZ ? "Z" : "") + (isUtcWithZeroes ? dateTime.Substring(dateTime.Length - 6) : "");
            }

            public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var str = reader.GetString();
                return DateTimeOffset.ParseExact(FormatDateTimeOffsetToSevenDigitsNanoseconds(str), new[] { RFC3339NanoFormat }, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString(RFC3339NanoFormat));
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
