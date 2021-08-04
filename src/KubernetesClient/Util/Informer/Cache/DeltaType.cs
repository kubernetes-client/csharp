namespace k8s.Util.Cache
{
    public enum DeltaType
    {
        /// <summary>
        /// Item added
        /// </summary>
        Added,

        /// <summary>
        /// Item updated
        /// </summary>
        Updated,

        /// <summary>
        /// Item deleted
        /// </summary>
        Deleted,

        /// <summary>
        /// Item synchronized
        /// </summary>
        Sync,

        /// <summary>
        /// Item replaced
        /// </summary>
        Replaced,
    }
}
