namespace k8s
{
    /// <summary>
    /// Provides the <c>ReadLineAsync(CancellationToken)</c> overload which is only available
    /// on .NET 7.0 or greater, so shared sources can rely on it unconditionally.
    /// </summary>
    internal static class TextReaderExtensions
    {
        public static Task<string> ReadLineAsync(this TextReader reader, CancellationToken cancellationToken)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<string>(cancellationToken);
            }

            var task = reader.ReadLineAsync();

            if (task.IsCompleted || !cancellationToken.CanBeCanceled)
            {
                return task;
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
    }
}
