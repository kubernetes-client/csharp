using System;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    /// <summary>
    /// Represents a line oriented stream used for watching server responses
    /// </summary>
    public interface IAsyncLineReader : IDisposable
    {
        /// <summary>
        /// Read a line from the server
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the read</param>
        Task<string> ReadLineAsync(CancellationToken cancellationToken);
    }
}
