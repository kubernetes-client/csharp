using System;

namespace k8s.Tests
{
    [Flags]
    public enum OperatingSystems
    {
        /// <summary>Windows operating system.</summary>
        Windows = 1,

        /// <summary>Linux operating system.</summary>
        Linux = 2,

        /// <summary>OSX operating system.</summary>
        OSX = 4,
    }
}
