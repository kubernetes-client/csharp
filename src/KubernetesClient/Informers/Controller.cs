using k8s.cache;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace k8s.informers
{
    public class Controller<T, L> : IController<T, L>
    {
        public IReflector<T,L> _reflector;       
        public IStore _store;
        private Queue<Tuple<WatchEventType, IKubernetesObject>> _fifo;
        private AutoResetEvent _fifoEvent;
        private Action<WatchEventType, IKubernetesObject> _onAdd;
        private Action<WatchEventType, IKubernetesObject> _onDelete;
        private Action<WatchEventType, IKubernetesObject, IKubernetesObject> _onUpdate;

        private void onAdd(WatchEventType eventType, IKubernetesObject obj) {
            if (_onAdd != null) {
                _onAdd(eventType, obj);
            }
        }

        private void onDelete(WatchEventType eventType, IKubernetesObject deletedObj) {
            if (_onDelete != null) {
                _onDelete(eventType, deletedObj);
            }
        }

        private void onUpdate(WatchEventType eventType, IKubernetesObject oldObj, IKubernetesObject newObj) {
            if (_onUpdate != null) {
                _onUpdate(eventType, oldObj, newObj);
            }
        }

        public Controller(ListerWatcher<T,L> lw, IStore s) 
        {            
            _store = s;
            _fifo = new Queue<Tuple<WatchEventType, IKubernetesObject>>();
            _fifoEvent = new AutoResetEvent(false);
            _reflector = new Reflector<T, L>(lw, TimeSpan.FromMinutes(1), _fifo, _fifoEvent);
            
        }

        /// <summary>
        /// Installs resource handler call backs for add, del and update of objects.
        /// </summary>
        public void AddResourceHandlers(Action<WatchEventType, IKubernetesObject> onAdd, Action<WatchEventType, IKubernetesObject> onDelete, 
                                        Action<WatchEventType, IKubernetesObject, IKubernetesObject> onUpdate) {
            // TODO: Add locking to ensure that we insert the callback after the current events are processed.
            _onAdd = onAdd;
            _onDelete = onDelete;
            _onUpdate = onUpdate;
        }
       
        public bool HasSynced()
        {
            throw new NotImplementedException();
        }

        public string LastSyncResourceVersion()
        {
            throw new NotImplementedException();
        }

        private async Task controllerWorker(CancellationToken cancellationToken) 
        {   
            try {                
                // We wait for either Q event or the cancellation event to happen. If the cancellation
                // occures, WaitAny would return 1 as index and we will quit the while loop. If the return 
                // value is 0, ie. the Q event occured we perform another loop iteration.
                // We also do a reconcilation every minute in case some events were missed.
                var waitHandles = new WaitHandle[]{_fifoEvent, cancellationToken.WaitHandle };
                var reconcileInterval = TimeSpan.FromMinutes(1);                
                while (WaitHandle.WaitAny(waitHandles, reconcileInterval) != 1) {                    
                    // If we get one event, we scan the Q until its empty.                   
                    while (_fifo.Count != 0 && !cancellationToken.IsCancellationRequested) {                                                  
                        var element = _fifo.Dequeue();                        
                        switch (element.Item1) {
                            case WatchEventType.Added:                               
                                _store.Add(element.Item2);
                                onAdd(element.Item1, element.Item2);
                                break;
                            case WatchEventType.Deleted:
                                _store.Delete(element.Item2);
                                onDelete(element.Item1, element.Item2);
                                break;
                            case WatchEventType.Modified:
                                var oldVal = _store.Update(element.Item2);
                                onUpdate(element.Item1, oldVal, element.Item2);
                                break;
                            case WatchEventType.Error:
                                Console.WriteLine("Error: Got watch event error"); 
                                break;
                        }
                        // Ensure that we don't burn the CPU while trying to process the Q contents.
                       await Task.Yield();
                    }                               
                }
                if (cancellationToken.IsCancellationRequested) {
                    Console.WriteLine("Controller exiting due to cancel!!");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Runs starts the controller. The controller starts up the reflector which does the list and watch and start accumulating the objects.
        /// </summary> 
        public void Run(CancellationToken cancellationToken)
        {  
           _reflector.Run(cancellationToken);
            // Start the Controller thread.
            var t = new Task(async () => {await controllerWorker(cancellationToken);});
            t.Start();  
        }
    }
}

