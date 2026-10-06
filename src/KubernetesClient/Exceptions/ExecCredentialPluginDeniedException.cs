namespace k8s.Exceptions
{
    /// <summary>
    /// An exec credential plugin was denied by the application policy before starting a process.
    /// </summary>
    public class ExecCredentialPluginDeniedException : KubeConfigException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecCredentialPluginDeniedException"/> class.
        /// </summary>
        /// <param name="command">The denied executable.</param>
        public ExecCredentialPluginDeniedException(string command)
            : base($"Exec credential plugin '{command}' was denied by the application policy.")
        {
            Command = command;
        }

        /// <summary>
        /// Gets the denied executable.
        /// </summary>
        public string Command { get; }
    }
}
