namespace k8s
{
    /// <summary>
    /// Polyfills for the cancellable overloads which are only available on modern .NET,
    /// so the shared sources can rely on them unconditionally.
    /// </summary>
    internal static class CancellationExtensions
    {
        /// <summary>
        /// Polyfill of <c>Task&lt;TResult&gt;.WaitAsync(CancellationToken)</c>.
        /// </summary>
        public static Task<TResult> WaitAsync<TResult>(this Task<TResult> task, CancellationToken cancellationToken)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (task.IsCompleted || !cancellationToken.CanBeCanceled)
            {
                return task;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<TResult>(cancellationToken);
            }

            // Observe any exception from the original task to prevent an
            // UnobservedTaskException when the continuation below is cancelled
            // before the original task faults (e.g. the transport tears down the
            // connection after cancellation).
            _ = task.ContinueWith(
                static t => { _ = t.Exception; },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

            // here to pass cancellationToken into task
            return task.ContinueWith(t => t.GetAwaiter().GetResult(), cancellationToken);
        }

        /// <summary>
        /// Polyfill of <c>TextReader.ReadLineAsync(CancellationToken)</c>.
        /// </summary>
        public static Task<string> ReadLineAsync(this TextReader reader, CancellationToken cancellationToken)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return reader.ReadLineAsync().WaitAsync(cancellationToken);
        }
    }
}
