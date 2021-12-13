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
            var resp = await kubernetes.CreateClusterCustomObjectWithHttpMessagesAsync(obj, group, version, plural, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> CreateNamespacedAsync<T>(T obj, string ns, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CreateNamespacedCustomObjectWithHttpMessagesAsync(obj, group, version, ns, plural, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> ListAsync<T>(CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.ListClusterCustomObjectWithHttpMessagesAsync(group, version, plural, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> ListNamespacedAsync<T>(string ns, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.ListNamespacedCustomObjectWithHttpMessagesAsync(group, version, ns, plural, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> ReadNamespacedAsync<T>(string ns, string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.GetNamespacedCustomObjectWithHttpMessagesAsync(group, version, ns, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> ReadAsync<T>(string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.GetClusterCustomObjectWithHttpMessagesAsync(group, version, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> DeleteAsync<T>(string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.DeleteClusterCustomObjectWithHttpMessagesAsync(group, version, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return KubernetesJson.Deserialize<T>(resp.Body.ToString());
        }

        public async Task<T> DeleteNamespacedAsync<T>(string ns, string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.DeleteNamespacedCustomObjectWithHttpMessagesAsync(group, version, ns, plural, name, cancellationToken: cancel).ConfigureAwait(false);
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
