namespace k8s.LeaderElection.ResourceLock
{
    public class ConfigMapLock : MetaObjectAnnotationLock<V1ConfigMap>
    {
        public ConfigMapLock(IKubernetes client, string @namespace, string name, string identity)
            : base(client, @namespace, name, identity)
        {
        }

        protected override Task<V1ConfigMap> ReadMetaObjectAsync(IKubernetes client, string name,
            string namespaceParameter,
            CancellationToken cancellationToken)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return client.CoreV1.ReadNamespacedConfigMapAsync(name, namespaceParameter, cancellationToken: cancellationToken);
        }

        protected override Task<V1ConfigMap> CreateMetaObjectAsync(IKubernetes client, V1ConfigMap obj,
            string namespaceParameter,
            CancellationToken cancellationToken)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return client.CoreV1.CreateNamespacedConfigMapAsync(obj, namespaceParameter, cancellationToken: cancellationToken);
        }

        protected override Task<V1ConfigMap> ReplaceMetaObjectAsync(IKubernetes client, V1ConfigMap obj, string name,
            string namespaceParameter,
            CancellationToken cancellationToken)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return client.CoreV1.ReplaceNamespacedConfigMapAsync(obj, name, namespaceParameter, cancellationToken: cancellationToken);
        }
    }
}
