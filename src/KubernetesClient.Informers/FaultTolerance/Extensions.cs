using System;
using System.Net;
using System.Net.Http;
using Microsoft.Rest.TransientFaultHandling;

namespace k8s.Informers.FaultTolerance
{
    public static class Extensions
    {
        /// <summary>
        ///     Checks if the type of exception is the one that is temporary and will resolve itself over time
        /// </summary>
        /// <param name="exception">Exception to check</param>
        /// <returns>Return <see langword="true"> if exception is transient, or <see langword="false"> if it's not</returns>
        public static bool IsTransient(this Exception exception)
        {
            if (exception is HttpRequestWithStatusException statusException)
            {
                return statusException.StatusCode >= HttpStatusCode.ServiceUnavailable || statusException.StatusCode == HttpStatusCode.RequestTimeout;
            }

            if (exception is HttpRequestException || exception is KubernetesException)
            {
                return true;
            }

            return false;
        }
    }
}
