using System.Runtime.InteropServices;
using Xunit;

namespace k8s.Tests
{
    public class OperatingSystemDependentFactAttribute : FactAttribute
    {
        public OperatingSystem Include { get; set; } = OperatingSystem.Linux | OperatingSystem.Windows | OperatingSystem.OSX;
        public OperatingSystem Exclude { get; set; }

        public override string Skip
        {
            get => IsOS(Include) && !IsOS(Exclude) ? null : "Not compatible with current OS";
            set { }
        }

        private bool IsOS(OperatingSystem operatingSystem)
        {
            if (operatingSystem.HasFlag(OperatingSystem.Linux) && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return true;
            }

            if (operatingSystem.HasFlag(OperatingSystem.Windows) && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return true;
            }

            if (operatingSystem.HasFlag(OperatingSystem.OSX) && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return true;
            }

            return false;
        }
    }
}
