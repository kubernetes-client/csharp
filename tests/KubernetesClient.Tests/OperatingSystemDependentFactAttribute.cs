using System.Runtime.InteropServices;
using Xunit;

namespace k8s.Tests
{
    public class OperatingSystemDependentFactAttribute : FactAttribute
    {
        public OperatingSystems Include { get; set; } = OperatingSystems.Linux | OperatingSystems.Windows | OperatingSystems.OSX;
        public OperatingSystems Exclude { get; set; }

        public override string Skip
        {
            get => IsOS(Include) && !IsOS(Exclude) ? null : "Not compatible with current OS";
            set { }
        }

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
