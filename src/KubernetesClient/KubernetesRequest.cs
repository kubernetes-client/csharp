using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using k8s.Models;
using Newtonsoft.Json;

namespace k8s
{
    #region KubernetesRequest
    /// <summary>Represents a single request to Kubernetes.</summary>
    public sealed class KubernetesRequest : ICloneable
    {
        /// <summary>Initializes a <see cref="KubernetesRequest"/> based on a <see cref="KubernetesClient"/>.</summary>
        public KubernetesRequest(Kubernetes client) : this(client.config, client.HttpClient, client.Scheme) { }

        /// <summary>Initializes a <see cref="KubernetesRequest"/> based on a <see cref="KubernetesClientConfiguration"/> and
        /// <see cref="HttpClient"/>.
        /// </summary>
        /// <remarks>Any necessary SSL configuration must have already been applied to the <paramref name="client"/>.</remarks>
        public KubernetesRequest(KubernetesClientConfiguration config, HttpClient client, KubernetesScheme scheme = null)
        {
            if(config == null) throw new ArgumentNullException(nameof(config));
            if(string.IsNullOrEmpty(config.Host)) throw new ArgumentException("The kubernetes host must be provided.");
            this.config = config;
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            Scheme(scheme);
        }

        /// <summary>Gets the value of the Accept header, or null to use the default of application/json.</summary>
        public string Accept() => _accept;

        /// <summary>Sets the value of the Accept header, or null or empty to use the default of application/json.</summary>
        public KubernetesRequest Accept(string mediaType) { _accept = NormalizeEmpty(mediaType); return this; }

        /// <summary>Adds a query-string parameter to the request. Multiple headers with the same name can be set this way.</summary>
        public KubernetesRequest AddHeader(string key, string value) => Add(ref headers, key, value);

        /// <summary>Adds a query-string parameter to the request. Multiple parameters with the same name can be set this way.</summary>
        public KubernetesRequest AddQuery(string key, string value) => Add(ref query, key, value);

        /// <summary>Gets the body to be sent to the server.</summary>
        public object Body() => _body;

        /// <summary>Sets the body to be sent to the server. If null, no body will be sent. If a string, byte array, or stream, the
        /// contents will be sent directly. Otherwise, the body will be serialized into JSON and sent.
        /// </summary>
        public KubernetesRequest Body(object body) { _body = body; return this; }

        /// <summary>Clears all custom header values.</summary>
        public KubernetesRequest ClearHeaders()
        {
            if(headers != null) headers.Clear();
            return this;
        }

        /// <summary>Clears custom header values with the given name.</summary>
        public KubernetesRequest ClearHeaders(string headerName)
        {
            if(headerName == null) throw new ArgumentNullException(nameof(headerName));
            if(headers != null) headers.Remove(headerName);
            return this;
        }

        /// <summary>Clears all query-string parameters.</summary>
        public KubernetesRequest ClearQuery()
        {
            if(query != null) query.Clear();
            return this;
        }

        /// <summary>Clears all query-string parameters with the given key.</summary>
        public KubernetesRequest ClearQuery(string key)
        {
            if(key == null) throw new ArgumentNullException(nameof(key));
            if(query != null) query.Remove(key);
            return this;
        }

        /// <summary>Creates a deep copy of the <see cref="KubernetesRequest"/>.</summary>
        public KubernetesRequest Clone()
        {
            var clone = (KubernetesRequest)MemberwiseClone();
            if(headers != null)
            {
                clone.headers = new Dictionary<string, List<string>>(headers.Count);
                foreach(KeyValuePair<string, List<string>> pair in headers) clone.headers.Add(pair.Key, new List<string>(pair.Value));
            }
            if(query != null)
            {
                clone.query = new Dictionary<string, List<string>>(query.Count);
                foreach(KeyValuePair<string, List<string>> pair in query) clone.query.Add(pair.Key, new List<string>(pair.Value));
            }
            return clone;
        }

        /// <summary>Sets the <see cref="Method()"/> to <see cref="HttpMethod.Delete"/>.</summary>
        public KubernetesRequest Delete() => Method(HttpMethod.Delete);

        /// <summary>Sets the <see cref="Method()"/> to <see cref="HttpMethod.Get"/>.</summary>
        public KubernetesRequest Get() => Method(HttpMethod.Get);

