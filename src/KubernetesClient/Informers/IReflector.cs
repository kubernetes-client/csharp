using System.Threading;

namespace k8s.informers
{
    public interface IReflector<T,L> {
        void Run(CancellationToken c);
    }
}

