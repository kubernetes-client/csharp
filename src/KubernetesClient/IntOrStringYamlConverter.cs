using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace k8s.Models
{
    public class IntOrStringYamlConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(IntstrIntOrString);
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

                    return new IntstrIntOrString(scalar?.Value);
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
            var obj = (IntstrIntOrString)value;
            emitter?.Emit(new YamlDotNet.Core.Events.Scalar(obj?.Value));
        }
    }
}
