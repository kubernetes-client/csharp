using System;
using System.Collections.Generic;
using System.IO;
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
        /// <param name="responseTask">the api response</param>
        /// <param name="onEvent">a callback when any event raised from api server</param>
        /// <param name="onError">a callbak when any exception was caught during watching</param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <returns>a watch object</returns>
        public static Watcher<T> Watch<T, L>(
            this Task<HttpOperationResponse<L>> responseTask,
            Action<WatchEventType, T> onEvent,
            Action<Exception> onError = null,
            Action onClosed = null)
        {
            return new Watcher<T>(MakeStreamReaderCreator<T, L>(responseTask), onEvent, onError, onClosed);
        }

        private static Func<Task<TextReader>> MakeStreamReaderCreator<T, L>(Task<HttpOperationResponse<L>> responseTask)
        {
            return async () =>
            {
                var response = await responseTask.ConfigureAwait(false);

                if (!(response.Response.Content is LineSeparatedHttpContent content))
                {
                    throw new KubernetesClientException("not a watchable request or failed response");
                }

                return content.StreamReader;
            };
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
        public static Watcher<T> Watch<T, L>(
            this HttpOperationResponse<L> response,
            Action<WatchEventType, T> onEvent,
            Action<Exception> onError = null,
            Action onClosed = null)
        {
            return Watch(Task.FromResult(response), onEvent, onError, onClosed);
        }

        /// <summary>
        /// create an IAsyncEnumerable from a call to api server with watch=true
        /// see https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8
        /// </summary>
        /// <typeparam name="T">type of the event object</typeparam>
        /// <typeparam name="L">type of the HttpOperationResponse object</typeparam>
        /// <param name="responseTask">the api response</param>
        /// <param name="onError">a callbak when any exception was caught during watching</param>
        /// <returns>IAsyncEnumerable of watch events</returns>
        public static IAsyncEnumerable<(WatchEventType, T)> WatchAsync<T, L>(
            this Task<HttpOperationResponse<L>> responseTask,
            Action<Exception> onError = null)
        {
            return Watcher<T>.CreateWatchEventEnumerator(MakeStreamReaderCreator<T, L>(responseTask), onError);
        }
    }
}
