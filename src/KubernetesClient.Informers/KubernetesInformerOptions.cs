using System;

namespace k8s.Informers
{
    public class KubernetesInformerOptions // theoretically this could be done with QObservable, but parsing expression trees is too much overhead at this point
        : IEquatable<KubernetesInformerOptions>
    {
        public bool Equals(KubernetesInformerOptions other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Namespace == other.Namespace && LabelSelector == other.LabelSelector;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((KubernetesInformerOptions)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Namespace != null ? Namespace.GetHashCode() : 0) * 397) ^ (LabelSelector != null ? LabelSelector.GetHashCode() : 0);
            }
        }

        public static bool operator ==(KubernetesInformerOptions left, KubernetesInformerOptions right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(KubernetesInformerOptions left, KubernetesInformerOptions right)
        {
            return !Equals(left, right);
        }

        public static IKubernetesInformerOptionsBuilder Builder => new KubernetesInformerOptionsBuilder();

        internal KubernetesInformerOptions()
        {
        }

        /// <summary>
        ///     The default options for kubernetes informer, without any server side filters
        /// </summary>
        public static KubernetesInformerOptions Default { get; } = new KubernetesInformerOptions();

        /// <summary>
        ///     The namespace to which observable stream should be filtered
        /// </summary>
        public string Namespace { get; internal set; }
        public string LabelSelector { get; internal set; }
    }
}
