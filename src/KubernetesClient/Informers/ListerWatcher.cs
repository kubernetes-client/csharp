using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using k8s;
using System;

namespace k8s.informers
{    
    public delegate Task<HttpOperationResponse<L>> ListerD<T, L>();
    public delegate Watcher<T> WatcherD<T, L>(Action<WatchEventType, T> onEvent, Action<Exception> onError = null, Action onClosed = null);

    public class ListerWatcher<T, L>
    {           
        public ListerD<T, L> Lister;
        public WatcherD<T,L> Watcher;
       
        ListerD<T,L> GetLister() {
            return Lister;
        }
        string _nameSpace;
        
        CancellationToken _cancellationToken;   
        public ListerWatcher(IKubernetes client) {
            var nameSpace = "";
            var cancellationToken = new CancellationToken();
            init(client, nameSpace, cancellationToken);
        }

        public ListerWatcher(IKubernetes client, string kubeNamespace, CancellationToken cancellationToken) {
           init(client, kubeNamespace, cancellationToken);
        }
        
        private void init(IKubernetes client, string kubeNamespace, CancellationToken cancellationToken) {
            _nameSpace = kubeNamespace.ToLower();
            _cancellationToken = cancellationToken;
            Lister = delegate() {                                           
                        return client.List<T, L>(_nameSpace, _cancellationToken, false);
                    };
            Watcher = delegate(Action<WatchEventType, T> onEvent, Action<Exception> onError, Action onClosed)
                    {

                        var l = client.List<T, L>(_nameSpace, _cancellationToken, watch:true);
                        Console.WriteLine("About to watch");
                        return l.Watch(onEvent, onError, onClosed);                         
                    };
        }

    }        
}
