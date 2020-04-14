using System;

namespace k8s.Informers.Notifications
{
    /// <summary>
    ///     Denotes flags that specify how the event in resource observable stream should be interpreted.
    ///     Note that more then one value is usually set - use HasFlag instead of equals
    /// </summary>
    [Flags]
    public enum EventTypeFlags
    {
        /// <summary>
        ///     A resource was added
        /// </summary>
        Add = 1,

        /// <summary>
        ///     A resource was deleted
        /// </summary>
        Delete = 2,

        /// <summary>
        ///     A resource was modified
        /// </summary>
        Modify = 4,

        /// <summary>
        ///     State of the resource has not changed and the intent of the message is inform of current state
        /// </summary>
        Current = 8,

        /// <summary>
        ///     The current state of the resource is published as part of regular synchronization interval
        /// </summary>
        Sync = 16,

        /// <summary>
        ///     The state of the resource has been reset, and all subscribers should reset their existing cache values based on the new
        /// </summary>
        Reset = 32,

        /// <summary>
        ///     The start of a sequence of reset messages, usually used to mark the start of a List operation
        /// </summary>
        ResetStart = 64,

        /// <summary>
        ///     The end of a sequence of reset messages, usually used to mark the start of a List operation
        /// </summary>
        ResetEnd = 128,

        /// <summary>
        ///     Marks the boundary between empty list operation and the start of watch in an observable stream
        /// </summary>
        ResetEmpty = 256,

        /// <summary>
        ///     The event was computed through discrepancy reconciliation with server rather then explicit event.
        ///     This can occur when relisting after reconnect to resource server when there are items in local cache that
        ///     don't match what is in cache, so there must have been updates that were missed. By comparing current state
        ///     and old state (cache), we can compute the kind of events that we missed and emit them with this flag
        /// </summary>
        Computed = 512
    }
}
