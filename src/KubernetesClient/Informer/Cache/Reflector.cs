using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using k8s.Models;
using k8s.Utils;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace k8s.Informer.Cache
{
  public class Reflector<TApi, TApiList> : IHostedService where TApi : class
  {
    private readonly ILogger _log;
    public string LastSyncResourceVersion { get; private set; }
    private IDisposable _watch;

    private readonly IListerWatcher<TApi, TApiList> _listerWatcher;

    private readonly IStore<LinkedList<DeltaFifo<TApi>.ObjectDelta>,TApi> _store;

    public Task Running { get; private set; }
    private CancellationTokenSource _cancellationTokenSource;

    public Reflector(IListerWatcher<TApi, TApiList> listerWatcher, IStore<LinkedList<DeltaFifo<TApi>.ObjectDelta>,TApi> store, ILogger<Reflector<TApi, TApiList>> log = null)
    {
      _listerWatcher = listerWatcher;
      _log = log;
      _store = store;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      Running = Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
      return Task.CompletedTask;
    }

    
    private async Task Run()
    {
      try
      {
        _log?.LogInformation($"{typeof(TApi)}#Start listing and watching...");

        //todo: see if underlying kubernetes shape objects can be exposed via interfaces
        dynamic list = _listerWatcher.List(new CallGeneratorParams(false, null, TimeSpan.Zero));

        V1ListMeta listMeta = list.Metadata;
        var resourceVersion = listMeta.ResourceVersion;
        IList<TApi> items = list.Items;
        _log?.LogDebug($"{typeof(TApi)}#Extract resourceVersion {resourceVersion} list meta");

        SyncWith(items.ToList(), resourceVersion);
        LastSyncResourceVersion = resourceVersion;

        _log?.LogDebug($"{typeof(TApi)}#Start watching with {LastSyncResourceVersion}...");


        while (true)
        {
          if (_cancellationTokenSource.IsCancellationRequested)
          {
            _watch?.Dispose();
            return;
          }

          try
          {
            _log?.LogDebug($"{typeof(TApi)}#Start watch with resource version {LastSyncResourceVersion}");

            _watch =
              _listerWatcher.Watch(
                new CallGeneratorParams(
                  true,
                  LastSyncResourceVersion, TimeSpan.FromMinutes(5)))
                .Subscribe(new Observer<Tuple<WatchEventType, TApi>>(x => this.WatchHandler(x.Item1, x.Item2)));
          }
          catch (Exception e)
          {
            _log?.LogInformation($"{typeof(TApi)}#Watch connection get exception {e.Message}");
            var underlyingException = e.InnerException;
            // If this is "connection refused" error, it means that most likely apiserver is not
            // responsive.
            // It doesn't make sense to re-list all objects because most likely we will be able to
            // restart
            // watch where we ended.
            // If that's the case wait and resend watch request.
            //todo: figure out the .NET equivalent of connectexception
            if (underlyingException != null && (underlyingException is SocketException))
            {
              _log?.LogInformation($"{typeof(TApi)}#Watch get connect exception, retry watch");
              await Task.Delay(1000);
              continue;
            }

            // if ((e instanceof RuntimeException)
            //   && t.getMessage().contains("IO Exception during hasNext")) {
            //   log.info("{}#Read timeout retry list and watch", apiTypeClass);
            //   return;
            // }
            _log?.LogError($"{typeof(TApi)}#Watch failed as {e.Message} unexpected");
            return;
          }
          finally
          {
            _watch?.Dispose();
            _watch = null;
          }
        }
      }
      catch (Exception e)
      {
        _log?.LogError($"{typeof(TApi)}#Failed to list-watch: {e}");
      }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      _cancellationTokenSource.Cancel();
      _watch.Dispose();
      await Running;
    }
    private void SyncWith(List<TApi> items, String resourceVersion) 
    {
      _store.Replace(items, resourceVersion);
    }

    private void WatchHandler(WatchEventType eventType, TApi item)
    {
      dynamic obj = item;      
      if (eventType == WatchEventType.Error)
      {
        var errorMessage = $"got ERROR event and its status: {obj.Status}";
        _log?.LogError(errorMessage);
        throw new Exception(errorMessage);
      }



      V1ObjectMeta meta;
      try
      {
        meta = obj.Metadata;
      }
      catch (RuntimeBinderException)
      {
        _log?.LogError($"malformed watch event {item}");
        return;
      }

      var newResourceVersion = meta.ResourceVersion;
      switch (eventType)
      {
        case WatchEventType.Added:
          _store.Add(item);
          break;
        case WatchEventType.Modified:
          _store.Update(item);
          break;
        case WatchEventType.Deleted:
          _store.Delete(item);
          break;
      }

      LastSyncResourceVersion = newResourceVersion;
      _log?.LogDebug($"{typeof(TApi)}#Receiving resourceVersion {LastSyncResourceVersion}");
    }
  }
}