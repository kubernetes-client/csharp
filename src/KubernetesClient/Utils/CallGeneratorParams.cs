namespace System.Collections.Generic
{
    public class CallGeneratorParams
    {
        public bool Watch { get; }
        public string ResourceVersion { get; }
        public TimeSpan Timeout { get; }

        public CallGeneratorParams(bool watch, string resourceVersion, TimeSpan timeout) 
        {
            Watch = watch;
            ResourceVersion = resourceVersion;
            Timeout = timeout;
        }
    }
}