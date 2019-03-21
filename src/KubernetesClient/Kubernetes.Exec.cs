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

            try
            {
                using (var muxedStream = await this.MuxedStreamNamespacedPodExecAsync(name: name, @namespace: @namespace, command: command, container: container, tty: tty, cancellationToken: cancellationToken).ConfigureAwait(false))
                using (Stream stdIn = muxedStream.GetStream(null, ChannelIndex.StdIn))
                using (Stream stdOut= muxedStream.GetStream(ChannelIndex.StdOut, null))
                using (Stream stdErr = muxedStream.GetStream(ChannelIndex.StdErr, null))
                using (Stream error = muxedStream.GetStream(ChannelIndex.Error, null))
                using (StreamReader errorReader = new StreamReader(error))
                {
                    muxedStream.Start();

                    await action(stdIn, stdOut, stdErr).ConfigureAwait(false);

                    var errors = await errorReader.ReadToEndAsync().ConfigureAwait(false);

                    // StatusError is defined here:
                    // https://github.com/kubernetes/kubernetes/blob/068e1642f63a1a8c48c16c18510e8854a4f4e7c5/staging/src/k8s.io/apimachinery/pkg/api/errors/errors.go#L37
                    var returnMessage = SafeJsonConvert.DeserializeObject<V1Status>(errors);
                    return GetExitCodeOrThrow(returnMessage);
                }
            }
            catch (HttpOperationException httpEx) when (httpEx.Body is V1Status)
            {
                throw new KubernetesException((V1Status)httpEx.Body);
            }
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
