using YamlDotNet.Serialization;

namespace k8s
{
    /// <summary>
    /// This class allows overriding the <see cref="YamlDotNet"/> settings.
    /// </summary>
    public static class KubernetesYamlConfiguration
    {
        internal static Action<DeserializerBuilder> DeseralizerAction { get; set; }
        internal static Action<SerializerBuilder> SeralizerAction { get; set; }

        /// <summary>
        /// Configures <see cref="SerializerBuilder"/> for <see cref="YamlDotNet"/>.
        /// </summary>
        /// <param name="configure">An <see cref="Action"/> to configure the <see cref="SerializerBuilder"/>.</param>
        public static void AddSerializerOptions(Action<SerializerBuilder> configure)
        {
            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            SeralizerAction = configure;
        }

        /// <summary>
        /// Configures <see cref="DeserializerBuilder"/> for <see cref="YamlDotNet"/>.
        /// </summary>
        /// <param name="configure">An <see cref="Action"/> to configure the <see cref="DeserializerBuilder"/>.</param>
        public static void AddDeserializerOptions(Action<DeserializerBuilder> configure)
        {
            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            DeseralizerAction = configure;
        }
    }
}
