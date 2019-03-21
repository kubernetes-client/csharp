using System.IO;
using System.Threading.Tasks;

namespace k8s
{
    /// <summary>
    /// A prototype for a callback which asynchronously processes the standard input, standard output and standard error of a command executing in
    /// a container.
    /// </summary>
    /// <param name="stdIn">
    /// The standard intput stream of the process.
    /// </param>
    /// <param name="stdOut">
    /// The standard output stream of the process.
    /// </param>
    /// <param name="stdErr">
    /// The standard error stream of the remote process.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> which represents the asynchronous processing of the process input, output and error streams. This task
    /// should complete once you're done interacting with the remote process.
    /// </returns>
    public delegate Task ExecAsyncCallback(Stream stdIn, Stream stdOut, Stream stdErr);
}
