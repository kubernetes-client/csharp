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
        public async Task<Watcher<T>> WatchObjectAsync<T>(string path, string @continue = null, string fieldSelector = null, bool? includeUninitialized = null, string labelSelector = null, int? limit = null, bool? pretty = null, int? timeoutSeconds = null, string resourceVersion = null, Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, T> onEvent = null, Action<Exception> onError = null, Action onClosed = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("path", path);
                tracingParameters.Add("continue", @continue);
                tracingParameters.Add("fieldSelector", fieldSelector);
                tracingParameters.Add("includeUninitialized", includeUninitialized);
                tracingParameters.Add("labelSelector", labelSelector);
                tracingParameters.Add("limit", limit);
                tracingParameters.Add("pretty", pretty);
                tracingParameters.Add("timeoutSeconds", timeoutSeconds);
                tracingParameters.Add("resourceVersion", resourceVersion);
                ServiceClientTracing.Enter(_invocationId, this, nameof(WatchObjectAsync), tracingParameters);
            }

            // Construct URL
            var uriBuilder = new UriBuilder(this.BaseUri);
            if (!uriBuilder.Path.EndsWith("/"))
            {
                uriBuilder.Path += "/";
            }

            uriBuilder.Path += path;

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
                query = QueryHelpers.AddQueryString(query, "resourceVersion", resourceVersion);
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

            return new Watcher<T>(reader, onEvent, onError, onClosed);
        }
    }
}
