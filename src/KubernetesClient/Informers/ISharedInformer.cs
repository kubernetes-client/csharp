using System;
using System.Threading;
using k8s.cache;

namespace k8s.informers
{
    public interface ISharedInformer<T, L> {
        /// <summary>
        /// Installs resource handler call backs for add, del and update of objects.
        /// </summary>
        void AddResourceHandlers(Action<WatchEventType, IKubernetesObject> onAdd = null, 
                                 Action<WatchEventType, IKubernetesObject> onDelete = null, 
                                 Action<WatchEventType, IKubernetesObject, IKubernetesObject> onUpdate = null);                                 

        /// <summary>
        /// Gets the controller object which runs as part of the shared informer.
        /// </summary> 
        IController<T, L> GetController();

        /// <summary>
        /// Gets the store object which is associated with the shared informer. Can be used to list the objects accumulated in the store.
        /// </summary> 
        IStore GetStore();

        String LastSyncedVersion();      

        /// <summary>
        /// Runs starts the controller. The controller starts up the reflector which does the list and watch and start accumulating the objects.
        /// </summary> 
        void Run(CancellationToken cancellationToken);        
    }
}

