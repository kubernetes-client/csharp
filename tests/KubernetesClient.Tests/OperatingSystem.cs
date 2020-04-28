using System;

namespace k8s.Tests
{
    [Flags]
    public enum OperatingSystem
    {
        Windows = 1,
        Linux = 2,
        OSX = 4
    }
}
