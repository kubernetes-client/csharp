using System;
using System.Threading;

namespace k8s.informers
{
    public interface IController<T, L>{
        /// <summary>
        /// Installs resource handler call backs for add, del and update of objects.
        /// </summary>
         void AddResourceHandlers(Action<WatchEventType, IKubernetesObject> onAdd, 
                                  Action<WatchEventType, IKubernetesObject> onDelete, 
                                  Action<WatchEventType, IKubernetesObject, IKubernetesObject> onUpdate);       
        bool HasSynced();
        String LastSyncResourceVersion();
        /// <summary>
        /// Runs starts the controller. The controller starts up the reflector which does the list and watch and start accumulating the objects.
        /// </summary> 
        void Run(CancellationToken cancellationToken);
    }
}

