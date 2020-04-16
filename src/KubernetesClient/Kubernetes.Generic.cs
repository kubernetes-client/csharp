using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using k8s.Models;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using static System.Net.HttpStatusCode;

namespace k8s
{
    public partial class Kubernetes
    {
        public async Task<HttpOperationResponse<KubernetesList<T>>> ListWithHttpMessagesAsync<T>(
            string namespaceParameter = default,
            bool? allowWatchBookmarks = default,
            string continueParameter = default,
            string fieldSelector = default,
            string labelSelector = default,
            int? limit = default,
            string resourceVersion = default,
            TimeSpan? timeout = default,
            bool? watch = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
            where T : IKubernetesObject
        {
            var result = await ListWithHttpMessagesAsync(
                typeof(T),
                namespaceParameter,
                allowWatchBookmarks,
                continueParameter,
                fieldSelector,
                labelSelector,
                limit,
                resourceVersion,
                timeout,
                watch,
                isPretty,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
            return (HttpOperationResponse<KubernetesList<T>>)result;
        }

        /// <inheritdoc cref="IKubernetes" />
        public async Task<HttpOperationResponse> ListWithHttpMessagesAsync(
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
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default) =>
            await SendStandardRequest(
                type,
                HttpMethod.Get,
                responseType: typeof(KubernetesList<>).MakeGenericType(type),
                namespaceParameter: namespaceParameter,
                allowWatchBookmarks: allowWatchBookmarks,
                continueParameter: continueParameter,
                fieldSelector: fieldSelector,
                labelSelector: labelSelector,
                limit: limit,
                resourceVersion: resourceVersion,
                timeout: timeout,
                watch: watch,
                isPretty: isPretty,
                customHeaders: customHeaders,
                cancellationToken: cancellationToken).ConfigureAwait(false);

        /// <inheritdoc cref="IKubernetes" />
        public async Task<HttpOperationResponse<T>> ReadWithHttpMessagesAsync<T>(
            string name,
            string namespaceParameter = default,
            bool? isExact = default,
            bool? isExport = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = default,
            CancellationToken cancellationToken = default)
            where T : IKubernetesObject
        {
            var result = await ReadWithHttpMessagesAsync(typeof(T), name, namespaceParameter, isExact, isExport, isPretty, customHeaders, cancellationToken)
                .ConfigureAwait(false);
            return (HttpOperationResponse<T>)result;
        }

        /// <inheritdoc cref="IKubernetes" />
        public async Task<HttpOperationResponse> ReadWithHttpMessagesAsync(
            Type type,
            string name,
            string namespaceParameter = default,
            bool? isExact = default,
            bool? isExport = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return await SendStandardRequest(
                type,
                HttpMethod.Get,
                name: name,
                namespaceParameter: namespaceParameter,
                exact: isExact,
                export: isExport,
                isPretty: isPretty,
                customHeaders: customHeaders,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IKubernetes" />
        public async Task<HttpOperationResponse<T>> CreateWithHttpMessagesAsync<T>(
            T body,
            string namespaceParameter = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = default,
            CancellationToken cancellationToken = default)
            where T : IKubernetesObject
        {
            var result = await CreateWithHttpMessagesAsync((object)body, namespaceParameter, dryRun, fieldManager, isPretty, customHeaders, cancellationToken).ConfigureAwait(false);
            return (HttpOperationResponse<T>)result;
        }

        /// <inheritdoc cref="IKubernetes" />
        public async Task<HttpOperationResponse> CreateWithHttpMessagesAsync(
            object body,
            string namespaceParameter = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            return await SendStandardRequest(
                body.GetType(),
                body: body,
                httpMethod: HttpMethod.Post,
                namespaceParameter: namespaceParameter,
                dryRun: dryRun,
                fieldManager: fieldManager,
                isPretty: isPretty,
                customHeaders: customHeaders,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IKubernetes" />
        public async Task<HttpOperationResponse<V1Status>> DeleteWithHttpMessagesAsync<TResource>(
            TResource resource,
            V1DeleteOptions body = default,
            DryRun? dryRun = default,
            TimeSpan? gracePeriod = default,
            bool? orphanDependents = default,
            PropagationPolicy? propagationPolicy = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
            where TResource : IKubernetesObject<V1ObjectMeta>
        {
            return await DeleteWithHttpMessagesAsync<TResource>(
                resource.Metadata.Name,
                resource.Metadata.NamespaceProperty,
                body,
                dryRun,
                gracePeriod,
                orphanDependents,
                propagationPolicy,
                isPretty,
                customHeaders,
                cancellationToken);
        }


        /// <inheritdoc cref="IKubernetes" />
        public async Task<HttpOperationResponse<V1Status>> DeleteWithHttpMessagesAsync<TResource>(
            string name = default,
            string namespaceParameter = default,
            V1DeleteOptions body = default,
            DryRun? dryRun = default,
            TimeSpan? gracePeriod = default,
            bool? orphanDependents = default,
            PropagationPolicy? propagationPolicy = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
            where TResource : IKubernetesObject
        {
            var result = await DeleteWithHttpMessagesAsync(
                typeof(TResource),
                name,
                namespaceParameter,
                body,
                dryRun,
                gracePeriod,
                orphanDependents,
                propagationPolicy,
                isPretty,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc cref="IKubernetes" />
        public async Task<HttpOperationResponse<V1Status>> DeleteWithHttpMessagesAsync(
            Type type,
            string name = default,
            string namespaceParameter = default,
            V1DeleteOptions body = default,
            DryRun? dryRun = default,
            TimeSpan? gracePeriod = default,
            bool? orphanDependents = default,
            PropagationPolicy? propagationPolicy = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var result = await SendStandardRequest(
                type,
                body: body,
                httpMethod: HttpMethod.Delete,
                responseType: typeof(V1Status),
                namespaceParameter: namespaceParameter,
                dryRun: dryRun,
                gracePeriod: gracePeriod,
                orphanDependents: orphanDependents,
                propagationPolicy: propagationPolicy,
                isPretty: isPretty,
                customHeaders: customHeaders,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return (HttpOperationResponse<V1Status>)result;
        }

        /// <inheritdoc cref="IKubernetes" />
        public async Task<HttpOperationResponse<T>> PatchWithHttpMessagesAsync<T>(
            V1Patch body,
            string name,
            string namespaceParameter = default,
            bool? statusOnly = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            bool? force = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
            where T : IKubernetesObject
        {
            var result = await PatchWithHttpMessagesAsync(typeof(T), body, name, namespaceParameter, statusOnly, dryRun, fieldManager, force, isPretty, customHeaders, cancellationToken)
                .ConfigureAwait(false);
            return (HttpOperationResponse<T>)result;
        }

        /// <inheritdoc cref="IKubernetes" />
        public async Task<HttpOperationResponse> PatchWithHttpMessagesAsync(
            Type type,
            V1Patch body,
            string name,
            string namespaceParameter = default,
            bool? statusOnly = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            bool? force = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return await SendStandardRequest(
                type,
                body: body,
                httpMethod: new HttpMethod("PATCH"),
                name: name,
                statusOnly: statusOnly,
                namespaceParameter: namespaceParameter,
                dryRun: dryRun,
                fieldManager: fieldManager,
                force: force,
                isPretty: isPretty,
                customHeaders: customHeaders,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }


        /// <inheritdoc cref="IKubernetes" />
        public async Task<HttpOperationResponse<T>> ReplaceWithHttpMessagesAsync<T>(
            T body,
            bool? statusOnly = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
            where T : IKubernetesObject<V1ObjectMeta>
        {
            return await ReplaceWithHttpMessagesAsync(body, body?.Metadata?.Name, body.Metadata.NamespaceProperty, statusOnly, dryRun, fieldManager, isPretty, customHeaders, cancellationToken).ConfigureAwait(false);
        }


        /// <inheritdoc cref="IKubernetes" />
        public async Task<HttpOperationResponse<T>> ReplaceWithHttpMessagesAsync<T>(
            T body,
            string name,
            string namespaceParameter = default,
            bool? statusOnly = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            var result = await ReplaceWithHttpMessagesAsync((object)body, name, namespaceParameter, statusOnly, dryRun, fieldManager, isPretty, customHeaders, cancellationToken).ConfigureAwait(false);
            return (HttpOperationResponse<T>)result;
        }

        /// <inheritdoc cref="IKubernetes" />
        public async Task<HttpOperationResponse> ReplaceWithHttpMessagesAsync(
            object body,
            string name,
            string namespaceParameter = default,
            bool? statusOnly = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return await SendStandardRequest(
                body.GetType(),
                body: body,
                httpMethod: HttpMethod.Put,
                name: name,
                statusOnly: statusOnly,
                namespaceParameter: namespaceParameter,
                dryRun: dryRun,
                fieldManager: fieldManager,
                isPretty: isPretty,
                customHeaders: customHeaders,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private object DeserializeObject(Type type, string json)
        {
            var jsonSerializer = JsonSerializer.Create(DeserializationSettings);
            jsonSerializer.CheckAdditionalContent = true;
            using (var jsonTextReader = new JsonTextReader(new StringReader(json)))
            {
                return jsonSerializer.Deserialize(jsonTextReader, type);
            }
        }

        private async Task<HttpOperationResponse> SendStandardRequest(
            Type resourceType,
            HttpMethod httpMethod,
            IList<HttpStatusCode> validResponseCodes = default,
            Type responseType = default,
            object body = default,
            string namespaceParameter = default,
            string name = default,
            bool? statusOnly = default,
            bool? export = default,
            bool? exact = default,
            TimeSpan? gracePeriod = default,
            bool? orphanDependents = default,
            PropagationPolicy? propagationPolicy = default,
            DryRun? dryRun = default,
            string fieldManager = default,
            bool? force = default,
            bool? allowWatchBookmarks = default,
            string continueParameter = default,
            string fieldSelector = default,
            string labelSelector = default,
            int? limit = default,
            string resourceVersion = default,
            TimeSpan? timeout = default,
            bool? watch = default,
            bool? isPretty = default,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            if (resourceType == null)
            {
                throw new ArgumentNullException(nameof(resourceType));
            }

            if (httpMethod == null)
            {
                throw new ArgumentNullException(nameof(httpMethod));
            }

            if (validResponseCodes == null || validResponseCodes.Count == 0)
            {
                validResponseCodes = new List<HttpStatusCode>();
                validResponseCodes.Add(OK);
                validResponseCodes.Add(Unauthorized);
                if (httpMethod.In(HttpMethod.Post, HttpMethod.Put))
                {
                    validResponseCodes.Add(Created);
                }

                if (httpMethod.In(HttpMethod.Post, HttpMethod.Delete))
                {
                    validResponseCodes.Add(Accepted);
                }
            }

            // Tracing
            var parameters = new Dictionary<string, object>
            {
                { nameof(resourceType), resourceType },
                { nameof(httpMethod), httpMethod },
                { nameof(validResponseCodes), validResponseCodes },
                { nameof(responseType), responseType },
                { nameof(body), body },
                { nameof(namespaceParameter), namespaceParameter },
                { nameof(name), name },
                { nameof(statusOnly), statusOnly },
                { nameof(export), export },
                { nameof(exact), exact },
                { nameof(gracePeriod), gracePeriod },
                { nameof(orphanDependents), orphanDependents },
                { nameof(propagationPolicy), propagationPolicy },
                { nameof(dryRun), dryRun },
                { nameof(fieldManager), fieldManager },
                { nameof(force), force },
                { nameof(allowWatchBookmarks), allowWatchBookmarks },
                { nameof(continueParameter), continueParameter },
                { nameof(fieldSelector), fieldSelector },
                { nameof(labelSelector), labelSelector },
                { nameof(limit), limit },
                { nameof(resourceVersion), resourceVersion },
                { nameof(timeout), timeout },
                { nameof(watch), watch },
                { nameof(isPretty), isPretty },
                { nameof(customHeaders), customHeaders },
                { nameof(cancellationToken), cancellationToken },
            };
            foreach (var parameterName in parameters.Where(x => x.Value == null).Select(x => x.Key).ToList())
            {
                parameters.Remove(parameterName);
            }

            var invocationId = AddTracing(parameters);
            var shouldTrace = invocationId != null;

            var entityAttribute = resourceType.GetKubernetesTypeMetadata().Validate();
            var isLegacy = string.IsNullOrEmpty(entityAttribute.Group);
            var segments = new List<string> { BaseUri.AbsoluteUri.Trim('/') };
            if (isLegacy)
            {
                segments.Add("api");
            }
            else
            {
                segments.Add("apis");
                segments.Add(entityAttribute.Group);
            }

            segments.Add(entityAttribute.ApiVersion);

            if (!string.IsNullOrEmpty(namespaceParameter))
            {
                segments.Add("namespaces");
                segments.Add(Uri.EscapeDataString(namespaceParameter));
            }

            segments.Add(entityAttribute.PluralName);
            if (!string.IsNullOrEmpty(name))
            {
                segments.Add(Uri.EscapeDataString(name));
            }

            if (statusOnly.GetValueOrDefault())
            {
                segments.Add("status");
            }

            var url = string.Join("/", segments);

            var sb = new StringBuilder();
            Utilities.AddQueryParameter(sb, "export", export);
            Utilities.AddQueryParameter(sb, "exact", exact);
            Utilities.AddQueryParameter(sb, "gracePeriodSeconds", (long?)gracePeriod?.TotalSeconds);
            Utilities.AddQueryParameter(sb, "orphanDependents", orphanDependents);
            Utilities.AddQueryParameter(sb, "propagationPolicy", propagationPolicy.ToString());
            Utilities.AddQueryParameter(sb, "dryRun", dryRun.HasValue ? dryRun.ToString().ToCamelCase() : null);
            Utilities.AddQueryParameter(sb, "fieldManager", fieldManager);
            Utilities.AddQueryParameter(sb, "force", force);
            Utilities.AddQueryParameter(sb, "allowWatchBookmarks", allowWatchBookmarks);
            Utilities.AddQueryParameter(sb, "continue", continueParameter);
            Utilities.AddQueryParameter(sb, "fieldSelector", fieldSelector);
            Utilities.AddQueryParameter(sb, "labelSelector", labelSelector);
            Utilities.AddQueryParameter(sb, "limit", limit);
            Utilities.AddQueryParameter(sb, "resourceVersion", resourceVersion);
            Utilities.AddQueryParameter(sb, "timeoutSeconds", (long?)timeout?.TotalSeconds);
            Utilities.AddQueryParameter(sb, "watch", watch);
            Utilities.AddQueryParameter(sb, "pretty", isPretty);

            url = string.Concat(url, sb);
            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage { Method = httpMethod, RequestUri = new Uri(url) };

            // Set Headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Serialize Request
            string requestContent = null;
            if (body != null)
            {
                (body as IValidate)?.Validate();
                requestContent = SafeJsonConvert.SerializeObject(body, SerializationSettings);
                httpRequest.Content = new StringContent(requestContent, Encoding.UTF8);
                if (httpMethod.Method != "PATCH")
                {
                    httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                }
                else
                {
                    httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json-patch+json; charset=utf-8");
                }
            }

            if (Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }

            // Send Request
            if (shouldTrace)
            {
                ServiceClientTracing.SendRequest(invocationId, httpRequest);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponse);
            }

            var statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string responseContent;

            if (!validResponseCodes.Contains(statusCode))
            {
                var ex = new HttpOperationException($"Operation returned an invalid status code '{statusCode}'");
                if (httpResponse.Content != null)
                {
                    responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    responseContent = string.Empty;
                }

                ex.Request = new HttpRequestMessageWrapper(httpRequest, requestContent);
                ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                if (shouldTrace)
                {
                    ServiceClientTracing.Error(invocationId, ex);
                }

                httpRequest.Dispose();
                httpResponse?.Dispose();
                throw ex;
            }

            if (responseType == null)
            {
                responseType = resourceType;
            }

            var result = (HttpOperationResponse)Activator.CreateInstance(typeof(HttpOperationResponse<>).MakeGenericType(responseType ?? resourceType));
            result.Request = httpRequest;
            result.Response = httpResponse;
            responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(responseContent))
            {
                try
                {
                    result.GetType().GetProperty("Body").SetValue(result, DeserializeObject(responseType, responseContent));
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    httpResponse?.Dispose();

                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }

            if (shouldTrace)
            {
                ServiceClientTracing.Exit(invocationId, result);
            }

            return result;
        }

        private string AddTracing(IDictionary<string, object> parameters)
        {
            var stackTrace = new StackTrace();
            var callingMethodName = stackTrace.GetFrame(1).GetMethod().Name;
            var shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString();
                ServiceClientTracing.Enter(invocationId, this, callingMethodName, parameters);
            }

            return invocationId;
        }
    }
}
