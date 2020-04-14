using System.Threading;
using System.Threading.Tasks;

namespace informers
{
    /// <summary>
    ///     Base interface for implementing controllers
    /// </summary>
    public interface IController
    {
        /// <summary>
        ///     Signals that controller is done processing all the work and no more work will ever be processed.
        ///     Mainly useful in testing
        /// </summary>
        public Task Initialize(CancellationToken cancellationToken);
    }
}
