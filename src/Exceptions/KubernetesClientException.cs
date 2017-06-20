namespace k8s.Exceptions
{
    using System;

    /// <summary>
    /// The exception that is thrown when there is a client exception
    /// </summary>
    public class KubernetesClientException : Exception
    {
        public KubernetesClientException()
        {
        }

        public KubernetesClientException(string message)
        : base(message)
        {
        }

        public KubernetesClientException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
