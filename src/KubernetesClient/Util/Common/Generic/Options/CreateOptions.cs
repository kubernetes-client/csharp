using Newtonsoft.Json;

namespace k8s.Util.Common.Generic.Options
{
    public class CreateOptions
    {
        public string DryRun { get; private set; }

        public string FieldManager { get; private set; }

        public CreateOptions(string dryRun = default, string fieldManager = default)
        {
            DryRun = dryRun;
            FieldManager = fieldManager;
        }
    }
}
