using k8s.Models;
using System; 
using System.Collections.Generic; 
using System.Threading; 
using System.Threading.Tasks; 

namespace k8s
{
    public partial class Kubernetes
    {
        /// <inheritdoc>
        public Task<Watcher<V1ConfigMap>> WatchNamespacedConfigMapAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1ConfigMap> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/namespaces/{@namespace}/configmaps/{name}";
            return WatchObjectAsync<V1ConfigMap>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1Endpoints>> WatchNamespacedEndpointsAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1Endpoints> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/namespaces/{@namespace}/endpoints/{name}";
            return WatchObjectAsync<V1Endpoints>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1Event>> WatchNamespacedEventAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1Event> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/namespaces/{@namespace}/events/{name}";
            return WatchObjectAsync<V1Event>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1LimitRange>> WatchNamespacedLimitRangeAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1LimitRange> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/namespaces/{@namespace}/limitranges/{name}";
            return WatchObjectAsync<V1LimitRange>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1PersistentVolumeClaim>> WatchNamespacedPersistentVolumeClaimAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1PersistentVolumeClaim> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/namespaces/{@namespace}/persistentvolumeclaims/{name}";
            return WatchObjectAsync<V1PersistentVolumeClaim>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1Pod>> WatchNamespacedPodAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1Pod> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/namespaces/{@namespace}/pods/{name}";
            return WatchObjectAsync<V1Pod>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1PodTemplate>> WatchNamespacedPodTemplateAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1PodTemplate> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/namespaces/{@namespace}/podtemplates/{name}";
            return WatchObjectAsync<V1PodTemplate>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1ReplicationController>> WatchNamespacedReplicationControllerAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1ReplicationController> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/namespaces/{@namespace}/replicationcontrollers/{name}";
            return WatchObjectAsync<V1ReplicationController>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1ResourceQuota>> WatchNamespacedResourceQuotaAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1ResourceQuota> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/namespaces/{@namespace}/resourcequotas/{name}";
            return WatchObjectAsync<V1ResourceQuota>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1Secret>> WatchNamespacedSecretAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1Secret> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/namespaces/{@namespace}/secrets/{name}";
            return WatchObjectAsync<V1Secret>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1ServiceAccount>> WatchNamespacedServiceAccountAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1ServiceAccount> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/namespaces/{@namespace}/serviceaccounts/{name}";
            return WatchObjectAsync<V1ServiceAccount>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1Service>> WatchNamespacedServiceAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1Service> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/namespaces/{@namespace}/services/{name}";
            return WatchObjectAsync<V1Service>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1Namespace>> WatchNamespaceAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1Namespace> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/namespaces/{name}";
            return WatchObjectAsync<V1Namespace>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1Node>> WatchNodeAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1Node> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/nodes/{name}";
            return WatchObjectAsync<V1Node>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1PersistentVolume>> WatchPersistentVolumeAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1PersistentVolume> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"api/v1/watch/persistentvolumes/{name}";
            return WatchObjectAsync<V1PersistentVolume>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1alpha1InitializerConfiguration>> WatchInitializerConfigurationAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1alpha1InitializerConfiguration> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/admissionregistration.k8s.io/v1alpha1/watch/initializerconfigurations/{name}";
            return WatchObjectAsync<V1alpha1InitializerConfiguration>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1MutatingWebhookConfiguration>> WatchMutatingWebhookConfigurationAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1MutatingWebhookConfiguration> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/admissionregistration.k8s.io/v1beta1/watch/mutatingwebhookconfigurations/{name}";
            return WatchObjectAsync<V1beta1MutatingWebhookConfiguration>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1ValidatingWebhookConfiguration>> WatchValidatingWebhookConfigurationAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1ValidatingWebhookConfiguration> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/admissionregistration.k8s.io/v1beta1/watch/validatingwebhookconfigurations/{name}";
            return WatchObjectAsync<V1beta1ValidatingWebhookConfiguration>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1CustomResourceDefinition>> WatchCustomResourceDefinitionAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1CustomResourceDefinition> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apiextensions.k8s.io/v1beta1/watch/customresourcedefinitions/{name}";
            return WatchObjectAsync<V1beta1CustomResourceDefinition>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1APIService>> WatchAPIServiceAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1APIService> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apiregistration.k8s.io/v1/watch/apiservices/{name}";
            return WatchObjectAsync<V1APIService>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1APIService>> WatchAPIServiceAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1APIService> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apiregistration.k8s.io/v1beta1/watch/apiservices/{name}";
            return WatchObjectAsync<V1beta1APIService>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1ControllerRevision>> WatchNamespacedControllerRevisionAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1ControllerRevision> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apps/v1/watch/namespaces/{@namespace}/controllerrevisions/{name}";
            return WatchObjectAsync<V1ControllerRevision>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1DaemonSet>> WatchNamespacedDaemonSetAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1DaemonSet> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apps/v1/watch/namespaces/{@namespace}/daemonsets/{name}";
            return WatchObjectAsync<V1DaemonSet>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1Deployment>> WatchNamespacedDeploymentAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1Deployment> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apps/v1/watch/namespaces/{@namespace}/deployments/{name}";
            return WatchObjectAsync<V1Deployment>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1ReplicaSet>> WatchNamespacedReplicaSetAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1ReplicaSet> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apps/v1/watch/namespaces/{@namespace}/replicasets/{name}";
            return WatchObjectAsync<V1ReplicaSet>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1StatefulSet>> WatchNamespacedStatefulSetAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1StatefulSet> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apps/v1/watch/namespaces/{@namespace}/statefulsets/{name}";
            return WatchObjectAsync<V1StatefulSet>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1ControllerRevision>> WatchNamespacedControllerRevisionAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1ControllerRevision> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apps/v1beta1/watch/namespaces/{@namespace}/controllerrevisions/{name}";
            return WatchObjectAsync<V1beta1ControllerRevision>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1StatefulSet>> WatchNamespacedStatefulSetAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1StatefulSet> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apps/v1beta1/watch/namespaces/{@namespace}/statefulsets/{name}";
            return WatchObjectAsync<V1beta1StatefulSet>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta2ControllerRevision>> WatchNamespacedControllerRevisionAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta2ControllerRevision> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apps/v1beta2/watch/namespaces/{@namespace}/controllerrevisions/{name}";
            return WatchObjectAsync<V1beta2ControllerRevision>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta2DaemonSet>> WatchNamespacedDaemonSetAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta2DaemonSet> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apps/v1beta2/watch/namespaces/{@namespace}/daemonsets/{name}";
            return WatchObjectAsync<V1beta2DaemonSet>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta2ReplicaSet>> WatchNamespacedReplicaSetAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta2ReplicaSet> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apps/v1beta2/watch/namespaces/{@namespace}/replicasets/{name}";
            return WatchObjectAsync<V1beta2ReplicaSet>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta2StatefulSet>> WatchNamespacedStatefulSetAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta2StatefulSet> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/apps/v1beta2/watch/namespaces/{@namespace}/statefulsets/{name}";
            return WatchObjectAsync<V1beta2StatefulSet>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1HorizontalPodAutoscaler>> WatchNamespacedHorizontalPodAutoscalerAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1HorizontalPodAutoscaler> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/autoscaling/v1/watch/namespaces/{@namespace}/horizontalpodautoscalers/{name}";
            return WatchObjectAsync<V1HorizontalPodAutoscaler>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V2beta1HorizontalPodAutoscaler>> WatchNamespacedHorizontalPodAutoscalerAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V2beta1HorizontalPodAutoscaler> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/autoscaling/v2beta1/watch/namespaces/{@namespace}/horizontalpodautoscalers/{name}";
            return WatchObjectAsync<V2beta1HorizontalPodAutoscaler>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1Job>> WatchNamespacedJobAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1Job> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/batch/v1/watch/namespaces/{@namespace}/jobs/{name}";
            return WatchObjectAsync<V1Job>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1CronJob>> WatchNamespacedCronJobAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1CronJob> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/batch/v1beta1/watch/namespaces/{@namespace}/cronjobs/{name}";
            return WatchObjectAsync<V1beta1CronJob>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V2alpha1CronJob>> WatchNamespacedCronJobAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V2alpha1CronJob> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/batch/v2alpha1/watch/namespaces/{@namespace}/cronjobs/{name}";
            return WatchObjectAsync<V2alpha1CronJob>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1CertificateSigningRequest>> WatchCertificateSigningRequestAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1CertificateSigningRequest> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/certificates.k8s.io/v1beta1/watch/certificatesigningrequests/{name}";
            return WatchObjectAsync<V1beta1CertificateSigningRequest>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1Event>> WatchNamespacedEventAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1Event> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/events.k8s.io/v1beta1/watch/namespaces/{@namespace}/events/{name}";
            return WatchObjectAsync<V1beta1Event>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1DaemonSet>> WatchNamespacedDaemonSetAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1DaemonSet> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/extensions/v1beta1/watch/namespaces/{@namespace}/daemonsets/{name}";
            return WatchObjectAsync<V1beta1DaemonSet>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1Ingress>> WatchNamespacedIngressAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1Ingress> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/extensions/v1beta1/watch/namespaces/{@namespace}/ingresses/{name}";
            return WatchObjectAsync<V1beta1Ingress>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1ReplicaSet>> WatchNamespacedReplicaSetAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1ReplicaSet> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/extensions/v1beta1/watch/namespaces/{@namespace}/replicasets/{name}";
            return WatchObjectAsync<V1beta1ReplicaSet>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1NetworkPolicy>> WatchNamespacedNetworkPolicyAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1NetworkPolicy> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/networking.k8s.io/v1/watch/namespaces/{@namespace}/networkpolicies/{name}";
            return WatchObjectAsync<V1NetworkPolicy>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1PodDisruptionBudget>> WatchNamespacedPodDisruptionBudgetAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1PodDisruptionBudget> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/policy/v1beta1/watch/namespaces/{@namespace}/poddisruptionbudgets/{name}";
            return WatchObjectAsync<V1beta1PodDisruptionBudget>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1ClusterRoleBinding>> WatchClusterRoleBindingAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1ClusterRoleBinding> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/rbac.authorization.k8s.io/v1/watch/clusterrolebindings/{name}";
            return WatchObjectAsync<V1ClusterRoleBinding>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1ClusterRole>> WatchClusterRoleAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1ClusterRole> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/rbac.authorization.k8s.io/v1/watch/clusterroles/{name}";
            return WatchObjectAsync<V1ClusterRole>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1RoleBinding>> WatchNamespacedRoleBindingAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1RoleBinding> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/rbac.authorization.k8s.io/v1/watch/namespaces/{@namespace}/rolebindings/{name}";
            return WatchObjectAsync<V1RoleBinding>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1Role>> WatchNamespacedRoleAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1Role> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/rbac.authorization.k8s.io/v1/watch/namespaces/{@namespace}/roles/{name}";
            return WatchObjectAsync<V1Role>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1alpha1ClusterRoleBinding>> WatchClusterRoleBindingAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1alpha1ClusterRoleBinding> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/rbac.authorization.k8s.io/v1alpha1/watch/clusterrolebindings/{name}";
            return WatchObjectAsync<V1alpha1ClusterRoleBinding>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1alpha1ClusterRole>> WatchClusterRoleAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1alpha1ClusterRole> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/rbac.authorization.k8s.io/v1alpha1/watch/clusterroles/{name}";
            return WatchObjectAsync<V1alpha1ClusterRole>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1alpha1RoleBinding>> WatchNamespacedRoleBindingAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1alpha1RoleBinding> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/rbac.authorization.k8s.io/v1alpha1/watch/namespaces/{@namespace}/rolebindings/{name}";
            return WatchObjectAsync<V1alpha1RoleBinding>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1alpha1Role>> WatchNamespacedRoleAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1alpha1Role> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/rbac.authorization.k8s.io/v1alpha1/watch/namespaces/{@namespace}/roles/{name}";
            return WatchObjectAsync<V1alpha1Role>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1ClusterRoleBinding>> WatchClusterRoleBindingAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1ClusterRoleBinding> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/rbac.authorization.k8s.io/v1beta1/watch/clusterrolebindings/{name}";
            return WatchObjectAsync<V1beta1ClusterRoleBinding>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1ClusterRole>> WatchClusterRoleAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1ClusterRole> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/rbac.authorization.k8s.io/v1beta1/watch/clusterroles/{name}";
            return WatchObjectAsync<V1beta1ClusterRole>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1RoleBinding>> WatchNamespacedRoleBindingAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1RoleBinding> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/rbac.authorization.k8s.io/v1beta1/watch/namespaces/{@namespace}/rolebindings/{name}";
            return WatchObjectAsync<V1beta1RoleBinding>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1Role>> WatchNamespacedRoleAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1Role> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/rbac.authorization.k8s.io/v1beta1/watch/namespaces/{@namespace}/roles/{name}";
            return WatchObjectAsync<V1beta1Role>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1alpha1PriorityClass>> WatchPriorityClassAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1alpha1PriorityClass> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/scheduling.k8s.io/v1alpha1/watch/priorityclasses/{name}";
            return WatchObjectAsync<V1alpha1PriorityClass>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1alpha1PodPreset>> WatchNamespacedPodPresetAsync(
            string name,
            string @namespace,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1alpha1PodPreset> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/settings.k8s.io/v1alpha1/watch/namespaces/{@namespace}/podpresets/{name}";
            return WatchObjectAsync<V1alpha1PodPreset>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1StorageClass>> WatchStorageClassAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1StorageClass> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/storage.k8s.io/v1/watch/storageclasses/{name}";
            return WatchObjectAsync<V1StorageClass>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1alpha1VolumeAttachment>> WatchVolumeAttachmentAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1alpha1VolumeAttachment> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/storage.k8s.io/v1alpha1/watch/volumeattachments/{name}";
            return WatchObjectAsync<V1alpha1VolumeAttachment>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1StorageClass>> WatchStorageClassAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1StorageClass> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/storage.k8s.io/v1beta1/watch/storageclasses/{name}";
            return WatchObjectAsync<V1beta1StorageClass>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

        /// <inheritdoc>
        public Task<Watcher<V1beta1VolumeAttachment>> WatchVolumeAttachmentAsync(
            string name,
            string @continue = null,
            string fieldSelector = null,
            bool? includeUninitialized = null,
            string labelSelector = null,
            int? limit = null,
            bool? pretty = null,
            string resourceVersion = null,
            int? timeoutSeconds = null,
            bool? watch = null,
            Dictionary<string, List<string>> customHeaders = null,
            Action<WatchEventType, V1beta1VolumeAttachment> onEvent = null,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"apis/storage.k8s.io/v1beta1/watch/volumeattachments/{name}";
            return WatchObjectAsync<V1beta1VolumeAttachment>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, onClosed: onClosed, cancellationToken: cancellationToken);
        }

    }
}
