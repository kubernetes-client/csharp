namespace k8s.Models;

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
