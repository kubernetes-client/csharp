using System;
using System.IO.Abstractions;

namespace k8s
{
    public static class FileUtils
    {
        private static readonly IFileSystem realFileSystem = new FileSystem();
        private static IFileSystem currentFileSystem = null;

        public static void InjectFilesystem(IFileSystem fs)
        {
            currentFileSystem = fs;
        }

        public static IFileSystem FileSystem()
        {
            return currentFileSystem != null ? currentFileSystem : realFileSystem;
        }

        public sealed class InjectedFileSystem : IDisposable
        {
            public InjectedFileSystem(IFileSystem fs)
            {
                InjectFilesystem(fs);
            }

            public void Dispose()
            {
                InjectFilesystem(null);
            }
        }
    }
}
