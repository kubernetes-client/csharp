namespace k8s.Util.Common
{
    public class BadNotificationException : Exception
    {
        public BadNotificationException()
        {
        }

        public BadNotificationException(string message)
            : base(message)
        {
        }
    }
}
