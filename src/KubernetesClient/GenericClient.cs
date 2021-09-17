using Microsoft.Rest.Serialization;
using System;
using System.Net.Http;
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

        public async Task<T> ListAsync<T>(CancellationToken cancel = default(CancellationToken))
        where T : IKubernetesObject
        {
            var resp = await this.kubernetes.ListClusterCustomObjectWithHttpMessagesAsync(this.group, this.version, this.plural, cancellationToken: cancel).ConfigureAwait(false);
            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        public async Task<T> ListNamespacedAsync<T>(string ns, CancellationToken cancel = default(CancellationToken))
        where T : IKubernetesObject
        {
            var resp = await this.kubernetes.ListNamespacedCustomObjectWithHttpMessagesAsync(this.group, this.version, ns, this.plural, cancellationToken: cancel).ConfigureAwait(false);
            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        public async Task<T> ReadNamespacedAsync<T>(string ns, string name, CancellationToken cancel = default(CancellationToken))
        where T : IKubernetesObject
        {
            var resp = await this.kubernetes.GetNamespacedCustomObjectWithHttpMessagesAsync(this.group, this.version, ns, this.plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        public async Task<T> ReadAsync<T>(string name, CancellationToken cancel = default(CancellationToken))
        where T : IKubernetesObject
        {
            var resp = await this.kubernetes.GetClusterCustomObjectWithHttpMessagesAsync(this.group, this.version, this.plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        public async Task<T> DeleteAsync<T>(string name, CancellationToken cancel = default(CancellationToken))
        where T : IKubernetesObject
        {
            var resp = await this.kubernetes.DeleteClusterCustomObjectWithHttpMessagesAsync(this.group, this.version, this.plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        public async Task<T> DeleteNamespacedAsync<T>(string ns, string name, CancellationToken cancel = default(CancellationToken))
        where T : IKubernetesObject
        {
            var resp = await this.kubernetes.DeleteNamespacedCustomObjectWithHttpMessagesAsync(this.group, this.version, ns, this.plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.kubernetes.Dispose();
        }
    }
}
