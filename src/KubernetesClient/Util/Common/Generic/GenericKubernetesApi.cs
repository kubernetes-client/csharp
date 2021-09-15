using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using k8s.Models;
using k8s.Util.Common.Generic.Options;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;

namespace k8s.Util.Common.Generic
{
    /// <summary>
    ///
    /// The Generic kubernetes api provides a unified client interface for not only the non-core-group
    /// built-in resources from kubernetes but also the custom-resources models meet the following
    /// requirements:
    ///
    /// 1. there's a `V1ObjectMeta` field in the model along with its getter/setter. 2. there's a
    /// `V1ListMeta` field in the list model along with its getter/setter. - supports Json
    /// serialization/deserialization. 3. the generic kubernetes api covers all the basic operations over
    /// the custom resources including {get, list, watch, create, update, patch, delete}.
    ///
    /// - For kubernetes-defined failures, the server will return a {@link V1Status} with 4xx/5xx
    /// code. The status object will be nested in {@link KubernetesApiResponse#getStatus()} - For the
    /// other unknown reason (including network, JVM..), throws an unchecked exception.
    /// </summary>
    public class GenericKubernetesApi
    {
        private readonly string _apiGroup;
        private readonly string _apiVersion;
        private readonly string _resourcePlural;
        private readonly IKubernetes _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericKubernetesApi"/> class.
        /// </summary>
        /// <param name="apiGroup"> the api group"></param>
        /// <param name="apiVersion"> the api version"></param>
        /// <param name="resourcePlural"> the resource plural, e.g. "jobs""></param>
        /// <param name="apiClient"> optional client"></param>
        public GenericKubernetesApi(string apiGroup = default, string apiVersion = default, string resourcePlural = default, IKubernetes apiClient = default)
        {
            _apiGroup = apiGroup ?? throw new ArgumentNullException(nameof(apiGroup));
            _apiVersion = apiVersion ?? throw new ArgumentNullException(nameof(apiVersion));
            _resourcePlural = resourcePlural ?? throw new ArgumentNullException(nameof(resourcePlural));
            _client = apiClient ?? new Kubernetes(KubernetesClientConfiguration.BuildDefaultConfig());
        }

        public TimeSpan ClientTimeout => _client.HttpClient.Timeout;

        public void SetClientTimeout(TimeSpan value)
        {
            _client.HttpClient.Timeout = value;
        }

        /// <summary>
        /// Get kubernetes object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="name">the object name </param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>The object</returns>
        public Task<T> GetAsync<T>(string name, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            return GetAsync<T>(name, new GetOptions(), cancellationToken);
        }

        /// <summary>
        /// Get kubernetes object under the namespaceProperty.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="namespaceProperty"> the namespaceProperty</param>
        /// <param name="name"> the name</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public Task<T> GetAsync<T>(string namespaceProperty, string name, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            return GetAsync<T>(namespaceProperty, name, new GetOptions(), cancellationToken);
        }

        /// <summary>
        /// List kubernetes object cluster-scoped.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public Task<T> ListAsync<T>(CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ListMeta>
        {
            return ListAsync<T>(new ListOptions(), cancellationToken);
        }

        /// <summary>
        /// List kubernetes object under the namespaceProperty.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="namespaceProperty"> the namespace</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public Task<T> ListAsync<T>(string namespaceProperty, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ListMeta>
        {
            return ListAsync<T>(namespaceProperty, new ListOptions(), cancellationToken);
        }

