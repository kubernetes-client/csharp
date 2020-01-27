using System;

namespace k8s.Controllers.Reconciler
{
    public class Result
    {
        protected bool Equals(Result other)
        {
            return ShouldRequeue == other.ShouldRequeue && RequeueAfter.Equals(other.RequeueAfter);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Result) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ShouldRequeue.GetHashCode() * 397) ^ RequeueAfter.GetHashCode();
            }
        }

        public Result(bool shouldRequeue)
        {
            ShouldRequeue = shouldRequeue;
        }

        public Result(bool shouldRequeue, TimeSpan requeueAfter)
        {
            ShouldRequeue = shouldRequeue;
            RequeueAfter = requeueAfter;
        }

        public bool ShouldRequeue { get; set; }
        public TimeSpan RequeueAfter { get; set; }

    }
}