namespace k8s
{
    internal static class FileSystem
    {
        public interface IFileSystem
        {
            Stream OpenRead(string path);

            bool Exists(string path);

            string ReadAllText(string path);
        }

        public static IFileSystem Current { get; private set; } = new RealFileSystem();

        public static IDisposable With(IFileSystem fileSystem)
        {
            return new InjectedFileSystem(fileSystem);
        }

        private class InjectedFileSystem : IDisposable
        {
            private readonly IFileSystem _original;

            public InjectedFileSystem(IFileSystem fileSystem)
            {
                _original = Current;
                Current = fileSystem;
            }

            public void Dispose()
            {
                Current = _original;
            }
        }

        private class RealFileSystem : IFileSystem
        {
            public bool Exists(string path)
            {
                return File.Exists(path);
            }

            public Stream OpenRead(string path)
            {
                return File.OpenRead(path);
            }

            public string ReadAllText(string path)
            {
                return File.ReadAllText(path);
            }
        }
    }
}