        /// <summary>Sets the <see cref="Method()"/> to <see cref="HttpMethod.Patch"/>.</summary>
        public KubernetesRequest Patch() => Method(new HttpMethod("PATCH"));

        /// <summary>Sets the <see cref="Method()"/> to <see cref="HttpMethod.Post"/>.</summary>
        public KubernetesRequest Post() => Method(HttpMethod.Post);

        /// <summary>Sets the <see cref="Method()"/> to <see cref="HttpMethod.Put"/>.</summary>
        public KubernetesRequest Put() => Method(HttpMethod.Put);

        /// <summary>Sets the value of the "dryRun" query-string parameter, as a boolean.</summary>
        public bool DryRun() => string.IsNullOrEmpty(GetQuery("dryRun"));

        /// <summary>Sets the value of the "dryRun" query-string parameter to "All" or removes it.</summary>
        public KubernetesRequest DryRun(bool dryRun) => SetQuery("dryRun", dryRun ? "All" : null);

        /// <summary>Executes the request and returns a <see cref="KubernetesResponse"/>. The request can be executed multiple times,
        /// and can be executed multiple times in parallel.
        /// </summary>
        public async Task<KubernetesResponse> ExecuteAsync(CancellationToken cancelToken = default)
        {
            cancelToken.ThrowIfCancellationRequested();
            HttpRequestMessage req = CreateRequestMessage();
            // requests like watches may not send a body immediately, so return as soon as we've got the response headers
            var completion = _streamResponse || _watchVersion != null ?
                HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead;
            return new KubernetesResponse(await client.SendAsync(req, completion, cancelToken).ConfigureAwait(false));
        }

        /// <summary>Executes the request and returns the deserialized response body (or the default value of type
        /// <typeparamref name="T"/> if the response was 404 Not Found).
        /// </summary>
        /// <exception cref="KubernetesException">Thrown if the response was any error besides 404 Not Found.</exception>
        public Task<T> ExecuteAsync<T>(CancellationToken cancelToken = default) => ExecuteAsync<T>(false, cancelToken);

        /// <summary>Executes the request and returns the deserialized response body.</summary>
        /// <param name="failIfMissing">If true and the response is 404 Not Found, an exception will be thrown. If false, the default
        /// value of type <typeparamref name="T"/> will be returned in that case. The default is false.
        /// </param>
        /// <param name="cancelToken">A <see cref="CancellationToken"/> that can be used to cancel the request</param>
        /// <exception cref="KubernetesException">Thrown if the response was any error besides 404 Not Found.</exception>
        public Task<T> ExecuteAsync<T>(bool failIfMissing, CancellationToken cancelToken = default)
        {
            if(_watchVersion != null) throw new InvalidOperationException("Watch requests cannot be deserialized all at once.");
            cancelToken.ThrowIfCancellationRequested();
            return ExecuteMessageAsync<T>(CreateRequestMessage(), failIfMissing, cancelToken);
        }

        /// <summary>Gets the "fieldManager" query-string parameter, or null if there is no field manager.</summary>
        public string FieldManager() => NormalizeEmpty(GetQuery("fieldManager"));

        /// <summary>Sets the "fieldManager" query-string parameter, or removes it if the value is null or empty.</summary>
        public KubernetesRequest FieldManager(string manager) =>
            SetQuery("fieldManager", !string.IsNullOrEmpty(manager) ? manager : null);

        /// <summary>Gets the "fieldSelector" query-string parameter, or null if there is no field selector.</summary>
        public string FieldSelector() => NormalizeEmpty(GetQuery("fieldSelector"));

        /// <summary>Sets the "fieldSelector" query-string parameter, or removes it if the selector is null or empty.</summary>
        public KubernetesRequest FieldSelector(string selector) =>
            SetQuery("fieldSelector", !string.IsNullOrEmpty(selector) ? selector : null);

        /// <summary>Gets the value of the named custom header, or null if it doesn't exist.</summary>
        /// <exception cref="InvalidOperationException">Thrown if there are multiple custom headers with the given name</exception>
        public string GetHeader(string key)
        {
            List<string> values = null;
            if(headers != null) headers.TryGetValue(key, out values);
            return values == null || values.Count == 0 ? null : values.Count == 1 ? values[0] :
                throw new InvalidOperationException($"There are multiple query-string parameters named '{key}'.");
        }

