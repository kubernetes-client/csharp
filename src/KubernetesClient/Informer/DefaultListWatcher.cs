using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using k8s.Utils;
using Microsoft.Rest;

namespace k8s.Informer
{
    public class DefaultListWatcher<TApi, TApiList> : IListerWatcher<TApi, TApiList>
    {
        private readonly IKubernetes _kubernetes;
        private readonly Func<IKubernetes, CallGeneratorParams, Task<HttpOperationResponse<TApiList>>> _callGenerator;

        public DefaultListWatcher(IKubernetes kubernetes, Func<IKubernetes, CallGeneratorParams, Task<HttpOperationResponse<TApiList>>> callGenerator)
        {
            _kubernetes = kubernetes;
            _callGenerator = callGenerator;
        }

        public async Task<HttpOperationResponse<TApiList>> List(CallGeneratorParams param)
        {
            return await _callGenerator(_kubernetes, param);
        }

        public IObservable<Tuple<WatchEventType,TApi>> Watch(CallGeneratorParams param)
        {
            return _callGenerator(_kubernetes, param).Watch<TApi,TApiList>(null).AsObservable();
        }
    }
}