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
        /// Watch changes to an object of kind ConfigMap
        /// </summary>
        /// <param name="name">
        /// name of the ConfigMap
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server.
        /// Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters
        /// (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is
        /// no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will
        /// respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not
        /// supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
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
        /// limit is a maximum number of responses to return for a list call.
        /// If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results.
        /// Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the
        /// presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results.
        /// If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or
        /// deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client
        /// that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object
        /// that was present at the time the first list result was calculated is returned
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history.
        /// When specified for list:
        /// - if unset, then the result is returned from remote storage based on quorum-read flag;
        /// - if it's 0, then we simply return what we currently have in cache, no guarantee;
        /// - if set to non zero, then the result is at least as fresh as given rv.
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
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1ConfigMap>> WatchNamespacedConfigMapAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1ConfigMap> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Watch changes to an object of kind Endpoints
        /// </summary>
        /// <param name="name">
        /// name of the ConfigMap
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server.
        /// Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters
        /// (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is
        /// no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will
        /// respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not
        /// supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
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
        /// limit is a maximum number of responses to return for a list call.
        /// If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results.
        /// Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the
        /// presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results.
        /// If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or
        /// deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client
        /// that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object
        /// that was present at the time the first list result was calculated is returned
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history.
        /// When specified for list:
        /// - if unset, then the result is returned from remote storage based on quorum-read flag;
        /// - if it's 0, then we simply return what we currently have in cache, no guarantee;
        /// - if set to non zero, then the result is at least as fresh as given rv.
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
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Endpoints>> WatchNamespacedEndpointAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1Endpoints> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Watch changes to an object of kind Event
        /// </summary>
        /// <param name="name">
        /// name of the ConfigMap
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server.
        /// Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters
        /// (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is
        /// no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will
        /// respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not
        /// supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
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
        /// limit is a maximum number of responses to return for a list call.
        /// If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results.
        /// Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the
        /// presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results.
        /// If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or
        /// deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client
        /// that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object
        /// that was present at the time the first list result was calculated is returned
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history.
        /// When specified for list:
        /// - if unset, then the result is returned from remote storage based on quorum-read flag;
        /// - if it's 0, then we simply return what we currently have in cache, no guarantee;
        /// - if set to non zero, then the result is at least as fresh as given rv.
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
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Event>> WatchNamespacedEventAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1Event> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Watch changes to an object of kind LimitRange
        /// </summary>
        /// <param name="name">
        /// name of the ConfigMap
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server.
        /// Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters
        /// (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is
        /// no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will
        /// respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not
        /// supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
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
        /// limit is a maximum number of responses to return for a list call.
        /// If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results.
        /// Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the
        /// presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results.
        /// If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or
        /// deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client
        /// that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object
        /// that was present at the time the first list result was calculated is returned
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history.
        /// When specified for list:
        /// - if unset, then the result is returned from remote storage based on quorum-read flag;
        /// - if it's 0, then we simply return what we currently have in cache, no guarantee;
        /// - if set to non zero, then the result is at least as fresh as given rv.
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
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1LimitRange>> WatchNamespacedLimitRangeAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1LimitRange> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Watch changes to an object of kind Namespace
        /// </summary>
        /// <param name="name">
        /// name of the ConfigMap
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server.
        /// Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters
        /// (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is
        /// no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will
        /// respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not
        /// supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
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
        /// limit is a maximum number of responses to return for a list call.
        /// If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results.
        /// Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the
        /// presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results.
        /// If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or
        /// deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client
        /// that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object
        /// that was present at the time the first list result was calculated is returned
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history.
        /// When specified for list:
        /// - if unset, then the result is returned from remote storage based on quorum-read flag;
        /// - if it's 0, then we simply return what we currently have in cache, no guarantee;
        /// - if set to non zero, then the result is at least as fresh as given rv.
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
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Namespace>> WatchNamespaceAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1Namespace> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Watch changes to an object of kind Node
        /// </summary>
        /// <param name="name">
        /// name of the ConfigMap
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server.
        /// Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters
        /// (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is
        /// no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will
        /// respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not
        /// supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
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
        /// limit is a maximum number of responses to return for a list call.
        /// If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results.
        /// Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the
        /// presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results.
        /// If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or
        /// deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client
        /// that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object
        /// that was present at the time the first list result was calculated is returned
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history.
        /// When specified for list:
        /// - if unset, then the result is returned from remote storage based on quorum-read flag;
        /// - if it's 0, then we simply return what we currently have in cache, no guarantee;
        /// - if set to non zero, then the result is at least as fresh as given rv.
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
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Node>> WatchNodeAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1Node> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Watch changes to an object of kind PersistentVolumeClaim
        /// </summary>
        /// <param name="name">
        /// name of the ConfigMap
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server.
        /// Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters
        /// (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is
        /// no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will
        /// respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not
        /// supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
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
        /// limit is a maximum number of responses to return for a list call.
        /// If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results.
        /// Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the
        /// presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results.
        /// If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or
        /// deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client
        /// that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object
        /// that was present at the time the first list result was calculated is returned
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history.
        /// When specified for list:
        /// - if unset, then the result is returned from remote storage based on quorum-read flag;
        /// - if it's 0, then we simply return what we currently have in cache, no guarantee;
        /// - if set to non zero, then the result is at least as fresh as given rv.
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
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1PersistentVolumeClaim>> WatchNamespacedPersistentVolumeClaimAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1PersistentVolumeClaim> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Watch changes to an object of kind PersistentVolume
        /// </summary>
        /// <param name="name">
        /// name of the ConfigMap
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server.
        /// Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters
        /// (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is
        /// no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will
        /// respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not
        /// supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
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
        /// limit is a maximum number of responses to return for a list call.
        /// If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results.
        /// Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the
        /// presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results.
        /// If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or
        /// deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client
        /// that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object
        /// that was present at the time the first list result was calculated is returned
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history.
        /// When specified for list:
        /// - if unset, then the result is returned from remote storage based on quorum-read flag;
        /// - if it's 0, then we simply return what we currently have in cache, no guarantee;
        /// - if set to non zero, then the result is at least as fresh as given rv.
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
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1PersistentVolume>> WatchPersistentVolumeAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1PersistentVolume> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Watch changes to an object of kind Pod
        /// </summary>
        /// <param name="name">
        /// name of the ConfigMap
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server.
        /// Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters
        /// (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is
        /// no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will
        /// respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not
        /// supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
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
        /// limit is a maximum number of responses to return for a list call.
        /// If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results.
        /// Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the
        /// presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results.
        /// If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or
        /// deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client
        /// that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object
        /// that was present at the time the first list result was calculated is returned
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history.
        /// When specified for list:
        /// - if unset, then the result is returned from remote storage based on quorum-read flag;
        /// - if it's 0, then we simply return what we currently have in cache, no guarantee;
        /// - if set to non zero, then the result is at least as fresh as given rv.
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
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1Pod>> WatchNamespacedPodAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1Pod> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Watch changes to an object of kind PodTemplate
        /// </summary>
        /// <param name="name">
        /// name of the ConfigMap
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server.
        /// Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters
        /// (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is
        /// no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will
        /// respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not
        /// supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
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
        /// limit is a maximum number of responses to return for a list call.
        /// If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results.
        /// Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the
        /// presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results.
        /// If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or
        /// deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client
        /// that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object
        /// that was present at the time the first list result was calculated is returned
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history.
        /// When specified for list:
        /// - if unset, then the result is returned from remote storage based on quorum-read flag;
        /// - if it's 0, then we simply return what we currently have in cache, no guarantee;
        /// - if set to non zero, then the result is at least as fresh as given rv.
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
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1PodTemplate>> WatchNamespacedPodTemplateAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1PodTemplate> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Watch changes to an object of kind ReplicationController
        /// </summary>
        /// <param name="name">
        /// name of the ConfigMap
        /// </param>
        /// <param name="namespace">
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name="continue">
        /// The continue option should be set when retrieving more results from the server.
        /// Since this value is server defined, clients may only use the continue value from a previous query result with identical query parameters
        /// (except for the value of continue) and the server may reject a continue value it does not recognize. If the specified continue value is
        /// no longer valid whether due to expiration (generally five to fifteen minutes) or a configuration change on the server the server will
        /// respond with a 410 ResourceExpired error indicating the client must restart their list without the continue field. This field is not
        /// supported when watch is true. Clients may start a watch from the last resourceVersion value returned by the server and not miss any modifications.
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
        /// limit is a maximum number of responses to return for a list call.
        /// If more items exist, the server will set the `continue` field on the list metadata to a value that can be used with the same initial query to retrieve the next set of results.
        /// Setting a limit may return fewer than the requested amount of items (up to zero items) in the event all requested objects are filtered out and clients should only use the
        /// presence of the continue field to determine whether more results are available. Servers may choose not to support the limit argument and will return all of the available results.
        /// If limit is specified and the continue field is empty, clients may assume that no more results are available. This field is not supported if watch is true.
        /// The server guarantees that the objects returned when using continue will be identical to issuing a single list call without a limit - that is, no objects created, modified, or
        /// deleted after the first request is issued will be included in any subsequent continued requests. This is sometimes referred to as a consistent snapshot, and ensures that a client
        /// that is using limit to receive smaller chunks of a very large result can ensure they see all possible objects. If objects are updated during a chunked list the version of the object
        /// that was present at the time the first list result was calculated is returned
        /// </param>
        /// <param name="pretty">
        /// If 'true', then the output is pretty printed.
        /// </param>
        /// <param name="timeoutSeconds">
        /// Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.
        /// </param>
        /// <param name="resourceVersion">
        /// When specified with a watch call, shows changes that occur after that particular version of a resource. Defaults to changes from the beginning of history.
        /// When specified for list:
        /// - if unset, then the result is returned from remote storage based on quorum-read flag;
        /// - if it's 0, then we simply return what we currently have in cache, no guarantee;
        /// - if set to non zero, then the result is at least as fresh as given rv.
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
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>
        Task<Watcher<V1ReplicationController>> WatchNamespacedReplicationControllerAsync(string name, string @namespace, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, int? resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, V1ReplicationController> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}
