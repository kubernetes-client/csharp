using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace k8s.Informer
{
    public interface ISharedInformer<TApiType> : IHostedService 
    {

        Task AddEventHandler(IResourceEventHandler<TApiType> handler);

        Task AddEventHandlerWithResyncPeriod(IResourceEventHandler<TApiType> handler, TimeSpan resyncPeriod);

        Task HasSynced { get; }

        string LastSyncResourceVersion { get; }
    }
}