        /// <summary>Gets the values of the named custom header, or null if it has no values.</summary>
        /// <remarks>The returned collection, if not null, can be mutated to change the set of values.</remarks>
        public List<string> GetHeaderValues(string key)
        {
            List<string> values = null;
            if(headers != null) headers.TryGetValue(key, out values);
            return values;
        }

        /// <summary>Gets the value of the named query-string parameter, or null if it doesn't exist.</summary>
        /// <exception cref="InvalidOperationException">Thrown if there are multiple query-string parameters with the given name</exception>
        public string GetQuery(string key)
        {
            List<string> values = GetQueryValues(key);
            return values == null || values.Count == 0 ? null : values.Count == 1 ? values[0] :
                throw new InvalidOperationException($"There are multiple query-string parameters named '{key}'.");
        }

        /// <summary>Gets the values of the named query-string parameter, or null if it has no values.</summary>
        /// <remarks>The returned collection, if not null, can be mutated to change the set of values.</remarks>
        public List<string> GetQueryValues(string key)
        {
            List<string> values = null;
            if(query != null) query.TryGetValue(key, out values);
            return values;
        }

        /// <summary>Gets the Kubernetes API group to use, or null or empty to use the default, which is the core API group
        /// unless a <see cref="RawUri(string)"/> is given.
        /// </summary>
        public string Group() => _group;

        /// <summary>Sets the Kubernetes API group to use, or null or empty to use the default, which is the core API group
        /// unless a <see cref="RawUri(string)"/> is given.
        /// </summary>
        public KubernetesRequest Group(string group) { _group = NormalizeEmpty(group); return this; }

        /// <summary>Attempts to set the <see cref="Group()"/>, <see cref="Version()"/>, and <see cref="Type()"/> based on an object.</summary>
        /// <remarks>The method calls <see cref="GVK(Type)"/> with the object's type. Then, if <see cref="IKuberenetesObject.ApiVersion"/>
        /// is set, it will override <see cref="Group()"/> and <see cref="Version()"/>.
        /// </remarks>
        public KubernetesRequest GVK(IKubernetesObject obj)
        {
            if(obj == null) throw new ArgumentNullException();
            GVK(obj.GetType());
            if(!string.IsNullOrEmpty(obj.ApiVersion)) // if the object has an API version set, use it...
            {
                int slash = obj.ApiVersion.IndexOf('/'); // the ApiVersion field is in the form "version" or "group/version"
                Group(slash >= 0 ? obj.ApiVersion.Substring(0, slash) : null).Version(obj.ApiVersion.Substring(slash+1));
            }
            return this;
        }

        /// <summary>Attempts to set the <see cref="Group()"/>, <see cref="Version()"/>, and <see cref="Type()"/> based on a Kubernetes
        /// group, version, and kind. The method uses heuristics and may not work in all cases.
        /// </summary>
        public KubernetesRequest GVK(string group, string version, string kind) =>
            Group(!string.IsNullOrEmpty(group) ? group : null).Version(!string.IsNullOrEmpty(version) ? version : null)
                .Type(KubernetesScheme.GuessPath(kind));

        /// <summary>Attempts to set the <see cref="Group()"/>, <see cref="Version()"/>, and <see cref="Type()"/> based on a type of object,
        /// such as <see cref="k8s.Models.V1Pod"/>.
        /// </summary>
        public KubernetesRequest GVK(Type type)
        {
            if(type == null) throw new ArgumentNullException(nameof(type));
            _scheme.GetGVK(type, out string group, out string version, out string kind, out string path);
            return Group(NormalizeEmpty(group)).Version(version).Type(path);
        }

        /// <summary>Attempts to set the <see cref="Group()"/>, <see cref="Version()"/>, and <see cref="Type()"/> based on a type of object,
        /// such as <see cref="k8s.Models.V1Pod"/>.
        /// </summary>
        public KubernetesRequest GVK<T>() => GVK(typeof(T));

        /// <summary>Gets the "labelSelector" query-string parameter, or null if there is no label selector.</summary>
        public string LabelSelector() => NormalizeEmpty(GetQuery("labelSelector"));

        /// <summary>Sets the "labelSelector" query-string parameter, or removes it if the selecor is null or empty.</summary>
        public KubernetesRequest LabelSelector(string selector) =>
            SetQuery("labelSelector", !string.IsNullOrEmpty(selector) ? selector : null);

