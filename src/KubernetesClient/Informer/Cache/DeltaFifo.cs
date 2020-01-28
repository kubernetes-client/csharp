using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using k8s.Informer.Cache;
using Microsoft.Extensions.Logging;

namespace k8s.Informer
{
    public class DeltaFifo<TApi> : ChannelReader<LinkedList<DeltaFifo<TApi>.ObjectDelta>>, IStore<LinkedList<DeltaFifo<TApi>.ObjectDelta>, TApi> where TApi : class
    {
        private readonly Func<TApi, string> _keyFunc;
        private readonly IStore<TApi> _knownObjects;
        private readonly ILogger<DeltaFifo<TApi>> _log;
        private TaskCompletionSource<bool> _hasSyncedTcs = new TaskCompletionSource<bool>();

        private bool Populated
        {
            get => _populated;
            set
            {
                _populated = value;
                UpdateHasSynced();
            }
        }

        private int InitialPopulationCount
        {
            get => _initialPopulationCount;
            set
            {
                if(_initialPopulationCount <= 0 && _hasSyncedTcs.Task.IsCompleted && value >= 0)
                    _hasSyncedTcs = new TaskCompletionSource<bool>();
                _initialPopulationCount = value;
                UpdateHasSynced();
            }
        }

        public Task HasSynced => _hasSyncedTcs.Task;

        private void UpdateHasSynced()
        {
            if(!_hasSyncedTcs.Task.IsCompleted && _populated && _initialPopulationCount == 0)
                _hasSyncedTcs.SetResult(true);
        }

        private readonly ReaderWriterLock _lock = new ReaderWriterLock();

        // only tracks keys, actual reader will pull the value at read time
        readonly Channel<string> _readerToWriterQueue = Channel.CreateUnbounded<string>();
        internal Dictionary<string, LinkedList<ObjectDelta>> Items { get; } = new Dictionary<string, LinkedList<ObjectDelta>>();
        private int _initialPopulationCount;
        private bool _populated;

        public DeltaFifo(Func<TApi, string> keyFunc, IStore<TApi> knownObjects, ILogger<DeltaFifo<TApi>> log = null)
        {
            _keyFunc = keyFunc;
            _knownObjects = knownObjects;
            _log = log;
        }

        public override bool TryRead(out LinkedList<ObjectDelta> item)
        {
            while (_readerToWriterQueue.Reader.TryRead(out var id))
            {
                if (InitialPopulationCount > 0) {
                    InitialPopulationCount--;
                }
                if (!Items.ContainsKey(id)) {
                    // Item may have been deleted subsequently.
                    continue;
                }
                item = Items[id];
                Items.Remove(id);
                return true;
            }
            item = null;
            return false;
        }

