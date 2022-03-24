using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace k8s.Models
{
    public class ResourceQuantityYamlConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(ResourceQuantity);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            if (parser?.Current is YamlDotNet.Core.Events.Scalar scalar)
            {
                try
                {
                    if (string.IsNullOrEmpty(scalar?.Value))
                    {
                        return null;
                    }

                    return new ResourceQuantity(scalar?.Value);
                }
                finally
                {
                    parser?.MoveNext();
                }
            }

            throw new InvalidOperationException(parser?.Current?.ToString());
        }


        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var obj = (ResourceQuantity)value;
            emitter?.Emit(new YamlDotNet.Core.Events.Scalar(obj?.ToString()));
        }
    }
}