        /// <summary>Gets the value of the Content-Type header, or null to use the default of application/json.</summary>
        public string MediaType() => _mediaType;

        /// <summary>Sets the value of the Content-Type header, not including any parameters, or null or empty to use the default
        /// of application/json. The header value will only be used if a <see cref="Body(object)"/> is supplied.
        /// </summary>
        public KubernetesRequest MediaType(string mediaType) { _mediaType = NormalizeEmpty(mediaType); return this; }

        /// <summary>Gets the <see cref="HttpMethod"/> to use.</summary>
        public HttpMethod Method() => _method ?? HttpMethod.Get;

        /// <summary>Sets the <see cref="HttpMethod"/> to use, or null to use the default of <see cref="HttpMethod.Get"/>.</summary>
        public KubernetesRequest Method(HttpMethod method) { _method = method; return this; }

        /// <summary>Gets the name of the top-level Kubernetes resource to access.</summary>
        public string Name() => _name;

        /// <summary>Sets the name of the top-level Kubernetes resource to access, or null or empty to not access a specific object.</summary>
        public KubernetesRequest Name(string name) { _name = name; return this; }

        /// <summary>Gets the Kubernetes namespace to access.</summary>
        public string Namespace() => _ns;

        /// <summary>Sets the Kubernetes namespace to access, or null or empty to not access a namespaced object.</summary>
        public KubernetesRequest Namespace(string ns) { _ns = ns; return this; }

        /// <summary>Gets the raw URL to access, relative to the configured Kubernetes host and not including the query string, or
        /// null if the URL will be constructed piecemeal based on the other properties.
        /// </summary>
        public string RawUri() => _rawUri;

        /// <summary>Sets the raw URL to access, relative to the configured Kubernetes host and not including the query string, or
        /// null or empty to construct the URI piecemeal based on the other properties.
        /// </summary>
        public KubernetesRequest RawUri(string uri)
        {
            uri = NormalizeEmpty(uri);
            if(uri != null && uri[0] != '/') throw new ArgumentException("The URI must begin with a slash.");
            _rawUri = uri;
            return this;
        }

        /// <summary>Performs an atomic get-modify-replace operation, using the GET method to read the object and the PUT method to
        /// replace it.
        /// </summary>
        /// <param name="modify">A function that modifies the resource, returning true if any changes were made and false if not</param>
        /// <param name="failIfMissing">If true, an exception will be thrown if the object doesn't exist. If false, null will be
        /// returned in that case.
        /// </param>
        /// <param name="cancelToken">A <see cref="CancellationToken"/> that can be used to cancel the request</param>
        public Task<T> ReplaceAsync<T>(Func<T,bool> update, bool failIfMissing = false, CancellationToken cancelToken = default)
            where T : class, IMetadata<V1ObjectMeta> => ReplaceAsync<T>(null, update, failIfMissing, cancelToken);

        /// <summary>Performs an atomic get-modify-replace operation, using the GET method to read the object and the PUT method to
        /// replace it.
        /// </summary>
        /// <param name="modify">A function that modifies the resource, returning true if any changes were made and false if not</param>
        /// <param name="failIfMissing">If true, an exception will be thrown if the object doesn't exist. If false, null will be
        /// returned in that case.
        /// </param>
        /// <param name="cancelToken">A <see cref="CancellationToken"/> that can be used to cancel the request</param>
        public Task<T> ReplaceAsync<T>(
            Func<T,CancellationToken,Task<bool>> update, bool failIfMissing = false, CancellationToken cancelToken = default)
            where T : class, IMetadata<V1ObjectMeta> => ReplaceAsync<T>(null, update, failIfMissing, cancelToken);

        /// <summary>Performs an atomic get-modify-replace operation, using the GET method to read the object and the PUT method to
        /// replace it.
        /// </summary>
        /// <param name="obj">The initial value of the resource, or null if it should be retrieved with a GET request</param>
        /// <param name="modify">A function that modifies the resource, returning true if any changes were made and false if not</param>
        /// <param name="failIfMissing">If true, an exception will be thrown if the object doesn't exist. If false, null will be
        /// returned in that case.
        /// </param>
        /// <param name="cancelToken">A <see cref="CancellationToken"/> that can be used to cancel the request</param>
        public Task<T> ReplaceAsync<T>(T obj, Func<T,bool> modify, bool failIfMissing = false, CancellationToken cancelToken = default)
            where T : class
        {
            if(modify == null) throw new ArgumentNullException(nameof(modify));
            return ReplaceAsync(obj, (o,ct) => Task.FromResult(modify(o)), failIfMissing, cancelToken);
        }

