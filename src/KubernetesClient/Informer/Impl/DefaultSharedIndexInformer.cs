using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using k8s.Informer.Cache;
using Microsoft.Extensions.Logging;

namespace k8s.Informer.Impl
{
    public class DefaultSharedIndexInformer<TApi, TApiList>: ISharedIndexInformer<TApi> where TApi : class
    {
        private readonly ILogger<DefaultSharedIndexInformer<TApi, TApiList>> _log;
        private static readonly TimeSpan MinimumResyncPeriod = TimeSpan.FromSeconds(1);
        private TimeSpan _resyncPeriod;
        private TimeSpan _defaultEventHandlerResyncPeriod;

        private IIndexer<TApi> _indexer;

        private SharedProcessor<TApi> _processor = new SharedProcessor<TApi>();

        private Controller<TApi, TApiList> _controller;
        private bool _isStarted;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public DefaultSharedIndexInformer(
            IListerWatcher<TApi, TApiList> listerWatcher, 
            TimeSpan resyncPeriod, 
            ILogger<DefaultSharedIndexInformer<TApi,TApiList>> log = null) :
            this(listerWatcher, resyncPeriod, new Cache<TApi>(), log)
        {
            
        }
        
        public DefaultSharedIndexInformer(
            IListerWatcher<TApi, TApiList> listerWatcher,
            TimeSpan resyncPeriod,
            Cache<TApi> cache,
            ILogger<DefaultSharedIndexInformer<TApi,TApiList>> log = null)
        : this(
            listerWatcher,
            resyncPeriod,
            new DeltaFifo<TApi>(cache.KeyFunc, cache),
            cache, 
            log)
        {
        }
        public DefaultSharedIndexInformer(
            IListerWatcher<TApi, TApiList> listerWatcher,
            TimeSpan resyncPeriod,
            DeltaFifo<TApi> deltaFifo,
            IIndexer<TApi> indexer,
            ILogger<DefaultSharedIndexInformer<TApi,TApiList>> log)
        {
            _resyncPeriod = resyncPeriod;
            _indexer = indexer;
            _defaultEventHandlerResyncPeriod = resyncPeriod;
            _log = log;
            _controller = new Controller<TApi,TApiList>(
                deltaFifo,
                listerWatcher,
                HandleDeltas,
                _processor.ShouldResync,
                _resyncPeriod);
        }
        //Func<LinkedList<MutablePair<DeltaFifo<TApiType>.DeltaType, object>>>

        public async Task AddEventHandler(IResourceEventHandler<TApi> handler)
        {
            await AddEventHandlerWithResyncPeriod(handler, _defaultEventHandlerResyncPeriod);
        }

        public async Task AddEventHandlerWithResyncPeriod(IResourceEventHandler<TApi> handler, TimeSpan resyncPeriod)
        {
            if (!_isStarted)
            {
                _log?.LogInformation("DefaultSharedIndexInformer#Handler was not added to shared informer because it has stopped already");
                return;
            }

            if (_resyncPeriod > TimeSpan.Zero)
            {
                if (resyncPeriod < MinimumResyncPeriod)
                {
                    _log?.LogWarning($"DefaultSharedIndexInformer#resyncPeriod {_resyncPeriod} is too small. Changing it to the minimum allowed rule of {MinimumResyncPeriod}");
                }

                if (resyncPeriod < _resyncPeriod)
                {
                    if (_isStarted)
                    {
                        _log?.LogWarning(
                            $"DefaultSharedIndexInformer#resyncPeriod {resyncPeriod} is smaller than resyncCheckPeriod {_resyncPeriod} and the informer has already started. Changing it to {_resyncPeriod}");
                        resyncPeriod = _resyncPeriod;
                    }
                    else
                    {
                        // if the event handler's resyncPeriod is smaller than the current resyncCheckPeriod,
                        // update resyncCheckPeriod to match resyncPeriod and adjust the resync periods of all
                        // the listeners accordingly
                        _resyncPeriod = resyncPeriod;
                    }
                }
            }
            var listener = new ProcessorListener<TApi>(handler, DetermineResyncPeriod(resyncPeriod, _resyncPeriod));
            if (!_isStarted) {
                await _processor.AddListener(listener);
                return;
            }

            await _processor.AddAndStartListener(listener);
            foreach (var item in _indexer) 
            {
                await listener.Add(new ProcessorListener<TApi>.AddNotification(item));
            }
        }

        public string LastSyncResourceVersion => !_isStarted ? String.Empty : _controller.LastSyncResourceVersion;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if(_isStarted)
                return;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
            _isStarted = true;
            await _processor.StartAsync(cancellationToken);
            await _controller.StartAsync(_cancellationTokenSource.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if(!_isStarted)
                return;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

            _isStarted = false;
            await _controller.StopAsync(_cancellationTokenSource.Token);
            _cancellationTokenSource.Cancel();
            await _processor.StopAsync(_cancellationTokenSource.Token);
        }

        public Task HasSynced => _controller.HasSynced;
        private async Task HandleDeltas(LinkedList<DeltaFifo<TApi>.ObjectDelta> deltas) 
        {
            if (deltas.Count == 0) 
            {
                return;
            }

            // from oldest to newest
            foreach (var delta in deltas) 
            {
                var deltaType = delta.DeltaType;
                switch (deltaType) 
                {
                    case DeltaType.Sync:
                    case DeltaType.Added:
                    case DeltaType.Updated:
                        var isSync = deltaType == DeltaType.Sync;
                        var oldObj = _indexer[delta.Object];
                        if (oldObj != null) 
                        {
                            _indexer.Update(delta.Object);
                            await _processor.Distribute(
                                new ProcessorListener<TApi>.UpdateNotification(oldObj, delta.Object), isSync);
                        } 
                        else 
                        {
                            _indexer.Add(delta.Object);
                            await _processor.Distribute(
                                new ProcessorListener<TApi>.AddNotification(delta.Object), isSync);
                        }
                        break;
                    case DeltaType.Deleted:
                        _indexer.Delete(delta.Object);
                        await _processor.Distribute(new ProcessorListener<TApi>.DeleteNotification(delta.Object, delta.IsFinalStateUnknown), false);
                        break;
                }
            }
        }
        public void AddIndexers(Dictionary<string, Func<TApi, List<string>>> indexers)
        {
            if (_isStarted) {
                throw new InvalidOperationException("cannot add indexers to a running informer");
            }
            _indexer.AddIndexers(indexers);
        }

        public IIndexer<TApi> Indexer { get; }
        
        private TimeSpan DetermineResyncPeriod(TimeSpan desired, TimeSpan check) 
        {
            if (desired == TimeSpan.Zero) 
            {
                return desired;
            }
            if (check == TimeSpan.Zero) 
            {
                return TimeSpan.Zero;
            }
            return desired < check ? check : desired;
        }
    }
}