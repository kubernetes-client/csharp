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

    private readonly IWriteStore<TApi> _store;

    private CancellationTokenSource _cancellationTokenSource;

    public Reflector(IListerWatcher<TApi, TApiList> listerWatcher, IWriteStore<TApi> store, ILogger<Reflector<TApi, TApiList>> log = null)
    {
      _listerWatcher = listerWatcher;
      _log = log;
      _store = store;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      
      try
      {
        await RunWatch();
      }
      catch (Exception e)
      {
        _log?.LogError($"{typeof(TApi)}#Failed to list-watch: {e}");
      }
    }

    private async Task RunWatch()
    {
      _log?.LogInformation($"{typeof(TApi)}#Start listing and watching...");

      //todo: see if underlying kubernetes shape objects can be exposed via interfaces
      var response = await _listerWatcher.List(new CallGeneratorParams(false, null, TimeSpan.Zero));
      dynamic listKubernetesObject = response.Body; 
      V1ListMeta listMeta = listKubernetesObject.Metadata;
      var resourceVersion = listMeta.ResourceVersion;
      IList<TApi> items = listKubernetesObject.Items ?? new List<TApi>();
      _log?.LogDebug($"{typeof(TApi)}#Extract resourceVersion {resourceVersion} list meta");

        
      SyncWith(items.ToList(), resourceVersion);
      LastSyncResourceVersion = resourceVersion;

      _log?.LogDebug($"{typeof(TApi)}#Start watching with {LastSyncResourceVersion}...");
        
      // todo: review logic with java implementation as there's a retry logic for failed connection state, seems like entire watcher object will become unsable in .net verison as the stream would close
      _watch =
        _listerWatcher.Watch(
            new CallGeneratorParams(
              true,
              LastSyncResourceVersion, TimeSpan.FromMinutes(5)))
          .Subscribe(new Observer<Tuple<WatchEventType, TApi>>(
            x => WatchHandler(x.Item1, x.Item2), 
            exception => _cancellationTokenSource.Cancel(),
            () => _cancellationTokenSource.Cancel()));
      
      _cancellationTokenSource.Token.Register(() => _watch.Dispose());
    }

    private async Task RestartWatch()
    {
      _watch?.Dispose();
      await Task.Delay(1000);
      await RunWatch();
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
      _cancellationTokenSource.Cancel();
      _watch.Dispose();
      return Task.CompletedTask;
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
        meta = obj.Metadata; // todo: dynamic hack is ugly, propose slapping k8s objects with something like IHasMetadata interface
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