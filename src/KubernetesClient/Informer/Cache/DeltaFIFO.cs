// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using Microsoft.Extensions.Logging;
//
// namespace k8s.Informer.Cache
// {
//     public class DeltaFifo<TApiType> : IStore<object,TApiType>
//     {
//         
//         private Func<TApiType, string> _keyFunc;
//
//         // Mapping deltas w/ key by calling keyFunc
//         private Dictionary<string, LinkedList<MutablePair<DeltaType, object>>> _items = new Dictionary<string, LinkedList<MutablePair<DeltaType, object>>>();
//
//         // an underlying queue storing incoming items' keys
//         private LinkedList<string> _queue = new LinkedList<string>();
//
//         //
//         private IStore<TApiType> _knownObjects;
//         private readonly ILogger _log;
//
//         // populated is true if the first batch of items inserted by Replace() has
//         // been populated or Delete/Add/Update was called first.
//         private bool _populated = false;
//
//         // initialPopulationCount is the number of items inserted by the first call
//         // of Replace()
//         private int _initialPopulationCount;
//
//         /** lock provides thread safety * */
//         private ReaderWriterLock _lock = new ReaderWriterLock();
//
//         /** indicates if the store is empty * */
//         private ManualResetEvent _notEmpty = new ManualResetEvent(false);
//         
//         public DeltaFifo(Func<TApiType, string> keyFunc, IStore<TApiType> knownObjects, ILogger<DeltaFifo<TApiType>> log = null) 
//         {
//             _keyFunc = keyFunc;
//             _knownObjects = knownObjects;
//             _log = log;
//         }
//
//         public void Add(TApiType obj)
//         {
//             _lock.AcquireWriterLock(TimeSpan.MaxValue);
//             try
//             {
//                 _populated = true;
//                 QueueActionLocked(DeltaType.Added, obj);
//             }
//             finally
//             {
//                 _lock.ReleaseWriterLock();
//             }
//         }
//
//         public void Update(TApiType obj)
//         {
//             _lock.AcquireWriterLock(TimeSpan.MaxValue);
//             try
//             {
//                 _populated = true;
//                 QueueActionLocked(DeltaType.Updated, obj);
//             }
//             finally
//             {
//                 _lock.ReleaseWriterLock();
//             }
//         }
//
//         public void Delete(TApiType obj)
//         {
//             var id = KeyOf(obj);
//             _lock.AcquireWriterLock(TimeSpan.MaxValue);
//             try
//             {
//                 _populated = true;
//                 if (_knownObjects == null)
//                 {
//                     if (_items.ContainsKey(id))
//                     {
//                         // Presumably, this was deleted when a relist happened.
//                         // Don't provide a second report of the same deletion.
//                         return;
//                     }
//                 }
//                 else
//                 {
//                     // We only want to skip the "deletion" action if the object doesn't
//                     // exist in knownObjects and it doesn't have corresponding item in items.
//                     if (_knownObjects[id] == null && !_items.ContainsKey(id))
//                     {
//                         return;
//                     }
//                 }
//
//                 QueueActionLocked(DeltaType.Deleted, obj);
//             }
//             finally
//             {
//                 _lock.ReleaseWriterLock();
//             }
//         }
//
//         public void Replace(List<TApiType> list, string resourceVersion)
//         {
//             _lock.AcquireWriterLock(TimeSpan.MaxValue);
//             try
//             {
//                 var keys = new HashSet<string>();
//                 foreach (var obj in list) {
//                     var key = KeyOf(obj);
//                     keys.Add(key);
//                     QueueActionLocked(DeltaType.Sync, obj);
//                 }
//
//                 if (_knownObjects == null) 
//                 {
//                     foreach (var entry in _items) 
//                     {
//                         if (keys.Contains(entry.Key)) 
//                         {
//                             continue;
//                         }
//
//                         object deletedObj = null;
//                         var delta = entry.Value.Last; // get newest
//                         if (delta != null) 
//                         {
//                             deletedObj = delta.Value.Right;
//                         }
//                         QueueActionLocked(DeltaType.Deleted, new DeletedFinalStateUnknown(entry.Key, deletedObj));
//                     }
//
//                     if (!_populated) 
//                     {
//                         _populated = true;
//                         _initialPopulationCount = list.Count;
//                     }
//                     return;
//                 }
//
//                 // Detect deletions not already in the queue.
//                 var knownKeys = _knownObjects.Keys;
//                 var queueDeletion = 0;
//                 foreach (var knownKey in knownKeys) 
//                 {
//                     if (keys.Contains(knownKey)) 
//                     {
//                         continue;
//                     }
//
//                     var deletedObj = _knownObjects[knownKey];
//                     if (deletedObj == null) 
//                     {
//                         _log?.LogWarning(
//                             "Key {} does not exist in known objects store, placing DeleteFinalStateUnknown marker without object",
//                             knownKey);
//                     }
//                     queueDeletion++;
//                     QueueActionLocked(DeltaType.Deleted, new DeletedFinalStateUnknown(knownKey, deletedObj));
//                 }
//
//                 if (!_populated) 
//                 {
//                     _populated = true;
//                     _initialPopulationCount = list.Count + queueDeletion;
//                 }
//             }
//             finally
//             {
//                 _lock.ReleaseWriterLock();
//             }
//         }
//
//         public void Resync()
//         {
//             _lock.AcquireWriterLock(TimeSpan.MaxValue);
//             try
//             {
//                 if (_knownObjects == null) 
//                 {
//                     return;
//                 }
//
//                 var keys = _knownObjects.Keys;
//                 foreach (var key in keys) 
//                 {
//                     SyncKeyLocked(key);
//                 }
//             }
//             finally
//             {
//                 _lock.ReleaseWriterLock();
//             }
//         }
//
//         public List<string> Keys
//         {
//             get
//             {
//                 lock (_lock)
//                 {
//                     return _items.Select(x => x.Key).ToList();
//                 }
//             }
//             
//         }
//
//         public object this[TApiType obj] => this[KeyOf(obj)];
//
//         public object this[string key]
//         {
//             get
//             {
//                 _lock.AcquireReaderLock(TimeSpan.MaxValue);
//                 try
//                 {
//                     var deltas = _items.GetOrDefault(key);
//                     if (deltas != null)
//                     {
//                         // returning a shallow copy
//                         return new LinkedList<MutablePair<DeltaType, object>>();
//                     }
//                 }
//                 finally
//                 {
//                     _lock.ReleaseReaderLock();
//                 }
//                 return null;
//             }
//         }
//         public IEnumerator<object> GetEnumerator()
//         {
//             List<object> list;
//             _lock.AcquireReaderLock(TimeSpan.MaxValue);
//             try
//             {
//                 list = _items
//                     .Values
//                     .Select(x => new LinkedList<MutablePair<DeltaType, object>>(x))
//                     .Cast<object>()
//                     .ToList();
//             }
//             finally
//             {
//                 _lock.ReleaseReaderLock();
//             }
//
//             return list.GetEnumerator();
//         }
//
//         IEnumerator IEnumerable.GetEnumerator()
//         {
//             return GetEnumerator();
//         }
//
//         public LinkedList<MutablePair<DeltaType, object>> Pop(Action<LinkedList<MutablePair<DeltaType, object>>> func)
//         {
//             //todo: this whole thing needs to be async
//             _lock.AcquireWriterLock(TimeSpan.MaxValue);
//             try
//             {
//                 while (true)
//                 {
//                     while (_queue.Count == 0)
//                     {
//                         _notEmpty.WaitOne();
//                     }
//
//                     // there should have data now
//                     var id = _queue.First.Value;
//                     _queue.RemoveFirst();
//                     if (_queue.Count == 0)
//                     {
//                         _notEmpty.Reset();
//                     }
//                     if (_initialPopulationCount > 0)
//                     {
//                         _initialPopulationCount--;
//                     }
//
//                     if (!_items.ContainsKey(id))
//                     {
//                         // Item may have been deleted subsequently.
//                         continue;
//                     }
//
//                     var deltas = _items[id];
//                     _items.Remove(id);
//                     func(deltas);
//                     // Don't make any copyDeltas here
//                     return deltas;
//
//                 }
//             }
//             finally
//             {
//                 _lock.ReleaseWriterLock();
//             }
//         }
//         
//         public bool HasSynced 
//         {
//             get
//             {
//                 _lock.AcquireReaderLock(TimeSpan.MaxValue);
//                 try
//                 {
//                     return _populated && _initialPopulationCount == 0;
//                 }
//                 finally
//                 {
//                     _lock.ReleaseReaderLock();
//                 }
//             }
//         }
//         private void QueueActionLocked(DeltaType actionType, object obj) 
//         {
//             var id = KeyOf(obj);
//
//             var deltas = _items.GetOrDefault(id);
//             if (deltas == null) 
//             {
//                 var deltaList = new LinkedList<MutablePair<DeltaType, object>>();
//                 deltaList.AddFirst(new MutablePair<DeltaType, object>(actionType, obj));
//                 deltas = new LinkedList<MutablePair<DeltaType, object>>(deltaList);
//             } 
//             else 
//             {
//                 deltas.AddFirst(new MutablePair<DeltaType, object>(actionType, obj));
//             }
//
//             var combinedDeltaList =  CombineDeltas(deltas);
//
//             var exist = _items.ContainsKey(id);
//             if (combinedDeltaList != null && combinedDeltaList.Count > 0) 
//             {
//                 if (!exist) 
//                 {
//                     _queue.AddFirst(id);
//                 }
//                 _items[id] = new LinkedList<MutablePair<DeltaType, object>>(combinedDeltaList);
//                 _notEmpty.Set();
//             } 
//             else 
//             {
//                 _items.Remove(id);
//             }
//         }
//         
//         private string KeyOf(object obj) 
//         {
//             var innerObj = obj;
//             if (obj is LinkedList<MutablePair<DeltaType, object>> deltas) 
//             {
//                 if (deltas.Count == 0) 
//                 {
//                     throw new KeyNotFoundException("0 length Deltas object; can't get key");
//                 }
//                 innerObj = deltas.Last.Value.Right;
//             }
//             if (innerObj is DeletedFinalStateUnknown) 
//             {
//                 return ((DeletedFinalStateUnknown) innerObj).Key;
//             }
//             return _keyFunc((TApiType) innerObj);
//         }
//         
//         private void SyncKeyLocked(string key) 
//         {
//             var obj = _knownObjects[key];
//             if (obj == null) 
//             {
//                 return;
//             }
//
//             var id = KeyOf(obj);
//             var deltas = _items.GetOrDefault(id);
//             if (deltas != null && deltas.Count != 0) 
//             {
//                 return;
//             }
//
//             this.QueueActionLocked(DeltaType.Sync, obj);
//         }
//         
//         // re-listing and watching can deliver the same update multiple times in any
//         // order. This will combine the most recent two deltas if they are the same.
//         private LinkedList<MutablePair<DeltaType, object>> CombineDeltas(
//             LinkedList<MutablePair<DeltaType, object>> deltas) 
//         {
//             if (deltas.Count < 2) 
//             {
//                 return deltas;
//             }
//             var size = deltas.Count;
//             var d1 = deltas.Last?.Value;
//             var d2 = deltas.ToList()[size - 2];
//             var @out = IsDuplicate(d1, d2);
//             if (@out != null) 
//             {
//                 var newDeltas = new LinkedList<MutablePair<DeltaType, object>>();
//                 foreach (var item in deltas.ToList().GetRange(0, size - 2))
//                 {
//                     newDeltas.AddLast(item);
//                 }
//                 newDeltas.AddFirst(@out);
//                 return newDeltas;
//             }
//             return deltas;
//         }
//         private MutablePair<DeltaType, Object> IsDuplicate(
//             MutablePair<DeltaType, Object> d1, MutablePair<DeltaType, Object> d2) 
//         {
//             var deletionDelta = IsDeletionDup(d1, d2);
//             if (deletionDelta != null) 
//             {
//                 return deletionDelta;
//             }
//             return null;
//         }
//         private MutablePair<DeltaType, Object> IsDeletionDup(
//             MutablePair<DeltaType, Object> d1, MutablePair<DeltaType, Object> d2) {
//             if (!d1.Left.Equals(DeltaType.Deleted) || !d2.Left.Equals(DeltaType.Deleted)) {
//                 return null;
//             }
//             Object obj = d2.Right;
//             if (obj is DeletedFinalStateUnknown) 
//             {
//                 return d1;
//             }
//             return d2;
//         }
//         
//         public class DeletedFinalStateUnknown
//         {
//
//             public string Key { get; }
//             public  object Obj { get; }
//
//             public DeletedFinalStateUnknown(string key, object obj) 
//             {
//                 Key = key;
//                 Obj = obj;
//             }
//         }
//         
//         
//         
//     }
     public enum DeltaType
     {
         Added,
         Updated,
         Deleted,
         Sync,
         Replaced
     }
//
// }