using System;
using System.Collections.Generic;
using k8s.Models;

namespace k8s.Informers
{
    public class KubernetesNameVersionEqualityComparer<T> : IEqualityComparer<T> where T : IMetadata<V1ObjectMeta>
    {
        private KubernetesNameVersionEqualityComparer()
        {
        }

        public static KubernetesNameVersionEqualityComparer<T> Instance => new KubernetesNameVersionEqualityComparer<T>();

        public bool Equals(T x, T y)
        {
            if (x?.Metadata?.Name == null || y?.Metadata?.Name == null || x.Metadata.ResourceVersion == null || y.Metadata.ResourceVersion == null)
            {
                return false;
            }
            return x.Metadata.Name.Equals(y.Metadata.Name) && x.Metadata.ResourceVersion.Equals(y.Metadata.ResourceVersion);
        }

        public int GetHashCode(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            unchecked
            {
                if (obj.Metadata?.Name == null || obj.Metadata?.ResourceVersion == null)
                {
                    return 0;
                }
                var hashCode = obj.Metadata.Name.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Metadata.ResourceVersion.GetHashCode();
                return hashCode;
            }
        }
    }
}
