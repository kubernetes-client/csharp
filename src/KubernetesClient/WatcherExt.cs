using System;
using System.Threading.Tasks;
using k8s.Exceptions;
using Microsoft.Rest;

namespace k8s
{
    public static class WatcherExt
    {
        /// <summary>
        /// create a watch object from a call to api server with watch=true
        /// </summary>
        /// <typeparam name="T">type of the event object</typeparam>
        /// <typeparam name="L">type of the HttpOperationResponse object</typeparam>
        /// <param name="response">the api response</param>
        /// <param name="onEvent">a callback when any event raised from api server</param>
        /// <param name="onError">a callbak when any exception was caught during watching</param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <returns>a watch object</returns>
        public static Watcher<T> Watch<T, L>(this Task<HttpOperationResponse<L>> responseTask,
            Action<WatchEventType, T> onEvent,
            Action<Exception> onError = null,
            Action onClosed = null)
        {
            return new Watcher<T>(async () =>
            {
                var response = await responseTask.ConfigureAwait(false);

                if (!(response.Response.Content is WatcherDelegatingHandler.LineSeparatedHttpContent content))
                {
                    throw new KubernetesClientException("not a watchable request or failed response");
                }

                return content.StreamReader;
            }, onEvent, onError, onClosed);
        }

        /// <summary>
        /// create a watch object from a call to api server with watch=true
        /// </summary>
        /// <typeparam name="T">type of the event object</typeparam>
        /// <typeparam name="L">type of the HttpOperationResponse object</typeparam>
        /// <param name="response">the api response</param>
        /// <param name="onEvent">a callback when any event raised from api server</param>
        /// <param name="onError">a callbak when any exception was caught during watching</param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <returns>a watch object</returns>
        public static Watcher<T> Watch<T, L>(this HttpOperationResponse<L> response,
            Action<WatchEventType, T> onEvent,
            Action<Exception> onError = null,
            Action onClosed = null)
        {
            return Watch(Task.FromResult(response), onEvent, onError, onClosed);
        }
    }
}
