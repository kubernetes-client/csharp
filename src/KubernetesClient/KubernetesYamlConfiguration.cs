using YamlDotNet.Serialization;

namespace k8s
{
    /// <summary>
    /// This class allows overriding the <see cref="YamlDotNet"/> settings.
    /// </summary>
    public static class KubernetesYamlConfiguration
    {
        /// <summary>
        /// Configures <see cref="DeserializerBuilder"/> for <see cref="YamlDotNet"/>.
        /// </summary>
        public static event EventHandler<DeserializerBuilder> DeseralizerEvent;

        /// <summary>
        /// Configures <see cref="SerializerBuilder"/> for <see cref="YamlDotNet"/>.
        /// </summary>
        public static event EventHandler<SerializerBuilder> SeralizerEvent;

        internal static SerializerBuilder ExecuteSerializerOptions(SerializerBuilder builder)
        {
            SeralizerEvent?.Invoke(null, builder);

            return builder;
        }

        internal static DeserializerBuilder ExecuteDeserializerOptions(DeserializerBuilder builder)
        {
            DeseralizerEvent?.Invoke(null, builder);

            return builder;
        }
    }
}
