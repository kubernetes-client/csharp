namespace k8s
{
    /// <summary>
    /// Kubernetes object that exposes list of objects
    /// </summary>
    /// <typeparam name="T">type of the objects</typeparam>
    public interface IItems<T>
    {
        /// <summary>
        /// Gets or sets list of objects. More info:
        /// https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md
        /// </summary>
        IList<T> Items { get; set; }
    }

    public static class ItemsExt
    {
        public static IEnumerator<T> GetEnumerator<T>(this IItems<T> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.Items.GetEnumerator();
        }
    }
}
