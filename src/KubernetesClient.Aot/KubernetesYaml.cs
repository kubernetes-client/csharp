using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace k8s
{
    /// <summary>
    /// This is a utility class that helps you load objects from YAML files.
    /// </summary>
    internal static class KubernetesYaml
    {
        private static StaticDeserializerBuilder CommonDeserializerBuilder =>
            new StaticDeserializerBuilder(new k8s.KubeConfigModels.StaticContext())
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new IntOrStringYamlConverter())
                .WithTypeConverter(new ByteArrayStringYamlConverter())
                .WithTypeConverter(new ResourceQuantityYamlConverter())
                .WithAttemptingUnquotedStringTypeDeserialization()
                ;

        private static readonly IDeserializer Deserializer =
            CommonDeserializerBuilder
            .IgnoreUnmatchedProperties()
            .Build();
        private static IDeserializer GetDeserializer(bool strict) => Deserializer;

        private static readonly IValueSerializer Serializer =
            new StaticSerializerBuilder(new k8s.KubeConfigModels.StaticContext())
                .DisableAliases()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new IntOrStringYamlConverter())
                .WithTypeConverter(new ByteArrayStringYamlConverter())
                .WithTypeConverter(new ResourceQuantityYamlConverter())
                .WithEventEmitter(e => new StringQuotingEmitter(e))
                .WithEventEmitter(e => new FloatEmitter(e))
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .BuildValueSerializer();

        private class ByteArrayStringYamlConverter : IYamlTypeConverter
        {
            public bool Accepts(Type type)
            {
                return type == typeof(byte[]);
            }

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

                        return Encoding.UTF8.GetBytes(scalar.Value);
                    }
                    finally
                    {
                        parser.MoveNext();
                    }
                }

                throw new InvalidOperationException(parser.Current?.ToString());
            }

            public void WriteYaml(IEmitter emitter, object value, Type type, ObjectSerializer serializer)
            {
                var obj = (byte[])value;
                emitter?.Emit(new Scalar(Encoding.UTF8.GetString(obj)));
            }
        }

        public static async Task<T> LoadFromStreamAsync<T>(Stream stream, bool strict = false)
        {
            var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync().ConfigureAwait(false);
            return Deserialize<T>(content, strict);
        }

        public static async Task<T> LoadFromFileAsync<T>(string file, bool strict = false)
        {
            using (var fs = File.OpenRead(file))
            {
                return await LoadFromStreamAsync<T>(fs, strict).ConfigureAwait(false);
            }
        }

        [Obsolete("use Deserialize")]
        public static T LoadFromString<T>(string content, bool strict = false)
        {
            return Deserialize<T>(content, strict);
        }

        [Obsolete("use Serialize")]
        public static string SaveToString<T>(T value)
        {
            return Serialize(value);
        }

        public static TValue Deserialize<TValue>(string yaml, bool strict = false)
        {
            using var reader = new StringReader(yaml);
            return GetDeserializer(strict).Deserialize<TValue>(new MergingParser(new Parser(reader)));
        }

        public static TValue Deserialize<TValue>(Stream yaml, bool strict = false)
        {
            using var reader = new StreamReader(yaml);
            return GetDeserializer(strict).Deserialize<TValue>(new MergingParser(new Parser(reader)));
        }

        public static string SerializeAll(IEnumerable<object> values)
        {
            if (values == null)
            {
                return "";
            }

            var stringBuilder = new StringBuilder();
            var writer = new StringWriter(stringBuilder);
            var emitter = new Emitter(writer);

            emitter.Emit(new StreamStart());

            foreach (var value in values)
            {
                if (value != null)
                {
                    emitter.Emit(new DocumentStart());
                    Serializer.SerializeValue(emitter, value, value.GetType());
                    emitter.Emit(new DocumentEnd(true));
                }
            }

            return stringBuilder.ToString();
        }

        public static string Serialize(object value)
        {
            if (value == null)
            {
                return "";
            }

            var stringBuilder = new StringBuilder();
            var writer = new StringWriter(stringBuilder);
            var emitter = new Emitter(writer);

            emitter.Emit(new StreamStart());
            emitter.Emit(new DocumentStart());
            Serializer.SerializeValue(emitter, value, value.GetType());

            return stringBuilder.ToString();
        }
    }
}
