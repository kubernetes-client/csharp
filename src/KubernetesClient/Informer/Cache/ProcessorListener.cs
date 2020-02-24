using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using k8s.Informer.Exceptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace k8s.Informer.Cache
{
    public class ProcessorListener<TApiType> : IHostedService
    {
        private readonly ILogger<Logger<ProcessorListener<TApiType>>> _log;
        private TimeSpan _resyncPeriod;
        private DateTime _nextResync;
        private Task _run;

        private Channel<Notification> _queue = Channel.CreateUnbounded<Notification>();

        private IResourceEventHandler<TApiType> _handler;
        private CancellationTokenSource _cancellation;

        public ProcessorListener(IResourceEventHandler<TApiType> handler, TimeSpan resyncPeriod, ILogger<Logger<ProcessorListener<TApiType>>> log = null)
        {
            _handler = handler;
            _resyncPeriod = resyncPeriod;
            _log = log;
            DetermineNextResync(DateTime.UtcNow);

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _run = Run();
            return Task.CompletedTask;

        }

        private async Task Run()
        {
            while (true)
            {
                var obj = await _queue.Reader.ReadAsync(_cancellation.Token);
                if (obj is UpdateNotification updatedObj)
                {
                    try
                    {
                        await _handler.OnUpdate(updatedObj.OldObj, updatedObj.NewObj);
                    }
                    catch (Exception e)
                    {
                        // Catch all exceptions here so that listeners won't quit unexpectedly
                        _log.LogError($"failed invoking UPDATE event handler: {e.Message}");
                        continue;
                    }
                }
                else if (obj is AddNotification addedObj)
                {
                    try
                    {
                        await _handler.OnAdd(addedObj.NewObj);
                    }
                    catch (Exception e)
                    {
                        // Catch all exceptions here so that listeners won't quit unexpectedly
                        _log.LogError($"failed invoking ADD event handler: {e.Message}");
                        continue;
                    }
                }
                else if (obj is DeleteNotification deleted)
                {
                    var deletedObj = deleted.OldObj;
                    try
                    {
                        await _handler.OnDelete(deletedObj, deleted.IsFinalStatusUnknown);
                    }
                    catch (Exception e)
                    {
                        // Catch all exceptions here so that listeners won't quit unexpectedly
                        _log.LogError($"failed invoking DELETE event handler: {e.Message}");
                        continue;
                    }
                }
                else
                {
                    // todo: this is running on unmonitored thread, so this will probably crash the app
                    throw new BadNotificationException("unrecognized notification");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellation.Cancel();
            return Task.CompletedTask;
        }

        public virtual async Task Add(Notification obj) 
        {
            if (obj == null) {
                return;
            }
            await _queue.Writer.WriteAsync(obj);
        }

        public void DetermineNextResync(DateTime now) 
        {
            _nextResync = now.Add(_resyncPeriod);
        }

        public bool ShouldResync(DateTime now) 
        {
            return _resyncPeriod != TimeSpan.Zero && now >= _nextResync;
        }
        public class Notification
        {
        }

        public class UpdateNotification : Notification
        {
            public TApiType OldObj { get; }
            public TApiType NewObj { get; }

            public UpdateNotification(TApiType oldObj, TApiType newObj)
            {
                OldObj = oldObj;
                NewObj = newObj;
            }
        }

        public class AddNotification : Notification
        {
            public TApiType NewObj { get; }

            public AddNotification(TApiType newObj)
            {
                NewObj = newObj;
            }
        }

        public class DeleteNotification : Notification
        {
            public bool IsFinalStatusUnknown { get; }
            public TApiType OldObj { get; }

            public DeleteNotification(TApiType oldObj, bool isFinalStatusUnknown = false)
            {
                OldObj = oldObj;
                IsFinalStatusUnknown = isFinalStatusUnknown;
            }

        }
    }
}