        /// <summary>Performs an atomic get-modify-replace operation, using the GET method to read the object and the PUT method to
        /// replace it.
        /// </summary>
        /// <param name="obj">The initial value of the resource, or null if it should be retrieved with a GET request</param>
        /// <param name="modify">A function that modifies the resource, returning true if any changes were made and false if not</param>
        /// <param name="failIfMissing">If true, an exception will be thrown if the object doesn't exist. If false, null will be
        /// returned in that case.
        /// </param>
        /// <param name="cancelToken">A <see cref="CancellationToken"/> that can be used to cancel the request</param>
        public async Task<T> ReplaceAsync<T>(
            T obj, Func<T,CancellationToken,Task<bool>> modify, bool failIfMissing = false, CancellationToken cancelToken = default)
            where T : class
        {
            if(modify == null) throw new ArgumentNullException(nameof(modify));
            if(_watchVersion != null) throw new InvalidOperationException("Watches cannot be updated.");
            while(true)
            {
                if(obj == null) // if we need to load the resource...
                {
                    cancelToken.ThrowIfCancellationRequested();
                    HttpRequestMessage getMsg = CreateRequestMessage(); // load it with a GET request
                    getMsg.Method = HttpMethod.Get;
                    obj = await ExecuteMessageAsync<T>(getMsg, failIfMissing, cancelToken).ConfigureAwait(false);
                }
                cancelToken.ThrowIfCancellationRequested();
                // if the resource is missing or no changes are needed, return it as-is
                if(obj == null || !await modify(obj, cancelToken).ConfigureAwait(false)) return obj;
                HttpRequestMessage updateMsg = CreateRequestMessage(); // otherwise, update it with a PUT request
                updateMsg.Method = HttpMethod.Put;
                KubernetesResponse resp = new KubernetesResponse(await client.SendAsync(updateMsg, cancelToken).ConfigureAwait(false));
                if(resp.StatusCode != HttpStatusCode.Conflict) // if there was no conflict, return the result
                {
                    if(resp.IsNotFound && !failIfMissing) return null;
                    else if(resp.IsError) throw new KubernetesException(await resp.GetErrorAsync().ConfigureAwait(false));
                    else return await resp.GetBodyAsync<T>().ConfigureAwait(false);
                }
                obj = null; // otherwise, there was a conflict, so reload the item
            }
        }

        /// <summary>Gets the <see cref="KubernetesScheme"/> used to map types to their Kubernetes groups, version, and kinds.</summary>
        public KubernetesScheme Scheme() => _scheme;

        /// <summary>Sets the <see cref="KubernetesScheme"/> used to map types to their Kubernetes groups, version, and kinds, or null to
        /// use the <see cref="KubernetesScheme.Default"/> scheme.
        /// </summary>
        public KubernetesRequest Scheme(KubernetesScheme scheme) { _scheme = scheme ?? KubernetesScheme.Default; return this; }

        /// <summary>Attempts to set the <see cref="Group()"/>, <see cref="Version()"/>, <see cref="Type()"/>, <see cref="Namespace()"/>,
        /// <see cref="Name()"/>, and optionally the <see cref="Body()"/> based on the given object.
        /// </summary>
        /// <remarks>If the object implements <see cref="IMetadata{T}"/> of <see cref="V1ObjectMeta"/>, it will be used to set the
        /// <see cref="Name()"/> and <see cref="Namespace()"/>. The <see cref="Name()"/> will be set if <see cref="V1ObjectMeta.Uid"/>
        /// is set (on the assumption that you're accessing an existing object), and cleared it's clear (on the assumption that you're
        /// creating a new object and want to POST to its container).
        /// </remarks>
        public KubernetesRequest Set(IKubernetesObject obj, bool setBody = true)
        {
            GVK(obj);
            if(setBody) Body(obj);
            var kobj = obj as IMetadata<V1ObjectMeta>;
            if(kobj != null) Namespace(kobj.Namespace()).Name(!string.IsNullOrEmpty(kobj.Uid()) ? kobj.Name() : null);
            return this;
        }

