using k8s.Informers.Notifications;

namespace k8s.Tests.Utils
{
    public struct ScheduledEvent<T>
    {
        public ResourceEvent<T> Event { get; set; }
        public long ScheduledAt { get; set; }
        public override string ToString()
        {
            return $"\n   T{ScheduledAt}: {Event.ToString().Replace("\r\n", string.Empty).Trim()}";
        }
    }
}