        /// <summary>
        /// Create kubernetes object, if the namespaceProperty in the object is present, it will send a
        /// namespaceProperty-scoped requests, vice versa.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="obj"> the object</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public Task<T> CreateAsync<T>(T obj, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            return CreateAsync(obj, new CreateOptions(), cancellationToken);
        }

        /// <summary>
        /// Create kubernetes object, if the namespaceProperty in the object is present, it will send a
        /// namespaceProperty-scoped requests, vice versa.
        /// </summary>
        /// <param name="obj"> the object</param>
        /// <param name="cancellationToken">the token </param>
        /// <typeparam name="T">the object type</typeparam>
        /// <returns>the kubernetes object</returns>
        public Task<T> UpdateAsync<T>(T obj, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            return UpdateAsync(obj, new UpdateOptions(), cancellationToken);
        }

        /// <summary>
        /// Patch kubernetes object.
        /// </summary>
        /// <param name="name"> the name</param>
        /// <param name="patch"> the string patch content</param>
        /// <param name="cancellationToken">the token </param>
        /// <typeparam name="T">the object type</typeparam>
        /// <returns>the kubernetes object</returns>
        public Task<T> PatchAsync<T>(string name, object patch, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            return PatchAsync<T>(name, patch, new PatchOptions(), cancellationToken);
        }

        /// <summary>
        /// Patch kubernetes object under the namespaceProperty.
        /// </summary>
        /// <param name="namespaceProperty"> the namespaceProperty</param>
        /// <param name="name"> the name</param>
        /// <param name="patch"> the string patch content</param>
        /// <param name="cancellationToken">the token </param>
        /// <typeparam name="T">the object type</typeparam>
        /// <returns>the kubernetes object</returns>
        public Task<T> PatchAsync<T>(string namespaceProperty, string name, object patch, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            return PatchAsync<T>(namespaceProperty, name, patch, new PatchOptions(), cancellationToken);
        }

        /// <summary>
        /// Delete kubernetes object.
        /// </summary>
        /// <param name="name"> the name</param>
        /// <param name="cancellationToken">the token </param>
        /// <typeparam name="T">the object type</typeparam>
        /// <returns>the kubernetes object</returns>
        public Task<T> DeleteAsync<T>(string name, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            return DeleteAsync<T>(name, new V1DeleteOptions(), cancellationToken);
        }

        /// <summary>
        /// Delete kubernetes object under the namespaceProperty.
        /// </summary>
        /// <param name="namespaceProperty"> the namespaceProperty</param>
        /// <param name="name"> the name</param>
        /// <param name="cancellationToken">the token </param>
        /// <typeparam name="T">the object type</typeparam>
        /// <returns>the kubernetes object</returns>
        public Task<T> DeleteAsync<T>(string namespaceProperty, string name, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            return DeleteAsync<T>(namespaceProperty, name, new V1DeleteOptions(), cancellationToken);
        }

        /// <summary>
        /// Creates a cluster-scoped Watch on the resource.
        /// </summary>
        /// <param name="onEvent">action on event</param>
        /// <param name="onError">action on error</param>
        /// <param name="onClosed">action on closed</param>
        /// <param name="cancellationToken">the token </param>
        /// <typeparam name="T">the object type</typeparam>
        /// <returns>the watchable</returns>
        public Watcher<T> Watch<T>(Action<WatchEventType, T> onEvent, Action<Exception> onError = default, Action onClosed = default, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            return Watch(new ListOptions(), onEvent, onError, onClosed, cancellationToken);
        }

        /// <summary>
        /// Creates a namespaceProperty-scoped Watch on the resource.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="namespaceProperty"> the namespaceProperty</param>
        /// <param name="onEvent">action on event</param>
        /// <param name="onError">action on error</param>
        /// <param name="onClosed">action on closed</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the watchable</returns>
        public Watcher<T> Watch<T>(string namespaceProperty, Action<WatchEventType, T> onEvent, Action<Exception> onError = default, Action onClosed = default,
            CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            return Watch(namespaceProperty, new ListOptions(), onEvent, onError, onClosed, cancellationToken);
        }

        // TODO(yue9944882): watch one resource?

        /// <summary>
        /// Get kubernetes object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="name"> the name</param>
        /// <param name="getOptions">the get options</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public async Task<T> GetAsync<T>(string name, GetOptions getOptions, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var resp = await _client.GetClusterCustomObjectWithHttpMessagesAsync(group: _apiGroup, plural: _resourcePlural, version: _apiVersion, name: name, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        /// <summary>
        /// Get kubernetes object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="namespaceProperty"> the namespaceProperty</param>
        /// <param name="name"> the name</param>
        /// <param name="getOptions">the get options</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public async Task<T> GetAsync<T>(string namespaceProperty, string name, GetOptions getOptions, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(namespaceProperty))
            {
                throw new ArgumentNullException(nameof(namespaceProperty));
            }

            var resp = await _client.GetNamespacedCustomObjectWithHttpMessagesAsync(group: _apiGroup, plural: _resourcePlural, version: _apiVersion, name: name, namespaceParameter: namespaceProperty,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        /// <summary>
        /// List kubernetes object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="listOptions">the list options</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public async Task<T> ListAsync<T>(ListOptions listOptions, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ListMeta>
        {
            if (listOptions == null)
            {
                throw new ArgumentNullException(nameof(listOptions));
            }

            var resp = await _client.ListClusterCustomObjectWithHttpMessagesAsync(group: _apiGroup, plural: _resourcePlural, version: _apiVersion, resourceVersion: listOptions.ResourceVersion,
                continueParameter: listOptions.Continue, fieldSelector: listOptions.FieldSelector, labelSelector: listOptions.LabelSelector, limit: listOptions.Limit,
                timeoutSeconds: listOptions.TimeoutSeconds, cancellationToken: cancellationToken).ConfigureAwait(false);
            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        /// <summary>
        /// List kubernetes object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="namespaceProperty"> the namespaceProperty</param>
        /// <param name="listOptions">the list options</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public async Task<T> ListAsync<T>(string namespaceProperty, ListOptions listOptions, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ListMeta>
        {
            if (listOptions == null)
            {
                throw new ArgumentNullException(nameof(listOptions));
            }

            if (string.IsNullOrEmpty(namespaceProperty))
            {
                throw new ArgumentNullException(nameof(namespaceProperty));
            }

            var resp = await _client.ListNamespacedCustomObjectWithHttpMessagesAsync(group: _apiGroup, plural: _resourcePlural, version: _apiVersion, resourceVersion: listOptions.ResourceVersion,
                continueParameter: listOptions.Continue, fieldSelector: listOptions.FieldSelector, labelSelector: listOptions.LabelSelector, limit: listOptions.Limit,
                timeoutSeconds: listOptions.TimeoutSeconds, namespaceParameter: namespaceProperty, cancellationToken: cancellationToken).ConfigureAwait(false);

            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        /// <summary>
        /// Create kubernetes object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="obj">the object</param>
        /// <param name="createOptions">the create options</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public async Task<T> CreateAsync<T>(T obj, CreateOptions createOptions, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (createOptions == null)
            {
                throw new ArgumentNullException(nameof(createOptions));
            }

            V1ObjectMeta objectMeta = obj.Metadata;

            var isNamespaced = !string.IsNullOrEmpty(objectMeta.NamespaceProperty);
            if (isNamespaced)
            {
                return await CreateAsync(objectMeta.NamespaceProperty, obj, createOptions, cancellationToken).ConfigureAwait(false);
            }

            var resp = await _client.CreateClusterCustomObjectWithHttpMessagesAsync(body: obj, group: _apiGroup, plural: _resourcePlural, version: _apiVersion, dryRun: createOptions.DryRun,
                fieldManager: createOptions.FieldManager, cancellationToken: cancellationToken).ConfigureAwait(false);

            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        /// <summary>
        /// Create namespaced kubernetes object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="namespaceProperty">the namespace</param>
        /// <param name="obj">the object</param>
        /// <param name="createOptions">the create options</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public async Task<T> CreateAsync<T>(string namespaceProperty, T obj, CreateOptions createOptions, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (createOptions == null)
            {
                throw new ArgumentNullException(nameof(createOptions));
            }

            var resp = await _client.CreateNamespacedCustomObjectWithHttpMessagesAsync(body: obj, group: _apiGroup, plural: _resourcePlural, version: _apiVersion,
                namespaceParameter: namespaceProperty, dryRun: createOptions.DryRun, fieldManager: createOptions.FieldManager, cancellationToken: cancellationToken).ConfigureAwait(false);

            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        /// <summary>
        /// Update kubernetes object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="obj">the object</param>
        /// <param name="updateOptions">the update options</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public async Task<T> UpdateAsync<T>(T obj, UpdateOptions updateOptions, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (updateOptions == null)
            {
                throw new ArgumentNullException(nameof(updateOptions));
            }

            V1ObjectMeta objectMeta = obj.Metadata;

            var isNamespaced = !string.IsNullOrEmpty(objectMeta.NamespaceProperty);
            HttpOperationResponse<object> resp;
            if (isNamespaced)
            {
                resp = await _client.ReplaceNamespacedCustomObjectWithHttpMessagesAsync(body: obj, name: objectMeta.Name, group: _apiGroup, plural: _resourcePlural, version: _apiVersion,
                        namespaceParameter: objectMeta.NamespaceProperty, dryRun: updateOptions.DryRun, fieldManager: updateOptions.FieldManager, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                resp = await _client.ReplaceClusterCustomObjectWithHttpMessagesAsync(body: obj, name: objectMeta.Name, group: _apiGroup ?? obj.ApiGroup(), plural: _resourcePlural,
                    version: _apiVersion, dryRun: updateOptions.DryRun, fieldManager: updateOptions.FieldManager, cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        /// <summary>
        /// Create kubernetes object, if the namespaceProperty in the object is present, it will send a
        /// namespaceProperty-scoped requests, vice versa.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="obj"> the object</param>
        /// <param name="status"> function to extract the status from the object</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public Task<T> UpdateStatusAsync<T>(T obj, Func<T, object> status, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            return UpdateStatusAsync(obj, status, new UpdateOptions(), cancellationToken);
        }

        /// <summary>
        /// Update status of kubernetes object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="obj"> the object</param>
        /// <param name="status"> function to extract the status from the object</param>
        /// <param name="updateOptions">the update options</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public async Task<T> UpdateStatusAsync<T>(T obj, Func<T, object> status, UpdateOptions updateOptions, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (updateOptions == null)
            {
                throw new ArgumentNullException(nameof(updateOptions));
            }

            V1ObjectMeta objectMeta = obj.Metadata;
            HttpOperationResponse<object> resp;
            var isNamespaced = !string.IsNullOrEmpty(objectMeta.NamespaceProperty);
            if (isNamespaced)
            {
                resp = await _client.PatchNamespacedCustomObjectStatusWithHttpMessagesAsync(body: obj, group: _apiGroup, version: _apiVersion, namespaceParameter: objectMeta.NamespaceProperty,
                    plural: _resourcePlural, name: objectMeta.Name, dryRun: updateOptions.DryRun, fieldManager: updateOptions.FieldManager, force: updateOptions.Force,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                resp = await _client.PatchClusterCustomObjectStatusWithHttpMessagesAsync(body: obj, group: _apiGroup, version: _apiVersion, plural: _resourcePlural, name: objectMeta.Name,
                    dryRun: updateOptions.DryRun, fieldManager: updateOptions.FieldManager, force: updateOptions.Force, cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        /// <summary>
        /// Patch kubernetes object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="name"> the name</param>
        /// <param name="obj"> the object</param>
        /// <param name="patchOptions">the patch options</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public async Task<T> PatchAsync<T>(string name, object obj, PatchOptions patchOptions, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (patchOptions == null)
            {
                throw new ArgumentNullException(nameof(patchOptions));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var resp = await _client.PatchClusterCustomObjectWithHttpMessagesAsync(body: obj, group: _apiGroup, version: _apiVersion, plural: _resourcePlural, name: name, dryRun: patchOptions.DryRun,
                fieldManager: patchOptions.FieldManager, force: patchOptions.Force, cancellationToken: cancellationToken).ConfigureAwait(false);
            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        /// <summary>
        /// Patch kubernetes object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="namespaceProperty"> the namespaceProperty</param>
        /// <param name="name"> the name</param>
        /// <param name="obj"> the object</param>
        /// <param name="patchOptions">the patch options</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public async Task<T> PatchAsync<T>(string namespaceProperty, string name, object obj, PatchOptions patchOptions, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            if (string.IsNullOrEmpty(namespaceProperty))
            {
                throw new ArgumentNullException(nameof(namespaceProperty));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (patchOptions == null)
            {
                throw new ArgumentNullException(nameof(patchOptions));
            }

            var resp = await _client.PatchNamespacedCustomObjectWithHttpMessagesAsync(body: obj, group: _apiGroup, version: _apiVersion, namespaceParameter: namespaceProperty, plural: _resourcePlural,
                name: name, dryRun: patchOptions.DryRun, fieldManager: patchOptions.FieldManager, force: patchOptions.Force, cancellationToken: cancellationToken).ConfigureAwait(false);
            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        /// <summary>
        /// Delete kubernetes object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="name"> the name</param>
        /// <param name="deleteOptions">the delete options</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public async Task<T> DeleteAsync<T>(string name, V1DeleteOptions deleteOptions, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var resp = await _client.DeleteClusterCustomObjectWithHttpMessagesAsync(
                group: _apiGroup, version: _apiVersion, plural: _resourcePlural, name: name, body: deleteOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        /// <summary>
        /// Delete kubernetes object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="namespaceProperty"> the namespaceProperty</param>
        /// <param name="name"> the name</param>
        /// <param name="deleteOptions">the delete options</param>
        /// <param name="cancellationToken">the token </param>
        /// <returns>the kubernetes object</returns>
        public async Task<T> DeleteAsync<T>(string namespaceProperty, string name, V1DeleteOptions deleteOptions, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            if (string.IsNullOrEmpty(namespaceProperty))
            {
                throw new ArgumentNullException(nameof(namespaceProperty));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var resp = await _client.DeleteNamespacedCustomObjectWithHttpMessagesAsync(group: _apiGroup, version: _apiVersion, namespaceParameter: namespaceProperty, plural: _resourcePlural,
                name: name, body: deleteOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
            return SafeJsonConvert.DeserializeObject<T>(resp.Body.ToString());
        }

        /// <summary>
        /// Watch watchable.
        /// </summary>
        /// <param name="listOptions">the list options</param>
        /// <param name="onEvent">action on event</param>
        /// <param name="onError">action on error</param>
        /// <param name="onClosed">action on closed</param>
        /// <param name="cancellationToken">the token </param>
        /// <typeparam name="T">the object type</typeparam>
        /// <returns>the watchable</returns>
        public Watcher<T> Watch<T>(ListOptions listOptions, Action<WatchEventType, T> onEvent, Action<Exception> onError = default, Action onClosed = default,
            CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            if (listOptions == null)
            {
                throw new ArgumentNullException(nameof(listOptions));
            }

            var resp = _client.ListClusterCustomObjectWithHttpMessagesAsync(group: _apiGroup, version: _apiVersion, plural: _resourcePlural, continueParameter: listOptions.Continue,
                fieldSelector: listOptions.FieldSelector, labelSelector: listOptions.LabelSelector, limit: listOptions.Limit, resourceVersion: listOptions.ResourceVersion,
                timeoutSeconds: listOptions.TimeoutSeconds, watch: true, cancellationToken: cancellationToken);

            return resp.Watch(onEvent, onError, onClosed);
        }

        /// <summary>
        /// Watch watchable.
        /// </summary>
        /// <param name="namespaceProperty"> the namespaceProperty</param>
        /// <param name="listOptions">the list options</param>
        /// <param name="onEvent">action on event</param>
        /// <param name="onError">action on error</param>
        /// <param name="onClosed">action on closed</param>
        /// <param name="cancellationToken">the token </param>
        /// <typeparam name="T">the object type</typeparam>
        /// <returns>the watchable</returns>
        public Watcher<T> Watch<T>(string namespaceProperty, ListOptions listOptions, Action<WatchEventType, T> onEvent, Action<Exception> onError = default, Action onClosed = default,
            CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject<V1ObjectMeta>
        {
            if (listOptions == null)
            {
                throw new ArgumentNullException(nameof(listOptions));
            }

            if (string.IsNullOrEmpty(namespaceProperty))
            {
                throw new ArgumentNullException(nameof(namespaceProperty));
            }

            var resp = _client.ListNamespacedCustomObjectWithHttpMessagesAsync(group: _apiGroup, version: _apiVersion, namespaceParameter: namespaceProperty, plural: _resourcePlural,
                continueParameter: listOptions.Continue, fieldSelector: listOptions.FieldSelector, labelSelector: listOptions.LabelSelector, limit: listOptions.Limit,
                resourceVersion: listOptions.ResourceVersion, timeoutSeconds: listOptions.TimeoutSeconds, watch: true, cancellationToken: cancellationToken);

            return resp.Watch(onEvent, onError, onClosed);
        }
    }
}