        /// <summary>Sets a custom header value, or deletes it if the value is null.</summary>
        public KubernetesRequest SetHeader(string headerName, string value)
        {
            if(headerName == "Accept" || headerName == "Content-Type")
            {
                throw new ArgumentException("The header must be set using the corresponding property.");
            }
            return Set(ref headers, headerName, value);
        }

        /// <summary>Sets a query-string value, or deletes it if the value is null.</summary>
        public KubernetesRequest SetQuery(string key, string value) => Set(ref query, key, value);

        /// <summary>Sets the <see cref="Subresource()"/> to "status", to get or set a resource's status.</summary>
        public KubernetesRequest Status() => Subresource("status");

        /// <summary>Gets whether the response must be streamed. If true, the response will be returned from <see cref="Execute"/>
        /// as soon as the headers are read and you will have to dispose the response. Otherwise, the entire response will be downloaded
        /// before <see cref="Execute"/> returns, and you will not have to dispose it. Note that regardless of the
        /// value of this property, the response is always streamed when <see cref="WatchVersion()"/> is not null.
        /// </summary>
        public bool StreamResponse() => _streamResponse;

        /// <summary>Sets whether the response must be streamed. If true, the response will be returned from <see cref="Execute"/>
        /// as soon as the headers are read and you will have to dispose the response. Otherwise, the entire response will be downloaded
        /// before <see cref="Execute"/> returns, and you will not have to dispose it. The default is false. Note that regardless of the
        /// value of this property, the response is always streamed when <see cref="WatchVersion()"/> is not null.
        /// </summary>
        public KubernetesRequest StreamResponse(bool stream) { _streamResponse = stream; return this; }

        /// <summary>Gets the URL-encoded subresource to access, or null to not access a subresource.</summary>
        public string Subresource() => _subresource;

        /// <summary>Sets the subresource to access, or null or empty to not access a subresource. The value must be URL-encoded
        /// already if necessary.
        /// </summary>
        public KubernetesRequest Subresource(string subresource) { _subresource = NormalizeEmpty(subresource); return this; }

        /// <summary>Sets the value of the <see cref="Subresource(string)"/> by joining together one or more path segments. The
        /// segments will be URL-escaped (and so should not be URL-escaped already).
        /// </summary>
        public KubernetesRequest Subresources(params string[] subresources) =>
            Subresource(subresources != null && subresources.Length != 0 ?
                string.Join("/", subresources.Select(Uri.EscapeDataString)) : null);

        /// <inheritdoc/>
        public override string ToString() => Method().Method + " " + GetRequestUri();

        /// <summary>Gets the resource type access (e.g. "pods").</summary>
        public string Type() => _type;

        /// <summary>Sets the resource type access (e.g. "pods").</summary>
        public KubernetesRequest Type(string type) { _type = NormalizeEmpty(type); return this; }

        /// <summary>Gets the Kubernetes API version to use, or null to use the default, which is "v1"
        /// unless a <see cref="RawUri()"/> is given.
        /// </summary>
        public string Version() => _version;

        /// <summary>Sets the Kubernetes API version to use, or null or empty to use the default, which is "v1"
        /// unless a <see cref="RawUri()"/> is given.
        /// </summary>
        public KubernetesRequest Version(string version) { _version = NormalizeEmpty(version); return this; }

        /// <summary>Gets the resource version to use when watching a resource, or empty to watch the current version, or null
        /// to not execute a watch.
        /// </summary>
        public string WatchVersion() => _watchVersion;

        /// <summary>Sets the resource version to use when watching a resource, or empty to watch the current version, or null to not
        /// execute a watch. The default is null. When set, the response is always streamed (as though <see cref="StreamResponse()"/>
        /// was true).
        /// </summary>
        public KubernetesRequest WatchVersion(string resourceVersion) { _watchVersion = resourceVersion; return this; }

        /// <summary>Adds a value to the query string or headers.</summary>
        KubernetesRequest Add(ref Dictionary<string,List<string>> dict, string key, string value)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if(dict == null) dict = new Dictionary<string, List<string>>();
            if(!dict.TryGetValue(key, out List<string> values)) dict[key] = values = new List<string>();
            values.Add(value);
            return this;
        }

