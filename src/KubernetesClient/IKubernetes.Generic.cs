using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using k8s.Models;
using Microsoft.Rest;

namespace k8s
{
    public partial interface IKubernetes
    {
        /// <summary>
        ///     A generic list or watch operation
        /// </summary>
        /// <param name='allowWatchBookmarks'>
        ///     allowWatchBookmarks requests watch events with type "BOOKMARK".
        ///     Servers that do not implement bookmarks may ignore this flag and
        ///     bookmarks are sent at the server's discretion. Clients should not
        ///     assume bookmarks are returned at any specific interval, nor may
        ///     they assume the server will send any BOOKMARK event during a
        ///     session. If this is not a watch, this field is ignored. If the
        ///     feature gate WatchBookmarks is not enabled in apiserver, this field
        ///     is ignored.
        ///     This field is beta.
        /// </param>
        /// <param name='continueParameter'>
        ///     The continue option should be set when retrieving more results from
        ///     the server. Since this value is server defined, clients may only
        ///     use the continue value from a previous query result with identical
        ///     query parameters (except for the value of continue) and the server
        ///     may reject a continue value it does not recognize. If the specified
        ///     continue value is no longer valid whether due to expiration
        ///     (generally five to fifteen minutes) or a configuration change on
        ///     the server, the server will respond with a 410 ResourceExpired
        ///     error together with a continue token. If the client needs a
        ///     consistent list, it must restart their list without the continue
        ///     field. Otherwise, the client may send another list request with the
        ///     token received with the 410 error, the server will respond with a
        ///     list starting from the next key, but from the latest snapshot,
        ///     which is inconsistent from the previous list results - objects that
        ///     are created, modified, or deleted after the first list request will
        ///     be included in the response, as long as their keys are after the
        ///     "next key".
        ///     This field is not supported when watch is true. Clients may start a
        ///     watch from the last resourceVersion value returned by the server
        ///     and not miss any modifications.
        /// </param>
        /// <param name='fieldSelector'>
        ///     A selector to restrict the list of returned objects by their
        ///     fields. Defaults to everything.
        /// </param>
        /// <param name='labelSelector'>
        ///     A selector to restrict the list of returned objects by their
        ///     labels. Defaults to everything.
        /// </param>
        /// <param name='limit'>
        ///     limit is a maximum number of responses to return for a list call.
        ///     If more items exist, the server will set the `continue` field on
        ///     the list metadata to a value that can be used with the same initial
        ///     query to retrieve the next set of results. Setting a limit may
        ///     return fewer than the requested amount of items (up to zero items)
        ///     in the event all requested objects are filtered out and clients
        ///     should only use the presence of the continue field to determine
        ///     whether more results are available. Servers may choose not to
        ///     support the limit argument and will return all of the available
        ///     results. If limit is specified and the continue field is empty,
        ///     clients may assume that no more results are available. This field
        ///     is not supported if watch is true.
        ///     The server guarantees that the objects returned when using continue
        ///     will be identical to issuing a single list call without a limit -
        ///     that is, no objects created, modified, or deleted after the first
        ///     request is issued will be included in any subsequent continued
        ///     requests. This is sometimes referred to as a consistent snapshot,
        ///     and ensures that a client that is using limit to receive smaller
        ///     chunks of a very large result can ensure they see all possible
        ///     objects. If objects are updated during a chunked list the version
        ///     of the object that was present at the time the first list result
        ///     was calculated is returned.
        /// </param>
        /// <param name='resourceVersion'>
        ///     When specified with a watch call, shows changes that occur after
        ///     that particular version of a resource. Defaults to changes from the
        ///     beginning of history. When specified for list: - if unset, then the
        ///     result is returned from remote storage based on quorum-read flag; -
        ///     if it's 0, then we simply return what we currently have in cache,
        ///     no guarantee; - if set to non zero, then the result is at least as
        ///     fresh as given rv.
        /// </param>
        /// <param name='timeoutSeconds'>
        ///     Timeout for the list/watch call. This limits the duration of the
        ///     call, regardless of any activity or inactivity.
        /// </param>
        /// <param name='watch'>
        ///     Watch for changes to the described resources and return them as a
        ///     stream of add, update, and remove notifications. Specify
        ///     resourceVersion.
        /// </param>
        /// <param name='pretty'>
        ///     If 'true', then the output is pretty printed.
        /// </param>
        /// <param name='customHeaders'>
        ///     The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        ///     The cancellation token.
        /// </param>
        Task<HttpOperationResponse<KubernetesList<T>>> ListWithHttpMessagesAsync<T>(
            string namespaceParameter = default,
            bool? allowWatchBookmarks = default,
            string continueParameter = default,
            string fieldSelector = default,
            string labelSelector = default,
            int? limit = default,
            string resourceVersion = default,
            TimeSpan? timeout = default,
            bool? watch = default,
            string pretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default) where T : IKubernetesObject;

