using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace k8s
{
    internal class FloatEmitter : ChainedEventEmitter
    {
        public FloatEmitter(IEventEmitter nextEmitter)
            : base(nextEmitter)
        {
        }

        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            switch (eventInfo.Source.Value)
            {
                // Floating point numbers should always render at least one zero (e.g. 1.0f => '1.0' not '1')
                case double d:
                    emitter.Emit(new Scalar(d.ToString("0.0######################", CultureInfo.InvariantCulture)));
                    break;
                case float f:
                    emitter.Emit(new Scalar(f.ToString("0.0######################", CultureInfo.InvariantCulture)));
                    break;
                default:
                    base.Emit(eventInfo, emitter);
                    break;
            }
        }
    }
}
