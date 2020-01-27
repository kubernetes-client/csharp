using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using k8s.Utils;
using Microsoft.Rest;

namespace k8s.Informer
{
    public interface IListerWatcher<TApi, TApiList> 
    {
        Task<HttpOperationResponse<TApiList>> List(CallGeneratorParams param);

        IObservable<Tuple<WatchEventType,TApi>> Watch(CallGeneratorParams param);
    }

}