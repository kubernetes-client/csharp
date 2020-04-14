using System;

namespace k8s.CustomResources
{
    /// <summary>
    /// A builder for creating CRDs
    /// </summary>
    public interface ICustomResourceDefinitionBuilder : ICustomResourceDefinitionBuilderFinal
    {
        /// <summary>
        /// Sets the scope (Cluster or Namespace)
        /// </summary>
        ICustomResourceDefinitionBuilder SetScope(Scope scope);
        ICustomResourceDefinitionVersionBuilder<T> AddVersion<T>();
    }
}
