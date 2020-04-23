namespace k8s.Exceptions
{
    using System;

    /// <summary>
    /// The exception that is thrown when the kube config is invalid
    /// </summary>
    public class KubeConfigException : Exception
    {
        public KubeConfigException()
        {
        }

        public KubeConfigException(string message)
            : base(message)
        {
        }

        public KubeConfigException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
