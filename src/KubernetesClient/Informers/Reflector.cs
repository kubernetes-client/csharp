using System;
using System.Threading;
using k8s.cache;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace k8s.informers
{
    public class Reflector<T, L>: IReflector<T,L> {
        private ListerWatcher<T,L> _listerWatcher;
        private TimeSpan _resyncInterval;
        private Queue<Tuple<WatchEventType, IKubernetesObject>> _fifo;
        private AutoResetEvent _fifoEvent;
        
        public Reflector(ListerWatcher<T,L> l, TimeSpan resyncInterval, Queue<Tuple<WatchEventType, IKubernetesObject>> fifo, AutoResetEvent fifoEvent) {
            _listerWatcher = l;
            _resyncInterval = resyncInterval;
            _fifo = fifo;
            _fifoEvent = fifoEvent;
        }

         /// <summary>
        /// From the body of http response we get the list objects. List objects are stored "Items" field. Extract this
        /// and return back as list of objects. 
        /// TODO: Find a more graceful way to perform this without using reflection.
        /// </summary>  
        private IList<T> getItemsAsList(Microsoft.Rest.HttpOperationResponse<L> response) {
            return (IList<T>)response.Body.GetType().GetProperty("Items").GetValue(response.Body, null);
        }

        private void reflectorWorker(CancellationToken c) 
        {       
            try {            
                var response = _listerWatcher.Lister().Result;
                var list = getItemsAsList(response);
                for (var i=0; i<list.Count; i++) {
                    // Cast the objects from the Items to IKubernetes for adding to store. 
                    var o = (IKubernetesObject)list[i];                    
                    _fifo.Enqueue(new Tuple<WatchEventType, IKubernetesObject>(WatchEventType.Added, o));                    
                    _fifoEvent.Set();
                }
                using (_listerWatcher.Watcher(
                    (type, item) =>
                        {
                            try {
                                var o = (IKubernetesObject)item;                                                                                                                                                     
                                _fifo.Enqueue(new Tuple<WatchEventType, IKubernetesObject>(type, o));                               
                                _fifoEvent.Set();
                            } catch (Exception ex) {                                    
                                throw ex;
                            }
                        })){                          
                            while(!c.IsCancellationRequested) {                                
                                Thread.Sleep(0);
                            }
                            if (c.IsCancellationRequested) {
                                Console.WriteLine("Cancellation requested. Going out of reflector work loop.");
                            }
                }                
            
            } catch (Exception ex) {
                    Console.WriteLine(ex);
                    throw ex;
            }
        }

        public void Run(CancellationToken c)
        {
            var t = new Task(() => {reflectorWorker(c);});
            t.Start();            
        }
    }
}

