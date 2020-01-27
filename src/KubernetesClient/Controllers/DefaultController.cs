using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using k8s.Controllers.Reconciler;
using k8s.WorkQueue;
using Microsoft.Extensions.Logging;

namespace k8s.Controllers
{
    public class DefaultController : IController
    {
        private readonly IRateLimitingChannel<Request> _workQueue;
        private readonly Task[] _readyFuncs;
        private readonly List<Task> _workers = new List<Task>();
        private readonly ILogger<DefaultController> _log;
        public TimeSpan ReadyTimeout { get; set; }
        // public TimeSpan ReadyCheckInternal { get; set; }
        public string Name { get; set; }
        public int WorkerCount { get; set; }
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public IReconciler Reconciler { get; set; }

        public DefaultController(
            IReconciler reconciler,
            IRateLimitingChannel<Request> workQueue,
            Task[] readyFuncs,
            ILogger<DefaultController> log) 
        {
            Reconciler = reconciler;
            _workQueue = workQueue;
            _readyFuncs = readyFuncs;
            _log = log;
            ReadyTimeout = TimeSpan.FromSeconds(30);
            // ReadyCheckInternal = TimeSpan.FromSeconds(1);
        }
        private async Task<bool> PreFlightCheck()
        {
            
            if (WorkerCount <= 0) {
                _log.LogError($"Fail to start controller {Name}: worker count must be positive.");
                return false;
            }
            var timeout = Task.Delay(ReadyTimeout);
            var isReady = false;
            try
            {
                isReady = (await Task.WhenAny(Task.WhenAll(_readyFuncs), timeout)) != timeout;
            }
            catch (Exception)
            {
                isReady = false;
            }
            if (!isReady) 
            {
                _log.LogError($"Fail to start controller {Name}: Timed out waiting for cache to be synced.");
                return false;
            }
            return true;
        }

        private async Task RunWorker()
        {
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {

                    try
                    {
                        var request = await _workQueue.Reader.ReadAsync(_cancellationTokenSource.Token);
                        var result = await Reconciler.Reconcile(request);
                        if (result.ShouldRequeue)
                        {
                            if (result.RequeueAfter == TimeSpan.Zero)
                            {
                                await _workQueue.Writer.WriteAsync(request, _cancellationTokenSource.Token);
                            }
                            else
                            {
#pragma warning disable 4014
                                Task.Run(async () =>
#pragma warning restore 4014
                                {
                                    try
                                    {
                                        await Task.Delay(result.RequeueAfter, _cancellationTokenSource.Token);
                                        await _workQueue.Writer.WriteAsync(request, _cancellationTokenSource.Token);
                                    }
                                    catch (OperationCanceledException)
                                    {
                                    }
                                    catch (Exception)
                                    {
                                        _log.LogError($"Error requing workitem {request.Name}");
                                    }
                                });
                            }
                        }
                    }
                    catch (ChannelClosedException)
                    {
                        return;
                    }

                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception e)
            {
                _log.LogError("Error in the worker processor", e);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!await PreFlightCheck()) 
            {
                _log.LogError($"Controller {Name} failed pre-run check, exiting..");
                return;
            }

            for (int i = 0; i < WorkerCount; i++)
            {
                _workers.Add(RunWorker());
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            _workQueue.Writer.Complete();
            await Task.WhenAll(_workers);
        }
    }
}