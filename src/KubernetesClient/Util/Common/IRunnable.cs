using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace k8s.Util.Common
{
    public interface IRunnable : IDisposable
    {
        /// <summary>
        /// A new task ready to run. The provided cancellation token's delegate registration does a graceful stop of appropriate resources
        /// but leaves things in a state ready for restart. This is equivalent to the "Stop" function in the Java client.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>the task</returns>
        Task RunAsync(CancellationToken cancellationToken);
    }

    // ReSharper disable once IdentifierTypo
    public interface IStartables : IDisposable
    {
        /// <summary>
        /// A collection of new no-blocking tasks created and started from <see cref="Task.Factory.StartNew"/>. The provided cancellation token's delegate registration does a graceful shutdown.
        /// Callers of the task don't need knowledge of what a graceful shutdown looks like.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>the started task collection</returns>
        IEnumerable<Task> StartAsync(CancellationToken cancellationToken);
    }
}
