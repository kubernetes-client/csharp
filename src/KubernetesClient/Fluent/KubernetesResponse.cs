using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using k8s.Models;
using Newtonsoft.Json;

namespace k8s.Fluent
{
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

        /// <summary>Returns the response body as a string.</summary>
        public async Task<string> GetBodyAsync()
        {
            if (body == null)
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
            if (string.IsNullOrWhiteSpace(body))
            {
                if (!failIfEmpty) throw new InvalidOperationException("The response body was empty.");
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
            if (string.IsNullOrWhiteSpace(body))
            {
                if (failIfEmpty) throw new InvalidOperationException("The response body was empty.");
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(body, Kubernetes.DefaultJsonSettings);
        }

        /// <summary>Deserializes the response body as a <see cref="V1Status"/> object, or creates one from the status code if the
        /// response body is not a JSON object.
        /// </summary>
        public async Task<V1Status> GetStatusAsync()
        {
            try
            {
                var status = await GetBodyAsync<V1Status>().ConfigureAwait(false);
                if (status != null && (status.Status == "Success" || status.Status == "Failure")) return status;
            }
            catch (JsonException) { }
            return new V1Status()
            {
                Status = IsError ? "Failure" : "Success", Code = (int)StatusCode, Reason = StatusCode.ToString(), Message = body
            };
        }

        string body;
    }
}
