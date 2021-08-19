using Newtonsoft.Json;

namespace k8s.Util.Common.Generic.Options
{
    public class UpdateOptions
    {
        [JsonProperty("dryRun")]
        public string DryRun { get; private set; }

        [JsonProperty("force")]
        public bool Force { get; private set; }

        [JsonProperty("fieldManager")]
        public string FieldManager { get; private set; }

        public UpdateOptions(string dryRun = default, bool force = false, string fieldManager = default)
        {
            DryRun = dryRun;
            Force = force;
            FieldManager = fieldManager;
        }
    }
}