        /// <summary>
        ///     A generic list or watch operation
        /// </summary>
        /// <param name="type">Type of list</param>
        /// <param name='allowWatchBookmarks'>
        ///     allowWatchBookmarks requests watch events with type "BOOKMARK".
        ///     Servers that do not implement bookmarks may ignore this flag and
        ///     bookmarks are sent at the server's discretion. Clients should not
        ///     assume bookmarks are returned at any specific interval, nor may
        ///     they assume the server will send any BOOKMARK event during a
        ///     session. If this is not a watch, this field is ignored. If the
        ///     feature gate WatchBookmarks is not enabled in apiserver, this field
        ///     is ignored.
        ///     This field is beta.
        /// </param>
        /// <param name='continueParameter'>
        ///     The continue option should be set when retrieving more results from
        ///     the server. Since this value is server defined, clients may only
        ///     use the continue value from a previous query result with identical
        ///     query parameters (except for the value of continue) and the server
        ///     may reject a continue value it does not recognize. If the specified
        ///     continue value is no longer valid whether due to expiration
        ///     (generally five to fifteen minutes) or a configuration change on
        ///     the server, the server will respond with a 410 ResourceExpired
        ///     error together with a continue token. If the client needs a
        ///     consistent list, it must restart their list without the continue
        ///     field. Otherwise, the client may send another list request with the
        ///     token received with the 410 error, the server will respond with a
        ///     list starting from the next key, but from the latest snapshot,
        ///     which is inconsistent from the previous list results - objects that
        ///     are created, modified, or deleted after the first list request will
        ///     be included in the response, as long as their keys are after the
        ///     "next key".
        ///     This field is not supported when watch is true. Clients may start a
        ///     watch from the last resourceVersion value returned by the server
        ///     and not miss any modifications.
        /// </param>
        /// <param name='fieldSelector'>
        ///     A selector to restrict the list of returned objects by their
        ///     fields. Defaults to everything.
        /// </param>
        /// <param name='labelSelector'>
        ///     A selector to restrict the list of returned objects by their
        ///     labels. Defaults to everything.
        /// </param>
        /// <param name='limit'>
        ///     limit is a maximum number of responses to return for a list call.
        ///     If more items exist, the server will set the `continue` field on
        ///     the list metadata to a value that can be used with the same initial
        ///     query to retrieve the next set of results. Setting a limit may
        ///     return fewer than the requested amount of items (up to zero items)
        ///     in the event all requested objects are filtered out and clients
        ///     should only use the presence of the continue field to determine
        ///     whether more results are available. Servers may choose not to
        ///     support the limit argument and will return all of the available
        ///     results. If limit is specified and the continue field is empty,
        ///     clients may assume that no more results are available. This field
        ///     is not supported if watch is true.
        ///     The server guarantees that the objects returned when using continue
        ///     will be identical to issuing a single list call without a limit -
        ///     that is, no objects created, modified, or deleted after the first
        ///     request is issued will be included in any subsequent continued
        ///     requests. This is sometimes referred to as a consistent snapshot,
        ///     and ensures that a client that is using limit to receive smaller
        ///     chunks of a very large result can ensure they see all possible
        ///     objects. If objects are updated during a chunked list the version
        ///     of the object that was present at the time the first list result
        ///     was calculated is returned.
        /// </param>
        /// <param name='resourceVersion'>
        ///     When specified with a watch call, shows changes that occur after
        ///     that particular version of a resource. Defaults to changes from the
        ///     beginning of history. When specified for list: - if unset, then the
        ///     result is returned from remote storage based on quorum-read flag; -
        ///     if it's 0, then we simply return what we currently have in cache,
        ///     no guarantee; - if set to non zero, then the result is at least as
        ///     fresh as given rv.
        /// </param>
        /// <param name='timeoutSeconds'>
        ///     Timeout for the list/watch call. This limits the duration of the
        ///     call, regardless of any activity or inactivity.
        /// </param>
        /// <param name='watch'>
        ///     Watch for changes to the described resources and return them as a
        ///     stream of add, update, and remove notifications. Specify
        ///     resourceVersion.
        /// </param>
        /// <param name='pretty'>
        ///     If 'true', then the output is pretty printed.
        /// </param>
        /// <param name='customHeaders'>
        ///     The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        ///     The cancellation token.
        /// </param>
        Task<HttpOperationResponse> ListWithHttpMessagesAsync(
            Type type,
            string namespaceParameter = default,
            bool? allowWatchBookmarks = default,
            string continueParameter = default,
            string fieldSelector = default,
            string labelSelector = default,
            int? limit = default,
            string resourceVersion = default,
            TimeSpan? timeout = default,
            bool? watch = default,
            string pretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     read the specified Kubernetes Object
        /// </summary>
        /// <param name='name'>
        ///     name of the Kubernetes Object
        /// </param>
        /// <param name='namespaceParameter'>
        ///     object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='exact'>
        ///     Should the export be exact.  Exact export maintains cluster-specific fields
        ///     like 'Namespace'. Deprecated. Planned for removal in 1.18.
        /// </param>
        /// <param name='export'>
        ///     Should this value be exported.  Export strips fields that a user can not
        ///     specify. Deprecated. Planned for removal in 1.18.
        /// </param>
        /// <param name='pretty'>
        ///     If 'true', then the output is pretty printed.
        /// </param>
        /// <param name='customHeaders'>
        ///     Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        ///     The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        ///     Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        ///     Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <return>
        ///     A response object containing the response body and response headers.
        /// </return>
        Task<HttpOperationResponse<T>> ReadWithHttpMessagesAsync<T>(
            string name,
            string namespaceParameter = default,
            bool? exact = default,
            bool? export = default,
            string pretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default) where T : IKubernetesObject;

        /// <summary>
        ///     read the specified ConfigMap
        /// </summary>
        /// <param name="type">Type of resource</param>
        /// <param name='name'>
        ///     name of the ConfigMap
        /// </param>
        /// <param name='namespaceParameter'>
        ///     object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='exact'>
        ///     Should the export be exact.  Exact export maintains cluster-specific fields
        ///     like 'Namespace'. Deprecated. Planned for removal in 1.18.
        /// </param>
        /// <param name='export'>
        ///     Should this value be exported.  Export strips fields that a user can not
        ///     specify. Deprecated. Planned for removal in 1.18.
        /// </param>
        /// <param name='pretty'>
        ///     If 'true', then the output is pretty printed.
        /// </param>
        /// <param name='customHeaders'>
        ///     Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        ///     The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        ///     Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        ///     Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <return>
        ///     A response object containing the response body and response headers.
        /// </return>
        Task<HttpOperationResponse> ReadWithHttpMessagesAsync(
            Type type,
            string name,
            string namespaceParameter = default,
            bool? exact = default,
            bool? export = default,
            string pretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     create a Kubernetes Object
        /// </summary>
        /// <param name='body'>
        /// </param>
        /// <param name='namespaceParameter'>
        ///     object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='dryRun'>
        ///     When present, indicates that modifications should not be persisted. An
        ///     invalid or unrecognized dryRun directive will result in an error response
        ///     and no further processing of the request. Valid values are: - All: all dry
        ///     run stages will be processed
        /// </param>
        /// <param name='fieldManager'>
        ///     fieldManager is a name associated with the actor or entity that is making
        ///     these changes. The value must be less than or 128 characters long, and only
        ///     contain printable characters, as defined by
        ///     https://golang.org/pkg/unicode/#IsPrint.
        /// </param>
        /// <param name='pretty'>
        ///     If 'true', then the output is pretty printed.
        /// </param>
        /// <param name='customHeaders'>
        ///     Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        ///     The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        ///     Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        ///     Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <return>
        ///     A response object containing the response body and response headers.
        /// </return>
        Task<HttpOperationResponse<T>> CreateWithHttpMessagesAsync<T>(
            T body,
            string namespaceParameter = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            string pretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default) where T : IKubernetesObject;


        /// <summary>
        ///     create a Kubernetes Object
        /// </summary>
        /// <param name='body'>
        /// </param>
        /// <param name='namespaceParameter'>
        ///     object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='dryRun'>
        ///     When present, indicates that modifications should not be persisted. An
        ///     invalid or unrecognized dryRun directive will result in an error response
        ///     and no further processing of the request. Valid values are: - All: all dry
        ///     run stages will be processed
        /// </param>
        /// <param name='fieldManager'>
        ///     fieldManager is a name associated with the actor or entity that is making
        ///     these changes. The value must be less than or 128 characters long, and only
        ///     contain printable characters, as defined by
        ///     https://golang.org/pkg/unicode/#IsPrint.
        /// </param>
        /// <param name='pretty'>
        ///     If 'true', then the output is pretty printed.
        /// </param>
        /// <param name='customHeaders'>
        ///     Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        ///     The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        ///     Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        ///     Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <return>
        ///     A response object containing the response body and response headers.
        /// </return>
        Task<HttpOperationResponse> CreateWithHttpMessagesAsync(
            object body,
            string namespaceParameter = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            string pretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     delete a Kubernetes Object
        /// </summary>
        /// <param name='name'>
        ///     name of the Kubernetes Object
        /// </param>
        /// <param name='namespaceParameter'>
        ///     object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='body'>
        /// </param>
        /// <param name='dryRun'>
        ///     When present, indicates that modifications should not be persisted. An
        ///     invalid or unrecognized dryRun directive will result in an error response
        ///     and no further processing of the request. Valid values are: - All: all dry
        ///     run stages will be processed
        /// </param>
        /// <param name='gracePeriodSeconds'>
        ///     The duration in seconds before the object should be deleted. Value must be
        ///     non-negative integer. The value zero indicates delete immediately. If this
        ///     value is nil, the default grace period for the specified type will be used.
        ///     Defaults to a per object value if not specified. zero means delete
        ///     immediately.
        /// </param>
        /// <param name='orphanDependents'>
        ///     Deprecated: please use the PropagationPolicy, this field will be deprecated
        ///     in 1.7. Should the dependent objects be orphaned. If true/false, the
        ///     "orphan" finalizer will be added to/removed from the object's finalizers
        ///     list. Either this field or PropagationPolicy may be set, but not both.
        /// </param>
        /// <param name='propagationPolicy'>
        ///     Whether and how garbage collection will be performed. Either this field or
        ///     OrphanDependents may be set, but not both. The default policy is decided by
        ///     the existing finalizer set in the metadata.finalizers and the
        ///     resource-specific default policy. Acceptable values are: 'Orphan' - orphan
        ///     the dependents; 'Background' - allow the garbage collector to delete the
        ///     dependents in the background; 'Foreground' - a cascading policy that
        ///     deletes all dependents in the foreground.
        /// </param>
        /// <param name='pretty'>
        ///     If 'true', then the output is pretty printed.
        /// </param>
        /// <param name='customHeaders'>
        ///     Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        ///     The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        ///     Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        ///     Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <return>
        ///     A response object containing the response body and response headers.
        /// </return>
        Task<HttpOperationResponse<V1Status>> DeleteWithHttpMessagesAsync<T>(
            string name = default,
            string namespaceParameter = default,
            bool? allowWatchBookmarks = default,
            V1DeleteOptions body = default,
            string continueParameter = default,
            DryRun? dryRun = default,
            string fieldSelector = default,
            TimeSpan? gracePeriodSeconds = default,
            string labelSelector = default,
            int? limit = default,
            bool? orphanDependents = default,
            PropagationPolicy? propagationPolicy = default,
            string resourceVersion = default,
            TimeSpan? timeout = default,
            bool? watch = default,
            string pretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default) where T : IKubernetesObject;


        /// <summary>
        ///     delete a resource
        /// </summary>
        /// <param name='name'>
        ///     name of the resource
        /// </param>
        /// <param name='namespaceParameter'>
        ///     object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='body'>
        /// </param>
        /// <param name='dryRun'>
        ///     When present, indicates that modifications should not be persisted. An
        ///     invalid or unrecognized dryRun directive will result in an error response
        ///     and no further processing of the request. Valid values are: - All: all dry
        ///     run stages will be processed
        /// </param>
        /// <param name='gracePeriodSeconds'>
        ///     The duration in seconds before the object should be deleted. Value must be
        ///     non-negative integer. The value zero indicates delete immediately. If this
        ///     value is nil, the default grace period for the specified type will be used.
        ///     Defaults to a per object value if not specified. zero means delete
        ///     immediately.
        /// </param>
        /// <param name='orphanDependents'>
        ///     Deprecated: please use the PropagationPolicy, this field will be deprecated
        ///     in 1.7. Should the dependent objects be orphaned. If true/false, the
        ///     "orphan" finalizer will be added to/removed from the object's finalizers
        ///     list. Either this field or PropagationPolicy may be set, but not both.
        /// </param>
        /// <param name='propagationPolicy'>
        ///     Whether and how garbage collection will be performed. Either this field or
        ///     OrphanDependents may be set, but not both. The default policy is decided by
        ///     the existing finalizer set in the metadata.finalizers and the
        ///     resource-specific default policy. Acceptable values are: 'Orphan' - orphan
        ///     the dependents; 'Background' - allow the garbage collector to delete the
        ///     dependents in the background; 'Foreground' - a cascading policy that
        ///     deletes all dependents in the foreground.
        /// </param>
        /// <param name='pretty'>
        ///     If 'true', then the output is pretty printed.
        /// </param>
        /// <param name='customHeaders'>
        ///     Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        ///     The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        ///     Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        ///     Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <return>
        ///     A response object containing the response body and response headers.
        /// </return>
        Task<HttpOperationResponse<V1Status>> DeleteWithHttpMessagesAsync(
            Type type,
            string name = default,
            string namespaceParameter = default,
            bool? allowWatchBookmarks = default,
            V1DeleteOptions body = default,
            string continueParameter = default,
            DryRun? dryRun = default,
            string fieldSelector = default,
            TimeSpan? gracePeriodSeconds = default,
            string labelSelector = default,
            int? limit = default,
            bool? orphanDependents = default,
            PropagationPolicy? propagationPolicy = default,
            string resourceVersion = default,
            TimeSpan? timeout = default,
            bool? watch = default,
            string pretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     partially update the specified Kubernetes Object
        /// </summary>
        /// <param name='body'>
        /// </param>
        /// <param name='name'>
        ///     name of the Kubernetes Object
        /// </param>
        /// <param name='namespaceParameter'>
        ///     object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='dryRun'>
        ///     When present, indicates that modifications should not be persisted. An
        ///     invalid or unrecognized dryRun directive will result in an error response
        ///     and no further processing of the request. Valid values are: - All: all dry
        ///     run stages will be processed
        /// </param>
        /// <param name='fieldManager'>
        ///     fieldManager is a name associated with the actor or entity that is making
        ///     these changes. The value must be less than or 128 characters long, and only
        ///     contain printable characters, as defined by
        ///     https://golang.org/pkg/unicode/#IsPrint. This field is required for apply
        ///     requests (application/apply-patch) but optional for non-apply patch types
        ///     (JsonPatch, MergePatch, StrategicMergePatch).
        /// </param>
        /// <param name='force'>
        ///     Force is going to "force" Apply requests. It means user will re-acquire
        ///     conflicting fields owned by other people. Force flag must be unset for
        ///     non-apply patch requests.
        /// </param>
        /// <param name='pretty'>
        ///     If 'true', then the output is pretty printed.
        /// </param>
        /// <param name='customHeaders'>
        ///     Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        ///     The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        ///     Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        ///     Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <return>
        ///     A response object containing the response body and response headers.
        /// </return>
        Task<HttpOperationResponse<T>> PatchWithHttpMessagesAsync<T>(
            V1Patch body,
            string name,
            string namespaceParameter = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            bool? force = default,
            string pretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default) where T : IKubernetesObject;


        /// <summary>
        ///     partially update status of the specified resource
        /// </summary>
        /// <param name='body'>
        /// </param>
        /// <param name='name'>
        ///     name of the resource
        /// </param>
        /// <param name='namespaceParameter'>
        ///     object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='dryRun'>
        ///     When present, indicates that modifications should not be persisted. An
        ///     invalid or unrecognized dryRun directive will result in an error response
        ///     and no further processing of the request. Valid values are: - All: all dry
        ///     run stages will be processed
        /// </param>
        /// <param name='fieldManager'>
        ///     fieldManager is a name associated with the actor or entity that is making
        ///     these changes. The value must be less than or 128 characters long, and only
        ///     contain printable characters, as defined by
        ///     https://golang.org/pkg/unicode/#IsPrint. This field is required for apply
        ///     requests (application/apply-patch) but optional for non-apply patch types
        ///     (JsonPatch, MergePatch, StrategicMergePatch).
        /// </param>
        /// <param name='force'>
        ///     Force is going to "force" Apply requests. It means user will re-acquire
        ///     conflicting fields owned by other people. Force flag must be unset for
        ///     non-apply patch requests.
        /// </param>
        /// <param name='pretty'>
        ///     If 'true', then the output is pretty printed.
        /// </param>
        /// <param name='customHeaders'>
        ///     Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        ///     The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        ///     Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        ///     Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <return>
        ///     A response object containing the response body and response headers.
        /// </return>
        Task<HttpOperationResponse> PatchWithHttpMessagesAsync(
            Type type,
            V1Patch body,
            string name,
            string namespaceParameter = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            bool? force = default,
            string pretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Replace the specified resource
        /// </summary>
        /// <param name='body'>
        /// </param>
        /// <param name='name'>
        ///     name of the PodDisruptionBudget
        /// </param>
        /// <param name='namespaceParameter'>
        ///     object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='dryRun'>
        ///     When present, indicates that modifications should not be persisted. An
        ///     invalid or unrecognized dryRun directive will result in an error response
        ///     and no further processing of the request. Valid values are: - All: all dry
        ///     run stages will be processed
        /// </param>
        /// <param name='fieldManager'>
        ///     fieldManager is a name associated with the actor or entity that is making
        ///     these changes. The value must be less than or 128 characters long, and only
        ///     contain printable characters, as defined by
        ///     https://golang.org/pkg/unicode/#IsPrint.
        /// </param>
        /// <param name='pretty'>
        ///     If 'true', then the output is pretty printed.
        /// </param>
        /// <param name='customHeaders'>
        ///     Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        ///     The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        ///     Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        ///     Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <return>
        ///     A response object containing the response body and response headers.
        /// </return>
        Task<HttpOperationResponse<T>> ReplaceWithHttpMessagesAsync<T>(
            T body,
            string name,
            string namespaceParameter = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            string pretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Replace the specified resource
        /// </summary>
        /// <param name='body'>
        /// </param>
        /// <param name='name'>
        ///     name of the PodDisruptionBudget
        /// </param>
        /// <param name='namespaceParameter'>
        ///     object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='dryRun'>
        ///     When present, indicates that modifications should not be persisted. An
        ///     invalid or unrecognized dryRun directive will result in an error response
        ///     and no further processing of the request. Valid values are: - All: all dry
        ///     run stages will be processed
        /// </param>
        /// <param name='fieldManager'>
        ///     fieldManager is a name associated with the actor or entity that is making
        ///     these changes. The value must be less than or 128 characters long, and only
        ///     contain printable characters, as defined by
        ///     https://golang.org/pkg/unicode/#IsPrint.
        /// </param>
        /// <param name='pretty'>
        ///     If 'true', then the output is pretty printed.
        /// </param>
        /// <param name='customHeaders'>
        ///     Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        ///     The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        ///     Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        ///     Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when a required parameter is null
        /// </exception>
        /// <return>
        ///     A response object containing the response body and response headers.
        /// </return>
        Task<HttpOperationResponse> ReplaceWithHttpMessagesAsync(object body,
            string name,
            string namespaceParameter = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            string pretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default);
    }
}
