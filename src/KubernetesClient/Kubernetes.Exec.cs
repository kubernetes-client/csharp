using k8s.Models;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    public partial class Kubernetes
    {
        public async Task<int> NamespacedPodExecAsync(string name, string @namespace, string container, IEnumerable<string> command, bool tty, ExecAsyncCallback action, CancellationToken cancellationToken)
        {
            // All other parameters are being validated by MuxedStreamNamespacedPodExecAsync
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            PipeStream stdIn = new PipeStream(), stdOut = new PipeStream(), stdErr = new PipeStream();
            Task<V1Status> execTask = Request<V1Pod>(@namespace, name).Body(stdIn)
                .ExecCommandAsync(command.First(), command.Skip(1).ToArray(), container, stdOut, stdErr, tty, false, cancellationToken);
            Task actionTask = action(stdIn, stdOut, stdErr);
            var status = await execTask.ConfigureAwait(false);
            stdOut.Close(); // complete the output streams in case the action is blocked waiting for them
            stdErr.Close();
            await actionTask.ConfigureAwait(false);
            if (status.Code.Value < 0) // if the exit code is unknown, throw an exception
            {
                throw new KubernetesException(status);
            }
            return status.Code.Value;
        }

        /// <summary>
        /// Determines the process' exit code based on a <see cref="V1Status"/> message.
        ///
        /// This will:
        /// - return 0 if the process completed successfully
        /// - return the exit code if the process completed with a non-zero exit code
        /// - throw a <see cref="KubernetesException"/> in all other cases.
        /// </summary>
        /// <param name="status">
        /// A <see cref="V1Status"/> object.
        /// </param>
        /// <returns>
        /// The process exit code.
        /// </returns>
        public static int GetExitCodeOrThrow(V1Status status)
        {
            if (status == null)
            {
                throw new ArgumentNullException(nameof(status));
            }

            if (status.Status == "Success")
            {
                return 0;
            }
            else if (status.Status == "Failure" && status.Reason == "NonZeroExitCode")
            {
                var exitCodeString = status.Details.Causes.FirstOrDefault(c => c.Reason == "ExitCode")?.Message;

                if (int.TryParse(exitCodeString, out int exitCode))
                {
                    return exitCode;
                }
                else
                {
                    throw new KubernetesException(status);
                }
            }
            else
            {
                throw new KubernetesException(status);
            }
        }
    }
}