        public override async ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return await _readerToWriterQueue.Reader.WaitToReadAsync(cancellationToken);
        }
        public void Add(TApi obj)
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                Populated = true;
                QueueActionLocked(AsDelta(DeltaType.Added, obj));
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        public void Update(TApi obj)
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                Populated = true;
                QueueActionLocked(AsDelta(DeltaType.Updated, obj));
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        public void Delete(TApi obj)
        {
            var id = KeyOf(obj);
            _lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                Populated = true;
                if (_knownObjects == null)
                {
                    if (Items.ContainsKey(id))
                    {
                        // Presumably, this was deleted when a relist happened.
                        // Don't provide a second report of the same deletion.
                        return;
                    }
                }
                else
                {
                    // We only want to skip the "deletion" action if the object doesn't
                    // exist in knownObjects and it doesn't have corresponding item in items.
                    if (_knownObjects[id] == null && !Items.ContainsKey(id))
                    {
                        return;
                    }
                }

                QueueActionLocked(AsDelta(DeltaType.Deleted, obj));
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        private ObjectDelta AsDelta(DeltaType deltaType, TApi obj, bool finalStateUnknown = false) =>
            new ObjectDelta
            {
                Object = obj,
                Key = KeyOf(obj),
                DeltaType = deltaType,
                IsFinalStateUnknown = finalStateUnknown
            };
        
        
        private void QueueActionLocked(ObjectDelta obj) 
        {
            var id = KeyOf(obj);

            var exists = Items.TryGetValue(id, out var deltas);
            if (!exists) 
            {
                deltas = new LinkedList<ObjectDelta>();
                
            } 
            deltas.AddLast(obj);
            
            var combinedDeltaList = CombineDeltas(deltas);

            if (combinedDeltaList?.Count > 0) 
            {
                Items[id] = combinedDeltaList;
                if (!exists) 
                {
                    if(!_readerToWriterQueue.Writer.TryWrite(id))
                        throw new ChannelClosedException();
                }
            } 
            else 
            {
                Items.Remove(id);
            }
        }

        private string KeyOf(ObjectDelta obj) => obj.Key; 
        private string KeyOf(LinkedList<ObjectDelta> obj)
        {
            if(obj.Count == 0)
                throw new InvalidOperationException("0 length Deltas object; can't get key");
            return obj.First.Value.Key;
        }

        private string KeyOf(TApi obj) => _keyFunc(obj);



        public void Replace(List<TApi> list, string resourceVersion)
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                var keys = new HashSet<string>();
                foreach (var obj in list) 
                {
                    var key = KeyOf(obj);
                    keys.Add(key);
                    QueueActionLocked(AsDelta(DeltaType.Replaced, obj));
                }

                if (_knownObjects == null) 
                {
                    foreach (var entry in Items) 
                    {
                        if (keys.Contains(entry.Key)) 
                        {
                            continue;
                        }

                        var deletedObj = entry.Value.Last?.Value.Object; // get newest
                        QueueActionLocked(new ObjectDelta
                        {
                            DeltaType = DeltaType.Deleted,
                            Key = entry.Key,
                            Object = deletedObj,
                            IsFinalStateUnknown = true
                        });
                    }
                    if (!Populated) 
                    {
                        Populated = true;
                        InitialPopulationCount = list.Count;
                    }
                    return;
                }

                // Detect deletions not already in the queue.
                var knownKeys = _knownObjects.Keys;
                foreach (var knownKey in knownKeys) 
                {
                    if (keys.Contains(knownKey)) 
                    {
                        continue;
                    }

                    var deletedObj = _knownObjects[knownKey];
                    if (deletedObj == null) 
                    {
                        _log?.LogWarning(
                            "Key {} does not exist in known objects store, placing DeleteFinalStateUnknown marker without object",
                            knownKey);
                    }
                    QueueActionLocked(new ObjectDelta
                    {
                        DeltaType = DeltaType.Deleted,
                        Key = knownKey,
                        Object = deletedObj,
                        IsFinalStateUnknown = true
                    });                }

                if (!Populated) 
                {
                    Populated = true;
                    InitialPopulationCount = list.Count;
                }
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        public void Resync()
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (_knownObjects == null) 
                {
                    return;
                }

                var keys = _knownObjects.Keys;
                foreach (var key in keys) 
                {
                    SyncKeyLocked(key);
                }
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }
        private void SyncKeyLocked(string key) 
        {
            var obj = _knownObjects[key];
            if (obj == null) 
            {
                return;
            }

            var id = KeyOf(obj);
            if(Items.TryGetValue(id, out var deltas) && deltas.Any())
            {
                return;
            }

            QueueActionLocked(AsDelta(DeltaType.Sync, obj));
        }
        public List<string> Keys
        {
            get
            {
                _lock.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    return Items.Select(x => x.Key).ToList();
                }
                finally
                {
                    _lock.ReleaseReaderLock();
                }
            }
        }

        public LinkedList<ObjectDelta> this[TApi obj] => this[KeyOf(obj)];

        public LinkedList<ObjectDelta> this[string key]
        {
            get
            {
                _lock.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    var deltas = Items.GetOrDefault(key);
                    if (deltas != null)
                    {
                        // returning a shallow copy
                        return new LinkedList<ObjectDelta>();
                    }
                }
                finally
                {
                    _lock.ReleaseReaderLock();
                }
                return null;
            }
        }
        
        public class ObjectDelta
        {
            public DeltaType DeltaType { get; set; }
            public bool IsFinalStateUnknown { get; set; }
            public TApi Object { get; set; }
            public string Key { get; set; }
        }

        public IEnumerator<LinkedList<ObjectDelta>> GetEnumerator()
        {
            _lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                return Items.Select(x => x.Value).ToList().GetEnumerator();
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        
        // re-listing and watching can deliver the same update multiple times in any
        // order. This will combine the most recent two deltas if they are the same.
        private LinkedList<ObjectDelta> CombineDeltas(LinkedList<ObjectDelta> deltas) 
        {
            if (deltas.Count < 2) 
            {
                return deltas;
            }
            var size = deltas.Count;
            var d1 = deltas.Last?.Value;
            var d2 = deltas.ToList()[size - 2];
            var @out = IsDuplicate(d1, d2);
            if (@out != null) 
            {
                var newDeltas = new LinkedList<ObjectDelta>();
                foreach (var item in deltas.ToList().GetRange(0, size - 2))
                {
                    newDeltas.AddLast(item);
                }
                newDeltas.AddLast(@out);
                return newDeltas;
            }
            return deltas;
        }
        private ObjectDelta IsDuplicate(
            ObjectDelta d1, ObjectDelta d2) 
        {
            var deletionDelta = IsDeletionDup(d1, d2);
            if (deletionDelta != null) 
            {
                return deletionDelta;
            }
            return null;
        }
        private ObjectDelta IsDeletionDup(
            ObjectDelta d1, ObjectDelta d2) {
            if (!d1.DeltaType.Equals(DeltaType.Deleted) || !d2.DeltaType.Equals(DeltaType.Deleted)) {
                return null;
            }
            if (d2.DeltaType == DeltaType.Deleted && d2.IsFinalStateUnknown) 
            {
                return d1;
            }
            return d2;
        }

    }

    
}