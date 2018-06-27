using k8s.Models;
using System; 
using System.Collections.Generic; 
using System.Threading; 
using System.Threading.Tasks; 

namespace k8s
{
    public partial interface IKubernetes
    {
        /// <summary>
        /// watch changes to an object of kind ConfigMap
        /// </summary>
        /// <param name="name">
        /// name of the ConfigMap
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1ConfigMap>> WatchNamespacedConfigMapAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Endpoints
        /// </summary>
        /// <param name="name">
        /// name of the Endpoints
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Endpoints>> WatchNamespacedEndpointsAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Event
        /// </summary>
        /// <param name="name">
        /// name of the Event
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Event>> WatchNamespacedEventAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind LimitRange
        /// </summary>
        /// <param name="name">
        /// name of the LimitRange
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1LimitRange>> WatchNamespacedLimitRangeAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind PersistentVolumeClaim
        /// </summary>
        /// <param name="name">
        /// name of the PersistentVolumeClaim
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1PersistentVolumeClaim>> WatchNamespacedPersistentVolumeClaimAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Pod
        /// </summary>
        /// <param name="name">
        /// name of the Pod
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Pod>> WatchNamespacedPodAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind PodTemplate
        /// </summary>
        /// <param name="name">
        /// name of the PodTemplate
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1PodTemplate>> WatchNamespacedPodTemplateAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ReplicationController
        /// </summary>
        /// <param name="name">
        /// name of the ReplicationController
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1ReplicationController>> WatchNamespacedReplicationControllerAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ResourceQuota
        /// </summary>
        /// <param name="name">
        /// name of the ResourceQuota
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1ResourceQuota>> WatchNamespacedResourceQuotaAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Secret
        /// </summary>
        /// <param name="name">
        /// name of the Secret
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Secret>> WatchNamespacedSecretAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ServiceAccount
        /// </summary>
        /// <param name="name">
        /// name of the ServiceAccount
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1ServiceAccount>> WatchNamespacedServiceAccountAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Service
        /// </summary>
        /// <param name="name">
        /// name of the Service
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Service>> WatchNamespacedServiceAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Namespace
        /// </summary>
        /// <param name="name">
        /// name of the Namespace
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Namespace>> WatchNamespaceAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Node
        /// </summary>
        /// <param name="name">
        /// name of the Node
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Node>> WatchNodeAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind PersistentVolume
        /// </summary>
        /// <param name="name">
        /// name of the PersistentVolume
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1PersistentVolume>> WatchPersistentVolumeAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind InitializerConfiguration
        /// </summary>
        /// <param name="name">
        /// name of the InitializerConfiguration
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1alpha1InitializerConfiguration>> WatchInitializerConfigurationAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind MutatingWebhookConfiguration
        /// </summary>
        /// <param name="name">
        /// name of the MutatingWebhookConfiguration
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1MutatingWebhookConfiguration>> WatchMutatingWebhookConfigurationAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ValidatingWebhookConfiguration
        /// </summary>
        /// <param name="name">
        /// name of the ValidatingWebhookConfiguration
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1ValidatingWebhookConfiguration>> WatchValidatingWebhookConfigurationAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind CustomResourceDefinition
        /// </summary>
        /// <param name="name">
        /// name of the CustomResourceDefinition
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1CustomResourceDefinition>> WatchCustomResourceDefinitionAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind APIService
        /// </summary>
        /// <param name="name">
        /// name of the APIService
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1APIService>> WatchAPIServiceAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind APIService
        /// </summary>
        /// <param name="name">
        /// name of the APIService
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1APIService>> WatchAPIServiceAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ControllerRevision
        /// </summary>
        /// <param name="name">
        /// name of the ControllerRevision
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1ControllerRevision>> WatchNamespacedControllerRevisionAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind DaemonSet
        /// </summary>
        /// <param name="name">
        /// name of the DaemonSet
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1DaemonSet>> WatchNamespacedDaemonSetAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Deployment
        /// </summary>
        /// <param name="name">
        /// name of the Deployment
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Deployment>> WatchNamespacedDeploymentAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ReplicaSet
        /// </summary>
        /// <param name="name">
        /// name of the ReplicaSet
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1ReplicaSet>> WatchNamespacedReplicaSetAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind StatefulSet
        /// </summary>
        /// <param name="name">
        /// name of the StatefulSet
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1StatefulSet>> WatchNamespacedStatefulSetAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ControllerRevision
        /// </summary>
        /// <param name="name">
        /// name of the ControllerRevision
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1ControllerRevision>> WatchNamespacedControllerRevisionAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind StatefulSet
        /// </summary>
        /// <param name="name">
        /// name of the StatefulSet
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1StatefulSet>> WatchNamespacedStatefulSetAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ControllerRevision
        /// </summary>
        /// <param name="name">
        /// name of the ControllerRevision
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta2ControllerRevision>> WatchNamespacedControllerRevisionAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind DaemonSet
        /// </summary>
        /// <param name="name">
        /// name of the DaemonSet
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta2DaemonSet>> WatchNamespacedDaemonSetAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ReplicaSet
        /// </summary>
        /// <param name="name">
        /// name of the ReplicaSet
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta2ReplicaSet>> WatchNamespacedReplicaSetAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind StatefulSet
        /// </summary>
        /// <param name="name">
        /// name of the StatefulSet
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta2StatefulSet>> WatchNamespacedStatefulSetAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind HorizontalPodAutoscaler
        /// </summary>
        /// <param name="name">
        /// name of the HorizontalPodAutoscaler
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1HorizontalPodAutoscaler>> WatchNamespacedHorizontalPodAutoscalerAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind HorizontalPodAutoscaler
        /// </summary>
        /// <param name="name">
        /// name of the HorizontalPodAutoscaler
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V2beta1HorizontalPodAutoscaler>> WatchNamespacedHorizontalPodAutoscalerAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Job
        /// </summary>
        /// <param name="name">
        /// name of the Job
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Job>> WatchNamespacedJobAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind CronJob
        /// </summary>
        /// <param name="name">
        /// name of the CronJob
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1CronJob>> WatchNamespacedCronJobAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind CronJob
        /// </summary>
        /// <param name="name">
        /// name of the CronJob
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V2alpha1CronJob>> WatchNamespacedCronJobAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind CertificateSigningRequest
        /// </summary>
        /// <param name="name">
        /// name of the CertificateSigningRequest
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1CertificateSigningRequest>> WatchCertificateSigningRequestAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Event
        /// </summary>
        /// <param name="name">
        /// name of the Event
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1Event>> WatchNamespacedEventAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind DaemonSet
        /// </summary>
        /// <param name="name">
        /// name of the DaemonSet
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1DaemonSet>> WatchNamespacedDaemonSetAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Ingress
        /// </summary>
        /// <param name="name">
        /// name of the Ingress
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1Ingress>> WatchNamespacedIngressAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ReplicaSet
        /// </summary>
        /// <param name="name">
        /// name of the ReplicaSet
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1ReplicaSet>> WatchNamespacedReplicaSetAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind NetworkPolicy
        /// </summary>
        /// <param name="name">
        /// name of the NetworkPolicy
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1NetworkPolicy>> WatchNamespacedNetworkPolicyAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind PodDisruptionBudget
        /// </summary>
        /// <param name="name">
        /// name of the PodDisruptionBudget
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1PodDisruptionBudget>> WatchNamespacedPodDisruptionBudgetAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ClusterRoleBinding
        /// </summary>
        /// <param name="name">
        /// name of the ClusterRoleBinding
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1ClusterRoleBinding>> WatchClusterRoleBindingAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ClusterRole
        /// </summary>
        /// <param name="name">
        /// name of the ClusterRole
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1ClusterRole>> WatchClusterRoleAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind RoleBinding
        /// </summary>
        /// <param name="name">
        /// name of the RoleBinding
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1RoleBinding>> WatchNamespacedRoleBindingAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Role
        /// </summary>
        /// <param name="name">
        /// name of the Role
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Role>> WatchNamespacedRoleAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ClusterRoleBinding
        /// </summary>
        /// <param name="name">
        /// name of the ClusterRoleBinding
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1alpha1ClusterRoleBinding>> WatchClusterRoleBindingAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ClusterRole
        /// </summary>
        /// <param name="name">
        /// name of the ClusterRole
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1alpha1ClusterRole>> WatchClusterRoleAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind RoleBinding
        /// </summary>
        /// <param name="name">
        /// name of the RoleBinding
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1alpha1RoleBinding>> WatchNamespacedRoleBindingAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Role
        /// </summary>
        /// <param name="name">
        /// name of the Role
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1alpha1Role>> WatchNamespacedRoleAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ClusterRoleBinding
        /// </summary>
        /// <param name="name">
        /// name of the ClusterRoleBinding
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1ClusterRoleBinding>> WatchClusterRoleBindingAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind ClusterRole
        /// </summary>
        /// <param name="name">
        /// name of the ClusterRole
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1ClusterRole>> WatchClusterRoleAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind RoleBinding
        /// </summary>
        /// <param name="name">
        /// name of the RoleBinding
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1RoleBinding>> WatchNamespacedRoleBindingAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind Role
        /// </summary>
        /// <param name="name">
        /// name of the Role
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1Role>> WatchNamespacedRoleAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind PriorityClass
        /// </summary>
        /// <param name="name">
        /// name of the PriorityClass
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1alpha1PriorityClass>> WatchPriorityClassAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind PodPreset
        /// </summary>
        /// <param name="name">
        /// name of the PodPreset
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1alpha1PodPreset>> WatchNamespacedPodPresetAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind StorageClass
        /// </summary>
        /// <param name="name">
        /// name of the StorageClass
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1StorageClass>> WatchStorageClassAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind VolumeAttachment
        /// </summary>
        /// <param name="name">
        /// name of the VolumeAttachment
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1alpha1VolumeAttachment>> WatchVolumeAttachmentAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind StorageClass
        /// </summary>
        /// <param name="name">
        /// name of the StorageClass
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1StorageClass>> WatchStorageClassAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// watch changes to an object of kind VolumeAttachment
        /// </summary>
        /// <param name="name">
        /// name of the VolumeAttachment
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server. Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
        /// </param>
        /// <param name="fieldSelector">
        /// A selector to restrict the list of returned objects by their fields. Defaults to everything.
        /// </param>
        /// <param name="includeUninitialized">
        /// If true, partially initialized resources are included in the response.
        /// </param>
        /// <param name="labelSelector">
        /// A selector to restrict the list of returned objects by their labels. Defaults to everything.
        /// </param>
        /// <param name="limit">
        /// limit is a maximum number of responses to return for a list call. If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results. Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results. If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// 
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object that was present at the time the first list result was calculated is returned.
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history. When specified for list: - if unset, then the result is returned from remote storage based on quorum-read flag; - if it's 0, then we simply return what we currently have in cache, no guarantee; - if set to non zero, then the result is at least as fresh as given rv.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="watch">
        /// Watch for changes to the described resources and return them as a stream of add, update, and remove notifications. Specify resourceVersion.
        /// </param>
        /// <param name="customHeaders">
        /// The headers that will be added to request.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1beta1VolumeAttachment>> WatchVolumeAttachmentAsync(
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
            CancellationToken cancellationToken = default(CancellationToken));

    }
}
