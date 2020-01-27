using System;
using System.Threading.Tasks;
using k8s.Controllers.Reconciler;
using k8s.Informer;
using k8s.WorkQueue;

namespace k8s.Controllers
{
    public class DefaultControllerWatch<TApi> : IControllerWatch<TApi>
    {
        private readonly IChannel<Request> _workQueue;
        private readonly Func<TApi, Request> _workKeyGenerator;

        public DefaultControllerWatch(IChannel<Request> workQueue, Func<TApi, Request> workKeyGenerator)
        {
            _workQueue = workQueue;
            _workKeyGenerator = workKeyGenerator;
        }

        public Func<TApi, bool> OnAddFilterPredicate { get; set; }
        public Func<TApi, TApi, bool> OnUpdateFilterPredicate { get; set; }
        public Func<TApi, bool, bool> OnDeleteFilterPredicate { get; set; }
        public IResourceEventHandler<TApi> ResourceEventHandler { get; }
        
        private class DefaultResourceEventHandler : IResourceEventHandler<TApi>
        {
            private readonly DefaultControllerWatch<TApi> _parent;

            public DefaultResourceEventHandler(DefaultControllerWatch<TApi> parent)
            {
                _parent = parent;
            }

            public async Task OnAdd(TApi obj)
            {
                if (_parent.OnAddFilterPredicate == null || _parent.OnAddFilterPredicate(obj)) {
                    await _parent._workQueue.Writer.WriteAsync(_parent._workKeyGenerator(obj));
                }
            }

            public async Task OnUpdate(TApi oldObj, TApi newObj)
            {
                if (_parent.OnAddFilterPredicate == null || _parent.OnUpdateFilterPredicate(oldObj, newObj)) {
                    await _parent._workQueue.Writer.WriteAsync(_parent._workKeyGenerator(newObj));
                }            }

            public async Task OnDelete(TApi obj, bool deletedFinalStateUnknown)
            {
                if (_parent.OnDeleteFilterPredicate == null
                    || _parent.OnDeleteFilterPredicate(obj, deletedFinalStateUnknown)) 
                {
                    await _parent._workQueue.Writer.WriteAsync(_parent._workKeyGenerator(obj));
                }            }
        }
    }
}