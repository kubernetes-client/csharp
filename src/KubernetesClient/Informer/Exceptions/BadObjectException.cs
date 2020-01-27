using System;
using System.Runtime.Serialization;

namespace k8s.Informer.Exceptions
{
    public class BadObjectException : Exception
    {
        public BadObjectException()
        {
        }

        protected BadObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public BadObjectException(string message) : base(message)
        {
        }

        public BadObjectException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}