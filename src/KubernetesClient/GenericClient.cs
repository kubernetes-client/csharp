using Microsoft.Rest.Serialization;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace k8s
{
    public class GenericClient : IDisposable
    {
        internal class TweakApiHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage msg, CancellationToken cancel)
            {
                msg.RequestUri = new Uri(msg.RequestUri, msg.RequestUri.AbsolutePath.Replace("/apis//", "/api/"));
                return base.SendAsync(msg, cancel);
            }
        }

        private readonly IKubernetes kubernetes;
        private readonly string group;
        private readonly string version;
        private readonly string plural;


        public GenericClient(KubernetesClientConfiguration config, string group, string version, string plural)
        {
            this.group = group;
            this.version = version;
            this.plural = plural;

            if (string.IsNullOrEmpty(group))
            {
                this.kubernetes = new Kubernetes(config, new DelegatingHandler[] { new TweakApiHandler() });
            }
            else
            {
                this.kubernetes = new Kubernetes(config);
            }
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
