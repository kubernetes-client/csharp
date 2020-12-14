using System;
using System.Threading;
using System.Threading.Tasks;
using k8s.Models;

namespace k8s.LeaderElection.ResourceLock
{
    public class LeaseLock : MetaObjectLock<V1Lease>
    {
        public LeaseLock(IKubernetes client, string @namespace, string name, string identity)
            : base(client, @namespace, name, identity)
        {
        }

        protected override Task<V1Lease> ReadMetaObjectAsync(IKubernetes client, string name, string namespaceParameter,
            CancellationToken cancellationToken)
        {
            return client.ReadNamespacedLeaseAsync(name, namespaceParameter, cancellationToken: cancellationToken);
        }

        protected override LeaderElectionRecord GetLeaderElectionRecord(V1Lease obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new LeaderElectionRecord()
            {
                AcquireTime = obj.Spec.AcquireTime,
                HolderIdentity = obj.Spec.HolderIdentity,
                LeaderTransitions = obj.Spec.LeaseTransitions ?? 0,
                LeaseDurationSeconds = obj.Spec.LeaseDurationSeconds ?? 15, // 15 = default value
                RenewTime = obj.Spec.RenewTime,
            };
        }

        protected override V1Lease SetLeaderElectionRecord(LeaderElectionRecord record, V1Lease metaObj)
        {
            if (record == null)
            {
                throw new NullReferenceException(nameof(record));
            }

            if (metaObj == null)
            {
                throw new NullReferenceException(nameof(metaObj));
            }

            metaObj.Spec = new V1LeaseSpec()
            {
                AcquireTime = record.AcquireTime,
                HolderIdentity = record.HolderIdentity,
                LeaseTransitions = record.LeaderTransitions,
                LeaseDurationSeconds = record.LeaseDurationSeconds,
                RenewTime = record.RenewTime,
            };

            return metaObj;
        }

        protected override Task<V1Lease> CreateMetaObjectAsync(IKubernetes client, V1Lease obj, string namespaceParameter,
            CancellationToken cancellationToken)
        {
            return client.CreateNamespacedLeaseAsync(obj, namespaceParameter, cancellationToken: cancellationToken);
        }

        protected override Task<V1Lease> ReplaceMetaObjectAsync(IKubernetes client, V1Lease obj, string name, string namespaceParameter,
            CancellationToken cancellationToken)
        {
            return client.ReplaceNamespacedLeaseAsync(obj, name, namespaceParameter, cancellationToken: cancellationToken);
        }
    }
}
