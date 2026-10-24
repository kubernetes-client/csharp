using k8s.Exceptions;
using k8s.KubeConfigModels;
using System.Runtime.InteropServices;

namespace k8s
{
    /// <summary>
    /// Application-controlled policy for kubeconfig exec credential plugins.
    /// </summary>
    public sealed class ExecCredentialPluginPolicy
    {
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
        /// Allows command basenames or absolute executable paths. The list must not be empty.
        /// </summary>
        /// <remarks>
        /// Commands match if their text is identical or if both resolve to the same executable using PATH.
        /// Resolution does not follow symlinks or support shell globs.
        /// Environment variables that can redirect executable resolution or inject code are rejected.
        /// Other kubeconfig-supplied environment variables are allowed. Plugin arguments are not restricted.
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
            if (copy.Length == 0)
            {
                throw new ArgumentException("Exec allowlists must contain at least one command.", nameof(commands));
            }

            foreach (var command in copy)
            {
                if (string.IsNullOrWhiteSpace(command) || command == "." || command == ".." ||
                    HasInvalidCommandCharacters(command) || command.IndexOfAny(new[] { '*', '?' }) >= 0 ||
                    (!IsBasename(command) && !IsAbsolutePath(command)))
                {
                    throw new ArgumentException("Exec allowlist entries must be command basenames or absolute executable paths.", nameof(commands));
                }
            }

            return new ExecCredentialPluginPolicy(false, copy);
        }

        internal bool Allows(string command)
            => Allows(command, Environment.GetEnvironmentVariable("PATH"));

        internal bool Allows(string command, string path)
        {
            if (allowAll)
            {
                return true;
            }

            if (commands.Length == 0 || string.IsNullOrWhiteSpace(command) || HasInvalidCommandCharacters(command))
            {
                return false;
            }

            if (commands.Any(entry => string.Equals(entry, command, StringComparison.Ordinal)))
            {
                return true;
            }

            if (!TryResolveCommand(command, path, out var resolvedCommand))
            {
                return false;
            }

            return commands.Any(entry =>
                TryResolveCommand(entry, path, out var resolvedEntry) &&
                string.Equals(resolvedEntry, resolvedCommand, StringComparison.Ordinal));
        }

        internal string Validate(string command, ExternalExecution execution)
        {
            if (allowAll)
            {
                return command;
            }

            if (HasDangerousEnvironmentVariable(execution.EnvironmentVariables) || !Allows(command))
            {
                throw new ExecCredentialPluginDeniedException(command);
            }

            return TryResolveCommand(command, Environment.GetEnvironmentVariable("PATH"), out var resolvedCommand)
                ? resolvedCommand
                : command;
        }

        private static bool HasDangerousEnvironmentVariable(IList<Dictionary<string, string>> environmentVariables)
        {
            return environmentVariables?.Any(variable =>
                variable != null && variable.ContainsKey("name") &&
                IsDangerousEnvironmentVariable(variable["name"])) == true;
        }

        private static bool IsDangerousEnvironmentVariable(string name)
        {
            return name != null && (string.Equals(name, "PATH", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "PATHEXT", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "COMSPEC", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "LIBPATH", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "SHLIB_PATH", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("LD_", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("DYLD_", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "PYTHONPATH", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "PYTHONHOME", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "PYTHONSTARTUP", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "PYTHONINSPECT", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "PERL5LIB", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "PERL5OPT", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "RUBYOPT", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "RUBYLIB", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "NODE_OPTIONS", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "NODE_PATH", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "JAVA_TOOL_OPTIONS", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "_JAVA_OPTIONS", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "JDK_JAVA_OPTIONS", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "CLASSPATH", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "DOTNET_STARTUP_HOOKS", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "BASH_ENV", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "PYTHONUSERBASE", StringComparison.OrdinalIgnoreCase));
        }

        private static bool TryResolveCommand(string command, string path, out string resolvedCommand)
        {
            resolvedCommand = null;
            if (string.IsNullOrWhiteSpace(command) || HasInvalidCommandCharacters(command))
            {
                return false;
            }

            if (Path.IsPathRooted(command) ||
                command.IndexOf(Path.DirectorySeparatorChar) >= 0 ||
                command.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
            {
                foreach (var extension in GetExecutableExtensions(command))
                {
                    if (TryGetFullExistingPath(command + extension, out resolvedCommand))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            foreach (var directory in path.Split(Path.PathSeparator))
            {
                if (string.IsNullOrEmpty(directory) || !Path.IsPathRooted(directory))
                {
                    continue;
                }

                foreach (var extension in GetExecutableExtensions(command))
                {
                    if (TryGetFullExistingPath(Path.Combine(directory, command + extension), out resolvedCommand))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryGetFullExistingPath(string command, out string resolvedCommand)
        {
            resolvedCommand = null;
            try
            {
                var fullPath = Path.GetFullPath(command);
                if (!File.Exists(fullPath))
                {
                    return false;
                }

                resolvedCommand = fullPath;
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
            catch (PathTooLongException)
            {
                return false;
            }
        }

        private static IEnumerable<string> GetExecutableExtensions(string command)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || Path.HasExtension(command))
            {
                yield return string.Empty;
                yield break;
            }

            var pathExtensions = Environment.GetEnvironmentVariable("PATHEXT");
            if (string.IsNullOrEmpty(pathExtensions))
            {
                pathExtensions = ".COM;.EXE;.BAT;.CMD";
            }

            foreach (var extension in pathExtensions.Split(';'))
            {
                if (!string.IsNullOrEmpty(extension))
                {
                    yield return extension;
                }
            }
        }

        private static bool IsBasename(string command)
        {
            return command.IndexOfAny(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, ':' }) < 0;
        }

        private static bool HasInvalidCommandCharacters(string command)
        {
            return command.IndexOfAny(Path.GetInvalidPathChars()) >= 0 ||
                command.IndexOfAny(new[] { '"', '\0', '\r', '\n' }) >= 0;
        }

        private static bool IsAbsolutePath(string command)
        {
            if (!Path.IsPathRooted(command))
            {
                return false;
            }

            var root = Path.GetPathRoot(command);
            return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
                root.StartsWith(@"\\", StringComparison.Ordinal) || root.StartsWith("//", StringComparison.Ordinal) ||
                (root.Length >= 3 && root[1] == ':' && (root[2] == '\\' || root[2] == '/'));
        }
    }
}
