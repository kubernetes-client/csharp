using System;
using System.Threading;
using k8s.cache;

namespace k8s.informers
{
    public class SharedInformer<T, L> : ISharedInformer<T, L>
    {        
        IController<T, L> _controller;
        IStore _store;
        public SharedInformer(ListerWatcher<T,L> lw)
        {
            _store = new Store();
            _controller = new Controller<T, L>(lw, _store);

        }

        /// <summary>
        /// Installs resource handler call backs for add, del and update of objects.
        /// </summary>        
        public void AddResourceHandlers(Action<WatchEventType, IKubernetesObject> onAdd=null, Action<WatchEventType, IKubernetesObject> onDelete=null, Action<WatchEventType, IKubernetesObject, IKubernetesObject> onUpdate=null)
        {
            // TODO: Raise a user friendly exception.
            if (_controller != null) {
                _controller.AddResourceHandlers(onAdd, onDelete, onUpdate);
            }
        }

        /// <summary>
        /// Gets the controller object which runs as part of the shared informer.
        /// </summary>        
        public IController<T,L> GetController()
        {
            return _controller;
        }

        /// <summary>
        /// Gets the store object which is associated with the shared informer. Can be used to list the objects accumulated in the store.
        /// </summary>        
        public IStore GetStore()
        {
            return _store;
        }

        public string LastSyncedVersion()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs starts the controller. The controller starts up the reflector which does the list and watch and start accumulating the objects.
        /// </summary>    
        public void Run(CancellationToken cancellationToken)
        {
            _controller.Run(cancellationToken);            
        }
    }
}

