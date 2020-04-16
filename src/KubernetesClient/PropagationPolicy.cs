namespace k8s
{
    /// <summary>
    /// Determines how garbage collection will be performed
    /// </summary>
    public enum PropagationPolicy
    {
        /// <summary>
        /// Orphan the dependents
        /// </summary>
        Orphan,
        /// <summary>
        ///  Allow the garbage collector to delete the dependents in the background
        /// </summary>
        Background,
        /// <summary>
        /// A cascading policy that deletes all dependents in the foreground.
        /// </summary>
        Foreground

    }
}