        /// <summary>Sets a value in the query string or headers.</summary>
        KubernetesRequest Set(ref Dictionary<string,List<string>> dict, string key, string value)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            dict = dict ?? new Dictionary<string, List<string>>();
            if(!dict.TryGetValue(key, out List<string> values)) dict[key] = values = new List<string>();
            values.Clear();
            values.Add(value);
            return this;
        }

        /// <summary>Creates an <see cref="HttpRequestMessage"/> representing the current request.</summary>
        HttpRequestMessage CreateRequestMessage()
        {
            var req = new HttpRequestMessage(Method(), GetRequestUri());

            // add the headers
            if(!string.IsNullOrEmpty(config.AccessToken))
            {
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.AccessToken);
            }
            else if(!string.IsNullOrEmpty(config.Username))
            {
                req.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(config.Username + ":" + config.Password)));
            }

            if(_accept != null) req.Headers.Add("Accept", _accept);
            List<KeyValuePair<string,List<string>>> contentHeaders = null;
            if(headers != null && headers.Count != 0) // add custom headers
            {
                contentHeaders = new List<KeyValuePair<string,List<string>>>(); // some headers must be added to .Content.Headers. track them
                foreach(KeyValuePair<string,List<string>> pair in headers)
                {
                    if(!req.Headers.TryAddWithoutValidation(pair.Key, pair.Value)) // if it's not legal to set this header on the request...
                    {
                        contentHeaders.Add(new KeyValuePair<string,List<string>>(pair.Key, pair.Value)); // assume we should set it on the content
                        break;
                    }
                }
            }

            // add the body, if any
            if(_body != null)
            {
                if(_body is byte[] bytes) req.Content = new ByteArrayContent(bytes);
                else if(_body is Stream stream) req.Content = new StreamContent(stream);
                else
                {
                    req.Content = new StringContent(
                        _body as string ?? JsonConvert.SerializeObject(_body, Kubernetes.DefaultJsonSettings), Encoding.UTF8);
                }
                req.Content.Headers.ContentType = new MediaTypeHeaderValue(_mediaType ?? "application/json") { CharSet = "UTF-8" };
                if(contentHeaders != null && contentHeaders.Count != 0) // go through the headers we couldn't set on the request
                {
                    foreach(KeyValuePair<string,List<string>> pair in contentHeaders)
                    {
                        if(!req.Content.Headers.TryAddWithoutValidation(pair.Key, pair.Value)) // if we can't set it on the content either...
                        {
                            throw new InvalidOperationException($"{pair.Value} is a response header and cannot be set on the request.");
                        }
                    }
                }
            }
            return req;
        }

        async Task<T> ExecuteMessageAsync<T>(HttpRequestMessage msg, bool failIfMissing, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            KubernetesResponse resp = new KubernetesResponse(await client.SendAsync(msg, cancelToken).ConfigureAwait(false));
            if(resp.IsNotFound && !failIfMissing) return default(T);
            else if(resp.IsError) throw new KubernetesException(await resp.GetErrorAsync().ConfigureAwait(false));
            else return await resp.GetBodyAsync<T>().ConfigureAwait(false);
        }

        string GetRequestUri()
        {
            if(_rawUri != null && (_group ?? _name ?? _ns ?? _subresource ?? _type ?? _version) != null)
            {
                throw new InvalidOperationException("You cannot use both raw and piecemeal URIs.");
            }

            // construct the request URL
            var sb = new StringBuilder();
            sb.Append(config.Host);
            if(sb[sb.Length-1] != '/') sb.Append('/');
            if(_rawUri != null) // if a raw URL was given, use it
            {
                sb.Append(_rawUri);
            }
            else // otherwise, construct it piecemeal
            {
                if(_group != null) sb.Append("apis/").Append(_group);
                else sb.Append("api");
                sb.Append('/').Append(_version ?? "v1");
                if(_ns != null) sb.Append("/namespaces/").Append(_ns);
                sb.Append('/').Append(_type);
                if(_name != null) sb.Append('/').Append(_name);
                if(_subresource != null) sb.Append('/').Append(_subresource);
            }
            if(query != null) // then add the query string, if any
            {
                bool first = true;
                foreach(KeyValuePair<string,List<string>> pair in query)
                {
                    string key = Uri.EscapeDataString(pair.Key);
                    foreach(string value in pair.Value)
                    {
                        sb.Append(first ? '?' : '&').Append(key).Append('=');
                        if(!string.IsNullOrEmpty(value)) sb.Append(Uri.EscapeDataString(value));
                        first = false;
                    }
                }
                if(_watchVersion != null)
                {
                    sb.Append(first ? '?' : '&').Append("watch=1");
                    if(_watchVersion.Length != 0) sb.Append("&resourceVersion=").Append(_watchVersion);
                }
            }
            return sb.ToString();
        }

        object ICloneable.Clone() => Clone();

        readonly HttpClient client;
        readonly KubernetesClientConfiguration config;
        Dictionary<string, List<string>> headers, query;
        string _accept = "application/json", _mediaType = "application/json";
        string _group, _name, _ns, _rawUri, _subresource, _type, _version, _watchVersion;
        object _body;
        HttpMethod _method;
        KubernetesScheme _scheme;
        bool _streamResponse;

        static string NormalizeEmpty(string value) => string.IsNullOrEmpty(value) ? null : value; // normalizes empty strings to null
    }
    #endregion

    #region KubernetesResponse
    /// <summary>Represents a response to a <see cref="KubernetesRequest"/>.</summary>
    public sealed class KubernetesResponse : IDisposable
    {
        /// <summary>Initializes a new <see cref="KubernetesResponse"/> from an <see cref="HttpResponseMessage"/>.</summary>
        public KubernetesResponse(HttpResponseMessage message) => Message = message ?? throw new ArgumentNullException(nameof(message));

        /// <summary>Indicates whether the server returned an error response.</summary>
        public bool IsError => (int)StatusCode >= 400;

        /// <summary>Indicates whether the server returned a 404 Not Found response.</summary>
        public bool IsNotFound => StatusCode == HttpStatusCode.NotFound;

        /// <summary>Gets the underlying <see cref="HttpResponseMessage"/>.</summary>
        public HttpResponseMessage Message { get; }

        /// <summary>Gets the <see cref="HttpStatusCode"/> of the response.</summary>
        public HttpStatusCode StatusCode => Message.StatusCode;

        /// <inheritdoc/>
        public void Dispose() => Message.Dispose();

        /// <summary>Deserializes the response body as a <see cref="V1Status"/> object, or creates one from the status code if the
        /// response body is not a JSON object.
        /// </summary>
        public async Task<V1Status> GetErrorAsync()
        {
            try { return await GetBodyAsync<V1Status>().ConfigureAwait(false); }
            catch(JsonException) { }
            return new V1Status()
            {
                Status = IsError ? "Failure" : "Success", Code = (int)StatusCode, Reason = StatusCode.ToString(), Message = body
            };
        }

        /// <summary>Returns the response body as a string.</summary>
        public async Task<string> GetBodyAsync()
        {
            if(body == null)
            {
                body = Message.Content != null ? await Message.Content.ReadAsStringAsync().ConfigureAwait(false) : string.Empty;
            }
            return body;
        }

        /// <summary>Deserializes the response body from JSON as a value of the given type, or null if the response body is empty.</summary>
        /// <param name="type">The type of object to return</param>
        /// <param name="failIfEmpty">If false, an empty response body will be returned as null. If true, an exception will be thrown if
        /// the body is empty. The default is false.
        /// </param>
        public async Task<object> GetBodyAsync(Type type, bool failIfEmpty = false)
        {
            string body = await GetBodyAsync().ConfigureAwait(false);
            if(string.IsNullOrWhiteSpace(body))
            {
                if(!failIfEmpty) throw new InvalidOperationException("The response body was empty.");
                return null;
            }
            return JsonConvert.DeserializeObject(body, type, Kubernetes.DefaultJsonSettings);
        }

        /// <summary>Deserializes the response body from JSON as a value of type <typeparamref name="T"/>, or the default value of
        /// type <typeparamref name="T"/> if the response body is empty.
        /// </summary>
        /// <param name="failIfEmpty">If false, an empty response body will be returned as the default value of type
        /// <typeparamref name="T"/>. If true, an exception will be thrown if the body is empty. The default is false.
        /// </param>
        public async Task<T> GetBodyAsync<T>(bool failIfEmpty = false)
        {
            string body = await GetBodyAsync().ConfigureAwait(false);
            if(string.IsNullOrWhiteSpace(body))
            {
                if(failIfEmpty) throw new InvalidOperationException("The response body was empty.");
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(body, Kubernetes.DefaultJsonSettings);
        }

        string body;
    }
    #endregion
}
