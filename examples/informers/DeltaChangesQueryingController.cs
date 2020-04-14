using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Informers;
using k8s.Informers.Notifications;
using k8s.Models;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.Logging;

namespace informers
{
    // this sample demos of informer in a basic controller context.
    // there are two loggers:
    //   _informerLogger lets you see raw data coming out of informer stream

    // try creating and deleting some pods in "default" namespace and watch the output
    // current code is not production grade and lacks concurrency guards against modifying same resource
    public class DeltaChangesQueryingController : IController
    {
        private readonly IKubernetesInformer<V1Pod> _podInformer;
        private readonly CompositeDisposable _subscription = new CompositeDisposable();
        private readonly ILogger _informerLogger;
        private readonly CompareLogic _objectCompare = new CompareLogic();

        public DeltaChangesQueryingController(IKubernetesInformer<V1Pod> podInformer, ILoggerFactory loggerFactory)
        {
            _podInformer = podInformer;
            _informerLogger = loggerFactory.CreateLogger("Informer");
            _objectCompare.Config.MaxDifferences = 100;
        }


        public Task Initialize(CancellationToken cancellationToken)
        {
            _podInformer
                .GetResource(ResourceStreamType.ListWatch, KubernetesInformerOptions.Builder.NamespaceEquals("default").Build())
                .Resync(TimeSpan.FromSeconds(10))
                .Catch<ResourceEvent<V1Pod>, Exception>(e =>
                {
                    _informerLogger.LogCritical(e, e.Message);
                    return Observable.Throw<ResourceEvent<V1Pod>>(e);
                })
                .Buffer(TimeSpan.FromSeconds(5))
                .Where(x => x.Any())
                .Do(x =>
                {
                    var eventsPerResource = x.GroupBy(x => x.Value.Metadata.Name);
                    foreach (var item in eventsPerResource)
                    {
                        PrintChanges(item.ToList());
                    }
                })
                .Subscribe()
                .DisposeWith(_subscription);
            return Task.CompletedTask;
        }

        private void PrintChanges(IList<ResourceEvent<V1Pod>> changes)
        {
            // it's possible to do reconciliation here, but the current code is not production grade and lacks concurrency guards against modifying same resource
            var obj = changes.First().Value;
            var sb = new StringBuilder();
            sb.AppendLine($"Received changes for object with ID {obj.Metadata.Name} with {changes.Count} items");
            sb.AppendLine($"Last known state was {changes.Last().EventFlags}");
            foreach (var item in changes)
            {
                sb.AppendLine($"==={item.EventFlags}===");
                sb.AppendLine($"Name: {item.Value.Metadata.Name}");
                sb.AppendLine($"Version: {item.Value.Metadata.ResourceVersion}");
                if (item.EventFlags.HasFlag(EventTypeFlags.Modify))
                {
                    var updateDelta = _objectCompare.Compare(item.OldValue, item.Value);
                    foreach (var difference in updateDelta.Differences)
                    {
                        sb.AppendLine($"{difference.PropertyName}: {difference.Object1} -> {difference.Object2}");
                    }
                }
            }

            _informerLogger.LogInformation(sb.ToString());

        }
    }
}
