using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using k8s.Models;

namespace k8s
{
    /// <summary>
    /// This is a utility class that helps you load objects from YAML files.
    /// </summary>
    public static class Yaml
    {
        public class ByteArrayStringYamlConverter : IYamlTypeConverter
        {
            public bool Accepts(Type type)
            {
                return type == typeof(byte[]);
            }

            public object ReadYaml(IParser parser, Type type)
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

            public void WriteYaml(IEmitter emitter, object value, Type type)
            {
                var obj = (byte[])value;
                emitter?.Emit(new Scalar(Encoding.UTF8.GetString(obj)));
            }
        }

        /// <summary>
        /// Load a collection of objects from a stream asynchronously
        /// </summary>
        /// <param name="stream">
        /// The stream to load the objects from.
        /// </param>
        /// <param name="typeMap">
        /// A map from apiVersion/kind to Type. For example "v1/Pod" -> typeof(V1Pod)
        /// </param>
        /// <returns>collection of objects</returns>
        public static async Task<List<object>> LoadAllFromStreamAsync(Stream stream, Dictionary<string, Type> typeMap)
        {
            var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync().ConfigureAwait(false);
            return LoadAllFromString(content, typeMap);
        }


        /// <summary>
        /// Load a collection of objects from a file asynchronously
        /// </summary>
        /// <param name="fileName">The name of the file to load from.</param>
        /// <param name="typeMap">A map from apiVersion/kind to Type. For example "v1/Pod" -> typeof(V1Pod)</param>
        /// <returns>collection of objects</returns>
        public static Task<List<object>> LoadAllFromFileAsync(string fileName, Dictionary<string, Type> typeMap)
        {
            var reader = File.OpenRead(fileName);
            return LoadAllFromStreamAsync(reader, typeMap);
        }

        /// <summary>
        /// Load a collection of objects from a string
        /// </summary>
        /// <param name="content">
        /// The string to load the objects from.
        /// </param>
        /// <param name="typeMap">
        /// A map from apiVersion/kind to Type. For example "v1/Pod" -> typeof(V1Pod)
        /// </param>
        /// <returns>collection of objects</returns>
        public static List<object> LoadAllFromString(string content, Dictionary<string, Type> typeMap)
        {
            if (typeMap == null)
            {
                throw new ArgumentNullException(nameof(typeMap));
            }

            var deserializer =
                new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .WithTypeInspector(ti => new AutoRestTypeInspector(ti))
                    .WithTypeConverter(new IntOrStringYamlConverter())
                    .WithTypeConverter(new ByteArrayStringYamlConverter())
                    .IgnoreUnmatchedProperties()
                    .Build();
            var types = new List<Type>();
            var parser = new Parser(new StringReader(content));
            parser.Consume<StreamStart>();
            while (parser.Accept<DocumentStart>(out _))
            {
                var obj = deserializer.Deserialize<KubernetesObject>(parser);
                types.Add(typeMap[obj.ApiVersion + "/" + obj.Kind]);
            }

            deserializer =
                new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .WithTypeInspector(ti => new AutoRestTypeInspector(ti))
                    .WithTypeConverter(new IntOrStringYamlConverter())
                    .WithTypeConverter(new ByteArrayStringYamlConverter())
                    .Build();
            parser = new Parser(new StringReader(content));
            parser.Consume<StreamStart>();
            var ix = 0;
            var results = new List<object>();
            while (parser.Accept<DocumentStart>(out _))
            {
                var objType = types[ix++];
                var obj = deserializer.Deserialize(parser, objType);
                results.Add(obj);
            }

            return results;
        }

        public static async Task<T> LoadFromStreamAsync<T>(Stream stream)
        {
            var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync().ConfigureAwait(false);
            return LoadFromString<T>(content);
        }

        public static async Task<T> LoadFromFileAsync<T>(string file)
        {
            using (var fs = File.OpenRead(file))
            {
                return await LoadFromStreamAsync<T>(fs).ConfigureAwait(false);
            }
        }

        public static T LoadFromString<T>(string content)
        {
            var deserializer =
                new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .WithTypeInspector(ti => new AutoRestTypeInspector(ti))
                    .WithTypeConverter(new IntOrStringYamlConverter())
                    .WithTypeConverter(new ByteArrayStringYamlConverter())
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
                    .DisableAliases()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .WithTypeInspector(ti => new AutoRestTypeInspector(ti))
                    .WithTypeConverter(new IntOrStringYamlConverter())
                    .WithTypeConverter(new ByteArrayStringYamlConverter())
                    .WithEventEmitter(e => new StringQuotingEmitter(e))
                    .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                    .BuildValueSerializer();
            emitter.Emit(new StreamStart());
            emitter.Emit(new DocumentStart());
            serializer.SerializeValue(emitter, value, typeof(T));

            return stringBuilder.ToString();
        }

        private class AutoRestTypeInspector : ITypeInspector
        {
            private readonly ITypeInspector _inner;

            public AutoRestTypeInspector(ITypeInspector inner)
            {
                _inner = inner;
            }

            public IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
            {
                var pds = _inner.GetProperties(type, container);
                return pds.Select(pd => TrimPropertySuffix(pd, type)).ToList();
            }

            public IPropertyDescriptor GetProperty(Type type, object container, string name, bool ignoreUnmatched)
            {
                try
                {
                    return _inner.GetProperty(type, container, name, ignoreUnmatched);
                }
                catch (System.Runtime.Serialization.SerializationException)
                {
                    return _inner.GetProperty(type, container, name + "Property", ignoreUnmatched);
                }
            }

            private IPropertyDescriptor TrimPropertySuffix(IPropertyDescriptor pd, Type type)
            {
                if (!pd.Name.EndsWith("Property", StringComparison.InvariantCulture))
                {
                    return pd;
                }

                // This might have been renamed by AutoRest.  See if there is a
                // JsonPropertyAttribute.PropertyName and use that instead if there is.
                var jpa = pd.GetCustomAttribute<JsonPropertyAttribute>();
                if (jpa == null || string.IsNullOrEmpty(jpa.PropertyName))
                {
                    return pd;
                }

                return new RenamedPropertyDescriptor(pd, jpa.PropertyName);
            }

            private class RenamedPropertyDescriptor : IPropertyDescriptor
            {
                private readonly IPropertyDescriptor _inner;
                private readonly string _name;

                public RenamedPropertyDescriptor(IPropertyDescriptor inner, string name)
                {
                    _inner = inner;
                    _name = name;
                }

                public string Name => _name;

                public bool CanWrite => _inner.CanWrite;

                public Type Type => _inner.Type;

                public Type TypeOverride
                {
                    get => _inner.TypeOverride;
                    set => _inner.TypeOverride = value;
                }

                public int Order
                {
                    get => _inner.Order;
                    set => _inner.Order = value;
                }

                public ScalarStyle ScalarStyle
                {
                    get => _inner.ScalarStyle;
                    set => _inner.ScalarStyle = value;
                }

                public T GetCustomAttribute<T>()
                    where T : Attribute
                {
                    return _inner.GetCustomAttribute<T>();
                }

                public IObjectDescriptor Read(object target)
                {
                    return _inner.Read(target);
                }

                public void Write(object target, object value)
                {
                    _inner.Write(target, value);
                }
            }
        }
    }
}
