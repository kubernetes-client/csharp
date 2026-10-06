using k8s.Exceptions;
using System.Runtime.InteropServices;

namespace k8s
{
    /// <summary>
    /// Application-controlled policy for kubeconfig exec credential plugins.
    /// </summary>
    public sealed class ExecCredentialPluginPolicy
    {
        private static readonly StringComparison CommandComparison =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        private readonly bool allowAll;
        private readonly string[] commands;

        private ExecCredentialPluginPolicy(bool allowAll, string[] commands)
        {
            this.allowAll = allowAll;
            this.commands = commands;
        }

        /// <summary>
        /// Allows any exec plugin. This is the default for compatibility.
        /// </summary>
        public static ExecCredentialPluginPolicy AllowAll { get; } = new ExecCredentialPluginPolicy(true, Array.Empty<string>());

        /// <summary>
        /// Prevents all exec plugins from running.
        /// </summary>
        public static ExecCredentialPluginPolicy DenyAll { get; } = new ExecCredentialPluginPolicy(false, Array.Empty<string>());

        /// <summary>
        /// Allows command basenames or exact, absolute executable paths. An empty list denies all commands.
        /// </summary>
        /// <remarks>
        /// Basenames allow that executable name in any directory, including directories searched through PATH.
        /// Absolute paths use exact textual matching, without resolving symlinks or normalizing paths.
        /// Matching is case-insensitive on Windows and case-sensitive elsewhere.
        /// This policy does not restrict plugin arguments or environment variables.
        /// </remarks>
        /// <param name="commands">Approved basenames or absolute paths. Relative paths and invalid entries are rejected.</param>
        /// <returns>An immutable allowlist policy.</returns>
        public static ExecCredentialPluginPolicy Allowlist(params string[] commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException(nameof(commands));
            }

            var copy = (string[])commands.Clone();
            foreach (var command in copy)
            {
                if (string.IsNullOrWhiteSpace(command) || command == "." || command == ".." ||
                    command.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || command.IndexOfAny(new[] { '*', '?' }) >= 0 ||
                    (!IsBasename(command) && !IsAbsolutePath(command)))
                {
                    throw new ArgumentException("Exec allowlist entries must be command basenames or absolute executable paths.", nameof(commands));
                }
            }

            return new ExecCredentialPluginPolicy(false, copy);
        }

        internal bool Allows(string command)
        {
            if (allowAll)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return commands.Any(entry => string.Equals(
                entry, IsBasename(entry) ? Path.GetFileName(command) : command, CommandComparison));
        }

        internal void Validate(string command)
        {
            if (!Allows(command))
            {
                throw new ExecCredentialPluginDeniedException(command);
            }
        }

        private static bool IsBasename(string command)
        {
            return command.IndexOfAny(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, ':' }) < 0;
        }

        private static bool IsAbsolutePath(string command)
        {
            if (!Path.IsPathRooted(command))
            {
                return false;
            }

            var root = Path.GetPathRoot(command);
            return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
                (root.Length > 1 && root[root.Length - 1] == Path.DirectorySeparatorChar);
        }
    }
}
