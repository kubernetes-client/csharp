// ReSharper disable once CheckNamespace

using System;

namespace k8s.Util.Utils
{
    public class CallGeneratorParams
    {
        public bool Watch { get; }
        public string ResourceVersion { get; }
        public int? TimeoutSeconds { get; }

        public CallGeneratorParams(bool watch, string resourceVersion, int? timeoutSeconds)
        {
            Watch = watch;
            ResourceVersion = resourceVersion;
            TimeoutSeconds = timeoutSeconds;
        }
    }
}
