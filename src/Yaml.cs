using System.IO;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace k8s
{
    /// <summary>
    /// This is a utility class that helps you load objects from YAML files.
    /// </summary>

    public class Yaml {
        public static async Task<T> LoadFromStreamAsync<T>(Stream stream) {
            var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            return LoadFromString<T>(content);
        }

        public static async Task<T> LoadFromFileAsync<T> (string file) {
            using (FileStream fs = File.OpenRead(file)) {
                return await LoadFromStreamAsync<T>(fs);
            }
        }

        public static T LoadFromString<T>(string content) {
            var deserializer =
                new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
            var obj = deserializer.Deserialize<T>(content);
            return obj;
        }

        public static string SaveToString<T>(T value)
        {
            var stringBuilder = new StringBuilder();
            var writer = new StringWriter(stringBuilder);
            var emitter = new Emitter(writer);

            var serializer =
                new SerializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .BuildValueSerializer();
            emitter.Emit(new StreamStart());
            emitter.Emit(new DocumentStart());
            serializer.SerializeValue(emitter, value, typeof(T));

            return stringBuilder.ToString();
        }
    }
}
