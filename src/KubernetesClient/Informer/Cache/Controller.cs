using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace k8s.Informer.Cache
{
    public class Controller<TApiType, TApiListType> : IHostedService where TApiType : class
    {
        private static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(1);
        private TimeSpan _fullResyncPeriod;
        private readonly ILogger<Controller<TApiType, TApiListType>> _log;
        private DeltaFifo<TApiType> _queue;

        private IListerWatcher<TApiType, TApiListType> _listerWatcher;

        private Reflector<TApiType, TApiListType> _reflector;

        private Func<bool> _resyncFunc;

        /** how we actually process items from the queue */
        private Func<LinkedList<DeltaFifo<TApiType>.ObjectDelta>, Task> _processFunc;

        private Task _resyncTask;
        private Task _reflectorTask;
        private Task _queueTask;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public Controller(
            DeltaFifo<TApiType> queue,
            IListerWatcher<TApiType, TApiListType> listerWatcher,
            Func<LinkedList<DeltaFifo<TApiType>.ObjectDelta>, Task> processFunc,
            ILogger<Controller<TApiType, TApiListType>> log = null) : this(queue, listerWatcher, processFunc, null, TimeSpan.Zero, log)
        {
            
        }

    public Controller(
            DeltaFifo<TApiType> queue,
            IListerWatcher<TApiType, TApiListType> listerWatcher,
            Func<LinkedList<DeltaFifo<TApiType>.ObjectDelta>, Task> processFunc,
            Func<bool> resyncFunc,
            TimeSpan fullResyncPeriod,
            ILogger<Controller<TApiType,TApiListType>> _log = null) {
            _queue = queue;
            _listerWatcher = listerWatcher;
            _processFunc = processFunc;
            _resyncFunc = resyncFunc;
            _fullResyncPeriod = fullResyncPeriod;
            this._log = _log;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
            _log?.LogInformation("informer#Controller: ready to run resync & reflector runnable");

            // start the resync runnable
            if (_fullResyncPeriod > TimeSpan.Zero)
            {
                _resyncTask = RunResyncLoop();
            } 
            else 
            {
                _log.LogInformation("informer#Controller: resync skipped due to 0 full resync period");
            }

            _reflector = new Reflector<TApiType, TApiListType>(_listerWatcher, _queue);
            _reflectorTask = RunReflectorLoop();
            _queueTask = RunQueueWorkerLoop();
            return Task.CompletedTask;
        }

        private async Task RunResyncLoop()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await Task.Delay(_fullResyncPeriod);
                bool shouldResync = _resyncFunc == null || _resyncFunc(); 
                if(shouldResync)
                    _queue.Resync();
            }
        }
        private async Task RunReflectorLoop()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await _reflector.StartAsync(_cancellationTokenSource.Token);
                //todo: handle reflector crash
                //await _reflector.Running; // reflector may crash, so we'll just restart it
                await Task.Delay(DefaultPeriod);
            }
        }
        
        private async Task RunQueueWorkerLoop()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var item = await _queue.ReadAsync();
                await _processFunc(item);
            }
        }

        public Task HasSynced => _queue.HasSynced;
        public string LastSyncResourceVersion => _reflector == null ? string.Empty : _reflector.LastSyncResourceVersion;

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            await Task.WhenAll(_queueTask, _reflectorTask, _resyncTask);
        }
    }
    
}