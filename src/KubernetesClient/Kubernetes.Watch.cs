using k8s.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    public partial class Kubernetes
    {
        /// <inheritdoc/>
        public Task<Watcher<V1ConfigMap>> WatchNamespacedConfigMapAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1ConfigMap> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WatchNamespacedObjectAsync<V1ConfigMap>("configmaps", name, @namespace, @continue, fieldSelector, includeUninitialized, labelSelector, limit, pretty, timeoutSeconds, resourceVersion, customHeaders, onEvent, onError, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Watcher<V1Endpoints>> WatchNamespacedEndpointAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1Endpoints> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WatchNamespacedObjectAsync<V1Endpoints>("endpoints", name, @namespace, @continue, fieldSelector, includeUninitialized, labelSelector, limit, pretty, timeoutSeconds, resourceVersion, customHeaders, onEvent, onError, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Watcher<V1Event>> WatchNamespacedEventAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1Event> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WatchNamespacedObjectAsync<V1Event>("events", name, @namespace, @continue, fieldSelector, includeUninitialized, labelSelector, limit, pretty, timeoutSeconds, resourceVersion, customHeaders, onEvent, onError, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Watcher<V1LimitRange>> WatchNamespacedLimitRangeAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1LimitRange> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WatchNamespacedObjectAsync<V1LimitRange>("limitranges", name, @namespace, @continue, fieldSelector, includeUninitialized, labelSelector, limit, pretty, timeoutSeconds, resourceVersion, customHeaders, onEvent, onError, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Watcher<V1Namespace>> WatchNamespaceAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1Namespace> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WatchObjectAsync<V1Namespace>("namespaces", name, @namespace, @continue, fieldSelector, includeUninitialized, labelSelector, limit, pretty, timeoutSeconds, resourceVersion, customHeaders, onEvent, onError, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Watcher<V1Node>> WatchNodeAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1Node> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WatchObjectAsync<V1Node>("nodes", name, @namespace, @continue, fieldSelector, includeUninitialized, labelSelector, limit, pretty, timeoutSeconds, resourceVersion, customHeaders, onEvent, onError, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Watcher<V1PersistentVolumeClaim>> WatchNamespacedPersistentVolumeClaimAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1PersistentVolumeClaim> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WatchNamespacedObjectAsync<V1PersistentVolumeClaim>("persistentvolumeclaims", name, @namespace, @continue, fieldSelector, includeUninitialized, labelSelector, limit, pretty, timeoutSeconds, resourceVersion, customHeaders, onEvent, onError, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Watcher<V1PersistentVolume>> WatchPersistentVolumeAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1PersistentVolume> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WatchObjectAsync<V1PersistentVolume>("persistentvolumes", name, @namespace, @continue, fieldSelector, includeUninitialized, labelSelector, limit, pretty, timeoutSeconds, resourceVersion, customHeaders, onEvent, onError, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Watcher<V1Pod>> WatchNamespacedPodAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1Pod> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WatchNamespacedObjectAsync<V1Pod>("pods", name, @namespace, @continue, fieldSelector, includeUninitialized, labelSelector, limit, pretty, timeoutSeconds, resourceVersion, customHeaders, onEvent, onError, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Watcher<V1PodTemplate>> WatchNamespacedPodTemplateAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1PodTemplate> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WatchNamespacedObjectAsync<V1PodTemplate>("podtemplates", name, @namespace, @continue, fieldSelector, includeUninitialized, labelSelector, limit, pretty, timeoutSeconds, resourceVersion, customHeaders, onEvent, onError, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Watcher<V1ReplicationController>> WatchNamespacedReplicationControllerAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1ReplicationController> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WatchNamespacedObjectAsync<V1ReplicationController>("replicationcontrollers", name, @namespace, @continue, fieldSelector, includeUninitialized, labelSelector, limit, pretty, timeoutSeconds, resourceVersion, customHeaders, onEvent, onError, cancellationToken);
        }
        
        public Task<Watcher<T>> WatchNamespacedObjectAsync<T>(string resourceType, string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, T> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (@namespace == null)
            {
                throw new ArgumentNullException(nameof(@namespace));
            }

            return WatchObjectAsync<T>(resourceType, name, @namespace, @continue, fieldSelector, includeUninitialized, labelSelector, limit, pretty, timeoutSeconds, resourceVersion, customHeaders, onEvent, onError, cancellationToken);
        }

        public async Task<Watcher<T>> WatchObjectAsync<T>(string resourceType, string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, T> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("name", name);
                tracingParameters.Add("namespace", @namespace);
                tracingParameters.Add("continue", @continue);
                tracingParameters.Add("fieldSelector", fieldSelector);
                tracingParameters.Add("includeUninitialized", includeUninitialized);
                tracingParameters.Add("labelSelector", labelSelector);
                tracingParameters.Add("limit", limit);
                tracingParameters.Add("pretty", pretty);
                tracingParameters.Add("timeoutSeconds", timeoutSeconds);
                tracingParameters.Add("resourceVersion", resourceVersion);
                ServiceClientTracing.Enter(_invocationId, this, nameof(WatchNamespacedPodAsync), tracingParameters);
            }

            // Construct URL
            var uriBuilder = new UriBuilder(this.BaseUri);
            if (!uriBuilder.Path.EndsWith("/"))
            {
                uriBuilder.Path += "/";
            }

            uriBuilder.Path += "api/v1/watch";

            if (@namespace != null)
            {
                uriBuilder.Path += $"/namespaces/{@namespace}";
            }

            uriBuilder.Path += $"/{resourceType}/{name}";

            var query = string.Empty;

            // Don't sent watch, because setting that value will cause the WatcherDelegatingHandler to kick in. That class
            // "eats" the first line, which is something we don't want.
            // query = QueryHelpers.AddQueryString(query, "watch", "true");

            if (@continue != null)
            {
                query = QueryHelpers.AddQueryString(query, "continue", Uri.EscapeDataString(@continue));
            }

            if (fieldSelector != null)
            {
                query = QueryHelpers.AddQueryString(query, "fieldSelector", Uri.EscapeDataString(fieldSelector));
            }

            if (includeUninitialized != null)
            {
                query = QueryHelpers.AddQueryString(query, "includeUninitialized", includeUninitialized.Value ? "true" : "false");
            }

            if (labelSelector != null)
            {
                query = QueryHelpers.AddQueryString(query, "labelSelector", Uri.EscapeDataString(labelSelector));
            }

            if (limit != null)
            {
                query = QueryHelpers.AddQueryString(query, "limit", limit.Value.ToString());
            }

            if (pretty != null)
            {
                query = QueryHelpers.AddQueryString(query, "pretty", pretty.Value ? "true" : "false");
            }

            if (timeoutSeconds != null)
            {
                query = QueryHelpers.AddQueryString(query, "timeoutSeconds", timeoutSeconds.Value.ToString());
            }

            if (resourceVersion != null)
            {
                query = QueryHelpers.AddQueryString(query, "resourceVersion", resourceVersion.Value.ToString());
            }

            uriBuilder.Query = query;

            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.ToString());

            // Set Credentials
            if (this.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await this.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }

            // Set Headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    if (httpRequest.Headers.Contains(header.Key))
                    {
                        httpRequest.Headers.Remove(header.Key);
                    }

                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Send Request
            if (_shouldTrace)
            {
                ServiceClientTracing.SendRequest(_invocationId, httpRequest);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var httpResponse = await HttpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

            if (_shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(_invocationId, httpResponse);
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                string responseContent = string.Empty;

                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", httpResponse.StatusCode));
                if (httpResponse.Content != null)
                {
                    responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                ex.Request = new HttpRequestMessageWrapper(httpRequest, responseContent);
                ex.Response = new HttpResponseMessageWrapper(httpResponse, string.Empty);

                httpRequest.Dispose();
                httpResponse?.Dispose();
                throw ex;
            }

            var stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            StreamReader reader = new StreamReader(stream);

            return new Watcher<T>(reader, onEvent, onError);
        }
    }
}
