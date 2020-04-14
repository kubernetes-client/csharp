using System;

namespace k8s.Informers
{
    /// <summary>
    ///     The type of resource observable stream that specifies whether to return current state of resource, observe changes, or both
    /// </summary>
    [Flags]
    public enum ResourceStreamType
    {
        /// <summary>
        ///     A Cold observable that returns current state of resources and then completes
        /// </summary>
        List = 1,

        /// <summary>
        ///     A Hot observable that publishes a list of changes as they happen
        /// </summary>
        Watch = 2,

        /// <summary>
        ///     A Hot observable that Lists current state of resources followed by watch.
        /// </summary>
        ListWatch = List | Watch
    }
}
