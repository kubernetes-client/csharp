using System;
using System.Runtime.Serialization;

namespace k8s.Informer.Exceptions
{
    public class BadNotificationException : Exception
    {
        public BadNotificationException()
        {
        }

        protected BadNotificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public BadNotificationException(string message) : base(message)
        {
        }

        public BadNotificationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}