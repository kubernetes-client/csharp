namespace k8s
{
    public class GenericClient : IDisposable
    {
        private readonly IKubernetes kubernetes;
        private readonly string group;
        private readonly string version;
        private readonly string plural;
        private readonly bool disposeClient;

        [Obsolete]
        public GenericClient(KubernetesClientConfiguration config, string group, string version, string plural)
            : this(new Kubernetes(config), group, version, plural)
        {
        }

        public GenericClient(IKubernetes kubernetes, string version, string plural, bool disposeClient = true)
            : this(kubernetes, "", version, plural, disposeClient)
        {
        }

        public GenericClient(IKubernetes kubernetes, string group, string version, string plural, bool disposeClient = true)
        {
            this.group = group;
            this.version = version;
            this.plural = plural;
            this.kubernetes = kubernetes;
            this.disposeClient = disposeClient;
        }

        public async Task<T> CreateAsync<T>(T obj, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.CreateClusterCustomObjectWithHttpMessagesAsync<T>(obj, group, version, plural, cancellationToken: cancel).ConfigureAwait(false);
            return resp.Body;
        }

        public async Task<T> CreateNamespacedAsync<T>(T obj, string ns, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.CreateNamespacedCustomObjectWithHttpMessagesAsync<T>(obj, group, version, ns, plural, cancellationToken: cancel).ConfigureAwait(false);
            return resp.Body;
        }

        public async Task<T> ListAsync<T>(CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.ListClusterCustomObjectWithHttpMessagesAsync<T>(group, version, plural, cancellationToken: cancel).ConfigureAwait(false);
            return resp.Body;
        }

        public async Task<T> ListNamespacedAsync<T>(string ns, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync<T>(group, version, ns, plural, cancellationToken: cancel).ConfigureAwait(false);
            return resp.Body;
        }

        public async Task<T> ReadNamespacedAsync<T>(string ns, string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.GetNamespacedCustomObjectWithHttpMessagesAsync<T>(group, version, ns, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return resp.Body;
        }

        public async Task<T> ReadAsync<T>(string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.GetClusterCustomObjectWithHttpMessagesAsync<T>(group, version, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return resp.Body;
        }

        public async Task<T> DeleteAsync<T>(string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.DeleteClusterCustomObjectWithHttpMessagesAsync<T>(group, version, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return resp.Body;
        }

        public async Task<T> DeleteNamespacedAsync<T>(string ns, string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.DeleteNamespacedCustomObjectWithHttpMessagesAsync<T>(group, version, ns, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return resp.Body;
        }

        public async Task<T> PatchAsync<T>(V1Patch patch, string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.PatchClusterCustomObjectWithHttpMessagesAsync<T>(patch, group, version, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return resp.Body;
        }

        public async Task<T> PatchNamespacedAsync<T>(V1Patch patch, string ns, string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.PatchNamespacedCustomObjectWithHttpMessagesAsync<T>(patch, group, version, ns, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return resp.Body;
        }

        public async Task<T> ReplaceAsync<T>(T obj, string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.ReplaceClusterCustomObjectWithHttpMessagesAsync<T>(obj, group, version, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return resp.Body;
        }

        public async Task<T> ReplaceNamespacedAsync<T>(T obj, string ns, string name, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var resp = await kubernetes.CustomObjects.ReplaceNamespacedCustomObjectWithHttpMessagesAsync<T>(obj, group, version, ns, plural, name, cancellationToken: cancel).ConfigureAwait(false);
            return resp.Body;
        }

        public IAsyncEnumerable<(WatchEventType, T)> WatchAsync<T>(Action<Exception> onError = null, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var respTask = kubernetes.CustomObjects.ListClusterCustomObjectWithHttpMessagesAsync<T>(group, version, plural, watch: true, cancellationToken: cancel);
            return respTask.WatchAsync<T, T>(onError, cancel);
        }

        public IAsyncEnumerable<(WatchEventType, T)> WatchNamespacedAsync<T>(string ns, Action<Exception> onError = null, CancellationToken cancel = default)
        where T : IKubernetesObject
        {
            var respTask = kubernetes.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync<T>(group, version, ns, plural, watch: true, cancellationToken: cancel);
            return respTask.WatchAsync<T, T>(onError, cancel);
        }

        public Watcher<T> Watch<T>(Action<WatchEventType, T> onEvent, Action<Exception> onError = null, Action onClosed = null)
        where T : IKubernetesObject
        {
            var respTask = kubernetes.CustomObjects.ListClusterCustomObjectWithHttpMessagesAsync<T>(group, version, plural, watch: true);
            return respTask.Watch(onEvent, onError, onClosed);
        }

        public Watcher<T> WatchNamespaced<T>(string ns, Action<WatchEventType, T> onEvent, Action<Exception> onError = null, Action onClosed = null)
        where T : IKubernetesObject
        {
            var respTask = kubernetes.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync<T>(group, version, ns, plural, watch: true);
            return respTask.Watch(onEvent, onError, onClosed);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposeClient)
            {
                kubernetes.Dispose();
            }
        }
    }
}
