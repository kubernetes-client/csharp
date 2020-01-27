using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace k8s.Informer.Cache
{
    public class SharedProcessor<TApiType> : IHostedService
    {
        private ReaderWriterLock _lock = new ReaderWriterLock();
        private List<ProcessorListener<TApiType>> _listeners = new List<ProcessorListener<TApiType>>();
        private List<ProcessorListener<TApiType>> _syncingListeners = new List<ProcessorListener<TApiType>>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public async Task AddAndStartListener(ProcessorListener<TApiType> processorListener) 
        {
            _lock.AcquireWriterLock(TimeSpan.MaxValue);
            try 
            {
                AddListenerLocked(processorListener);
                await processorListener.StartAsync(_cancellationTokenSource.Token);
            } 
            finally 
            {
                _lock.ReleaseWriterLock();
            }
        }
        public Task AddListener(ProcessorListener<TApiType> processorListener) 
        {
            _lock.AcquireWriterLock(TimeSpan.MaxValue);
            try 
            {
                AddListenerLocked(processorListener);
            } 
            finally 
            {
                _lock.ReleaseWriterLock();
            }
            return Task.CompletedTask;
        }
        private void AddListenerLocked(ProcessorListener<TApiType> processorListener) 
        {
            _listeners.Add(processorListener);
            _syncingListeners.Add(processorListener);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _lock.AcquireReaderLock(TimeSpan.MaxValue);
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
            try
            {
                await Task.WhenAll(_listeners.Select(x => x.StartAsync(_cancellationTokenSource.Token)));
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }
        }
        
        public async Task Distribute(ProcessorListener<TApiType>.Notification obj, bool isSync)
        {
            // todo: confirm if locks work fine with async/await. should work cuz context capture should drop us on the original thread
            _lock.AcquireReaderLock(TimeSpan.MaxValue);
            try 
            {
                if (isSync) 
                {
                    foreach (var listener in _syncingListeners) 
                    {
                        await listener.Add(obj);
                    }
                } 
                else 
                {
                    foreach (var listener in _listeners) 
                    {
                        await listener.Add(obj);
                    }
                }
            } 
            finally 
            {
                _lock.ReleaseReaderLock();
            }
        }
        
        public bool ShouldResync() 
        {
            _lock.AcquireWriterLock(TimeSpan.MaxValue);
            var resyncNeeded = false;
            try 
            {
                _syncingListeners = new List<ProcessorListener<TApiType>>();

                DateTime now = DateTime.UtcNow;
                foreach (var listener in _listeners) 
                {
                    if (listener.ShouldResync(now)) 
                    {
                        resyncNeeded = true;
                        _syncingListeners.Add(listener);
                        listener.DetermineNextResync(now);
                    }
                }
            } 
            finally 
            {
                _lock.ReleaseWriterLock();
            }
            return resyncNeeded;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _lock.AcquireWriterLock(TimeSpan.MaxValue);
            try
            {
                await Task.WhenAll(_listeners.Select(x => x.StopAsync(cancellationToken)));
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }
    }
}