using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using k8s.Informer.Impl;
using k8s.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Rest;

namespace k8s.Informer
{
    public class SharedInformerFactory : IHostedService
    {
        private ConcurrentDictionary<Type, IHostedService> _informers = new ConcurrentDictionary<Type, IHostedService>();


        private Kubernetes _apiClient;

        public SharedInformerFactory(Kubernetes apiClient)
        {
            _apiClient = apiClient;
        }

        public ISharedIndexInformer<TApi> SharedIndexInformerFor<TApi, TApiList>(
            Func<IKubernetes, CallGeneratorParams, Task<HttpOperationResponse<TApiList>>> callGenerator) where TApi : class
        {
            return SharedIndexInformerFor<TApi, TApiList>(callGenerator, TimeSpan.Zero);
        }

        public ISharedIndexInformer<TApi> SharedIndexInformerFor<TApi, TApiList>(
            Func<IKubernetes, CallGeneratorParams, Task<HttpOperationResponse<TApiList>>> callGenerator,
            TimeSpan resyncPeriod) where TApi : class
        {
            
            var listerWatcher = ListerWatcherFor<TApi, TApiList>(callGenerator);
            return SharedIndexInformerFor(listerWatcher, resyncPeriod);
        }
        public ISharedIndexInformer<TApi> SharedIndexInformerFor<TApi, TApiList>(
            IListerWatcher<TApi, TApiList> listerWatcher,
            TimeSpan resyncPeriod) where TApi : class
        {
            var informer = new DefaultSharedIndexInformer<TApi, TApiList>(listerWatcher, resyncPeriod);
            _informers.TryAdd(typeof(TApi), informer);
            return informer;
        }
        
        private  IListerWatcher<TApi, TApiList> ListerWatcherFor<TApi, TApiList>(
            Func<IKubernetes, CallGeneratorParams, Task<HttpOperationResponse<TApiList>>> callGenerator)
        {
            if (_apiClient.HttpClient.Timeout < Timeout.InfiniteTimeSpan) 
            {
                _apiClient.HttpClient.Timeout = Timeout.InfiniteTimeSpan;
            }
            return new DefaultListWatcher<TApi, TApiList>(_apiClient, callGenerator);
        }
        public ISharedIndexInformer<ApiType> GetExistingSharedIndexInformer<ApiType>() 
        {
            return (ISharedIndexInformer<ApiType>)_informers.GetOrDefault(typeof(ApiType));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_informers.Select(x => x.Value.StartAsync(cancellationToken)));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_informers.Select(x => x.Value.StopAsync(cancellationToken)));
        }
    }
}