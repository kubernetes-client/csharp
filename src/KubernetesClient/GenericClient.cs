using System.Threading;
using System.Threading.Tasks;


namespace k8s
{
    public class GenericClient : IDisposable
    {
        private readonly IKubernetes kubernetes;
        private readonly string group;
        private readonly string version;
        private readonly string plural;

        [Obsolete]
        public GenericClient(KubernetesClientConfiguration config, string group, string version, string plural)
            : this(new Kubernetes(config), group, version, plural)
        {
        }

        public GenericClient(IKubernetes kubernetes, string version, string plural)
            : this(kubernetes, "", version, plural)
        {
        }

        public GenericClient(IKubernetes kubernetes, string group, string version, string plural)
        {
            this.group = group;
            this.version = version;
            this.plural = plural;
            this.kubernetes = kubernetes;
        }

        public async Task<T> CreateAsync<T>(T obj, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.CreateClusterCustomObjectWithHttpMessagesAsync(obj, group, version, plural, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> CreateNamespacedAsync<T>(T obj, string ns, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.CreateNamespacedCustomObjectWithHttpMessagesAsync(obj, group, version, ns, plural, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> ListAsync<T>(CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.ListClusterCustomObjectWithHttpMessagesAsync(group, version, plural, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> ListNamespacedAsync<T>(string ns, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync(group, version, ns, plural, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> ReadNamespacedAsync<T>(string ns, string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.GetNamespacedCustomObjectWithHttpMessagesAsync(group, version, ns, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> ReadAsync<T>(string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.GetClusterCustomObjectWithHttpMessagesAsync(group, version, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> DeleteAsync<T>(string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.DeleteClusterCustomObjectWithHttpMessagesAsync(group, version, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> DeleteNamespacedAsync<T>(string ns, string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.DeleteNamespacedCustomObjectWithHttpMessagesAsync(group, version, ns, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            kubernetes.Dispose();
        }
    }
}
