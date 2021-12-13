namespace k8s.Util.Common.Generic.Options
{
    public class PatchOptions
    {
        public string DryRun { get; private set; }

        public bool Force { get; private set; }

        public string FieldManager { get; private set; }

        public PatchOptions(string dryRun = default, bool force = false, string fieldManager = default)
        {
            DryRun = dryRun;
            Force = force;
            FieldManager = fieldManager;
        }
    }
}
