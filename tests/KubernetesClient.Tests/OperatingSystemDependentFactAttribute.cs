using System.Runtime.InteropServices;
using Xunit;
using Xunit.v3;

namespace k8s.Tests
{
    public class OperatingSystemDependentFactAttribute : FactAttribute, IFactAttribute
    {
        public OperatingSystems Include { get; set; } = OperatingSystems.Linux | OperatingSystems.Windows | OperatingSystems.OSX;
        public OperatingSystems Exclude { get; set; }

        string IFactAttribute.Skip => IsOS(Include) && !IsOS(Exclude) ? null : "Not compatible with current OS";

        private bool IsOS(OperatingSystems operatingSystems)
        {
            if (operatingSystems.HasFlag(OperatingSystems.Linux) && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return true;
            }

            if (operatingSystems.HasFlag(OperatingSystems.Windows) && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return true;
            }

            if (operatingSystems.HasFlag(OperatingSystems.OSX) && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return true;
            }

            return false;
        }
    }
}
