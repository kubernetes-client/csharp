using System.Threading;
using System.Threading.Tasks;
using k8s.Models;

namespace k8s.LeaderElection.ResourceLock
{
    public class EndpointsLock : MetaObjectAnnotationLock<V1Endpoints>
    {
        public EndpointsLock(IKubernetes client, string @namespace, string name, string identity)
            : base(client, @namespace, name, identity)
        {
        }

        protected override Task<V1Endpoints> ReadMetaObjectAsync(IKubernetes client, string name, string namespaceParameter, CancellationToken cancellationToken)
        {
            return client.CoreV1.ReadNamespacedEndpointsAsync(name, namespaceParameter, cancellationToken: cancellationToken);
        }

        protected override Task<V1Endpoints> CreateMetaObjectAsync(IKubernetes client, V1Endpoints obj, string namespaceParameter,
            CancellationToken cancellationToken)
        {
            return client.CoreV1.CreateNamespacedEndpointsAsync(obj, namespaceParameter, cancellationToken: cancellationToken);
        }

        protected override Task<V1Endpoints> ReplaceMetaObjectAsync(IKubernetes client, V1Endpoints obj, string name, string namespaceParameter,
            CancellationToken cancellationToken)
        {
            return client.CoreV1.ReplaceNamespacedEndpointsAsync(obj, name, namespaceParameter, cancellationToken: cancellationToken);
        }
    }
}
