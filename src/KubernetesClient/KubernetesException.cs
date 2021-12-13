using k8s.Models;
using System.Runtime.Serialization;

namespace k8s
{
    /// <summary>
    /// Represents an error message returned by the Kubernetes API server.
    /// </summary>
    public class KubernetesException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesException"/> class.
        /// </summary>
        public KubernetesException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesException"/> class using
        /// the data from a <see cref="V1Status"/> object.
        /// </summary>
        /// <param name="status">
        /// A status message which triggered this exception to be thrown.
        /// </param>
        public KubernetesException(V1Status status)
            : this(status?.Message)
        {
            Status = status;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesException"/> class using
        /// the data from a <see cref="V1Status"/> object and a reference to the inner exception
        /// that is the cause of this exception..
        /// </summary>
        /// <param name="status">
        /// A status message which triggered this exception to be thrown.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or <see langword="null"/>
        /// if no inner exception is specified.
        /// </param>
        public KubernetesException(V1Status status, Exception innerException)
            : this(status?.Message, innerException)
        {
            Status = status;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesException"/> class with an error message.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        public KubernetesException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesException"/> class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or <see langword="null"/>
        /// if no inner exception is specified.
        /// </param>
        public KubernetesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information
        /// about the source or destination.
        /// </param>
        protected KubernetesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets, when this exception was raised because of a Kubernetes status message, the underlying
        /// Kubernetes status message.
        /// </summary>
        public V1Status Status { get; private set; }
    }
}
