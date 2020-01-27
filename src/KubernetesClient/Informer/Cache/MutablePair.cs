using System.Collections.Generic;

namespace k8s.Informer.Cache
{
    public class MutablePair<TLeft,TRight>
    {
        protected bool Equals(MutablePair<TLeft, TRight> other)
        {
            return EqualityComparer<TLeft>.Default.Equals(Left, other.Left) && EqualityComparer<TRight>.Default.Equals(Right, other.Right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MutablePair<TLeft, TRight>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TLeft>.Default.GetHashCode(Left) * 397) ^ EqualityComparer<TRight>.Default.GetHashCode(Right);
            }
        }

        public MutablePair()
        {
        }

        public MutablePair(TLeft left, TRight right)
        {
            Left = left;
            Right = right;
        }

        public TLeft Left { get; set; }
        public TRight Right { get; set; }
    }
}