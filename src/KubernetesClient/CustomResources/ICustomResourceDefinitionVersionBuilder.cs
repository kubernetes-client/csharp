using System;
using System.Linq.Expressions;
using k8s.Models;

namespace k8s.CustomResources
{
    public interface ICustomResourceDefinitionVersionBuilder<T> : ICustomResourceDefinitionBuilderFinal
    {
        ICustomResourceDefinitionVersionBuilder<T> EnableScaleSubresource<TReplicaSpec, TReplicaStatus>(
            Expression<Func<T, TReplicaSpec>> replicaSpecField,
            Expression<Func<T, TReplicaStatus>> replicaStatusField);
        ICustomResourceDefinitionVersionBuilder<T> IsServe(bool val = true);
        ICustomResourceDefinitionVersionBuilder<T> IsStore(bool val = true);
        ICustomResourceDefinitionVersionBuilder<TOther> AddVersion<TOther>();
    }
}
